using Paganism.Lexer;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser
{
    public class Parser
    {
        public Parser(Token[] tokens)
        {
            Tokens = tokens;
        }

        public int Position { get; private set; }

        public Token[] Tokens { get; }

        public Token Current => Tokens[Position];

        public BlockStatementExpression Run()
        {
            var expressions = new List<IStatement>();

            while (Position < Tokens.Length)
            {
                expressions.Add(ParseStatement());
            }

            return new BlockStatementExpression(expressions.ToArray());
        }

        private IStatement ParseStatement()
        {
            if (Match(TokenType.If))
            {
                return ParseIf();
            }

            if (Match(TokenType.Function))
            {
                return ParseDeclarateFunction();
            }

            if (Match(TokenType.Return))
            {
                return ParseReturn();
            }

            if (Current.Type == TokenType.Word) {

                if (Require(1, TokenType.LeftPar))
                {
                    return ParseFunctionCall();
                }
                else if (Require(1, TokenType.Assign))
                {
                    return ParseDeclarateOrSetVariable();
                }
            }

            if (Require(0, TokenType.StringType, TokenType.NumberType, TokenType.AnyType))
            {
                return ParseDeclarateFunctionOrVariable();
            }
 
            return null;
        }

        private IStatement ParseIf()
        {
            if (!Match(TokenType.LeftPar))
            {
                throw new Exception("Except (");
            }

            var expression = ParseBinary() as IEvaluable;

            if (!Match(TokenType.RightPar)) throw new Exception("Except ')'");

            if (!Match(TokenType.Then)) throw new Exception("Except 'then'");

            List<IStatement> statements = new List<IStatement>();

            while (!Match(TokenType.End))
            {
                statements.Add(ParseStatement());
            }

            return new IfExpression(expression, new BlockStatementExpression(statements.ToArray()), new BlockStatementExpression(null));
        }

        private IStatement ParseDeclarateFunctionOrVariable()
        {
            List<TokenType> types = new List<TokenType>();

            while (Require(0, TokenType.StringType, TokenType.NumberType, TokenType.AnyType))
            {
                types.Add(Current.Type);
                Position++;
            }

            if (Match(TokenType.Function))
            {
                return ParseDeclarateFunction(types.ToArray());
            }
            else
            {
                return ParseDeclarateOrSetVariable(true);
            }
        }

        private IStatement ParseDeclarateOrSetVariable(bool isWithType = false)
        {
            var type = TokenType.AnyType;

            if (isWithType)
            {
                type = Current.Type;
                Position++; //Skip type
            }

            var left = ParsePrimary() as IEvaluable;

            Match(TokenType.Assign);

            var right = ParseBinary() as IEvaluable;

            var valueType = type;

            if (left is VariableExpression variable)
            {
                left = new VariableExpression(variable.Name, valueType);
            }

            return new AssignExpression(BinaryOperatorType.Assign, left, right);
        }

        private FunctionCallExpression ParseFunctionCall()
        {
            var name = Current.Value;

            Match(TokenType.Word);

            var arguments = new List<Argument>();

            if (Match(TokenType.LeftPar))
            {
                while (!Match(TokenType.RightPar))
                {
                    if (Match(TokenType.Comma))
                    {
                        continue;
                    }

                    arguments.Add(new Argument(string.Empty, Current.Type, true, ParseBinary() as IEvaluable));
                }
            }

            return new FunctionCallExpression(name, arguments.ToArray());
        }

        private ReturnExpression ParseReturn()
        {
            List<Expression> expressions = new List<Expression>();

            while (true)
            {
                if (Match(TokenType.Comma))
                {
                    continue;
                }

                if (Current.Type == TokenType.Number)
                {
                    expressions.Add(ParseBinary());

                    continue;
                }
                else if (Current.Type == TokenType.String)
                {
                    expressions.Add(new StringExpression(Current.Value));

                    Position++;
                    continue;
                }

                break;
            }

            return new ReturnExpression(expressions.ToArray());
        }

        private FunctionDeclarateExpression ParseDeclarateFunction(params TokenType[] returnTypes)
        {
            var name = string.Empty;
            var arguments = new List<Argument>();

            var current = Current;

            if (Match(TokenType.Word))
            {
                name = current.Value;
            }

            if (Match(TokenType.LeftPar))
            {
                var lastType = TokenType.AnyType;

                while (!Match(TokenType.RightPar))
                {
                    if (Match(TokenType.NoneType))
                    {
                        continue;
                    }

                    var previousCurrent = Current;
                    
                    if (Match(TokenType.Comma))
                    {
                        continue;
                    }

                    if (Match(TokenType.StringType, TokenType.NumberType, TokenType.BooleanType))
                    {
                        lastType = previousCurrent.Type;
                    }

                    previousCurrent = Current;

                    if (Match(TokenType.Word))
                    {
                        arguments.Add(new Argument(previousCurrent.Value, lastType, true));

                        lastType = TokenType.AnyType;
                    }
                }
            }

            List<IStatement> statements = new List<IStatement>();

            while (!Match(TokenType.End))
            {
                statements.Add(ParseStatement());
            }

            return new FunctionDeclarateExpression(name, new BlockStatementExpression(statements.ToArray()), arguments.ToArray(), returnTypes);
        }

        private Expression ParseBinary()
        {
            var result = ParseMultiplicative();

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Plus))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Plus, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Minus))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Minus, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Is))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Is, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                break;
            }

            return result;
        }

        private Expression ParseMultiplicative()
        {
            var result = ParseUnary();

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Star))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Multiplicative, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Slash))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Division, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                break;
            }

            return result;
        }

        private Expression ParseUnary()
        {
            if (Match(TokenType.Plus))
            {
                return ParsePrimary();
            }
            if (Match(TokenType.Minus))
            {
                return new UnaryExpression((IEvaluable)ParsePrimary(), BinaryOperatorType.Minus);
            }

            return ParsePrimary();
        }

        private Expression ParsePrimary()
        {
            var current = Current;

            if (Match(TokenType.Number))
            {
                return new NumberExpression(double.Parse(current.Value.Replace(".", ",")));
            }
            else if (Match(TokenType.String))
            {
                return new StringExpression(current.Value);
            }
            else if (Match(TokenType.True, TokenType.False))
            {
                return new BooleanExpression(bool.Parse(current.Value));
            }
            else if(Require(0, TokenType.Word) && Require(1, TokenType.LeftPar))
            {
                return ParseFunctionCall();
            }
            else if (Match(TokenType.Word))
            {
                return new VariableExpression(current.Value, TokenType.AnyType);
            }

            if (Match(TokenType.LeftPar))
            {
                var result = ParseBinary();
                Match(TokenType.RightPar);
                return result;
            }

            return ParseStatement() as Expression;
        }

        private bool Match(params TokenType[] type)
        {
            if (type.Contains(Current.Type))
            {
                Position++;

                return true;
            }

            return false;
        }

        private bool Require(int relativePosition, params TokenType[] type)
        {
            var position = Position + relativePosition;

            return type.Contains(Tokens[position].Type);
        }
    }
}
