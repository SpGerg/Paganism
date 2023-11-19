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

        public bool InLoop { get; private set; }

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

            if (Match(TokenType.For))
            {
                return ParseFor();
            }

            if (Match(TokenType.Function))
            {
                return ParseDeclarateFunction();
            }

            if (Match(TokenType.Structure))
            {
                return ParseStructure();
            }

            if (Match(TokenType.Return))
            {
                return ParseReturn();
            }

            if (Match(TokenType.Break))
            {
                return ParseBreak();
            }

            if (Current.Type == TokenType.Word)
            {
                if (Require(1, TokenType.LeftPar))
                {
                    return ParseFunctionCall();
                }
                else if (Require(1, TokenType.Assign) || Require(1, TokenType.LeftBracket))
                {
                    return ParseDeclarateOrSetVariable();
                }
            }

            if (Require(0, TokenType.StringType, TokenType.NumberType, TokenType.AnyType, TokenType.BooleanType))
            {
                return ParseDeclarateFunctionOrVariable();
            }
 
            return null;
        }

        private IStatement ParseStructure()
        {
            var name = Current.Value;

            if (!Match(TokenType.Word)) throw new Exception("Except structure name");

            List<StructureMemberExpression> statements = new List<StructureMemberExpression>();

            while (!Match(TokenType.End))
            {
                var isCastable = false;
                var isShow = false;

                if (Match(TokenType.Show))
                {
                    isShow = true;
                }

                if (Match(TokenType.Castable))
                {
                    isCastable = true;
                }

                var current = Current.Type;

                if (!Match(TokenType.StringType, TokenType.NumberType, TokenType.AnyType, TokenType.BooleanType)) throw new Exception("Except structure member type");

                var memberName = Current.Value;

                if (!Match(TokenType.Word)) throw new Exception("Except structure member name");

                statements.Add(new StructureMemberExpression(current, memberName, isShow, isCastable));

                if (!Match(TokenType.Comma)) throw new Exception("Except ','");
            }

            return new StructureDeclarateExpression(name, statements.ToArray());
        }

        private IStatement ParseFor()
        {
            if (!Match(TokenType.LeftPar)) throw new Exception("Except '('");

            var variable = ParseDeclarateOrSetVariable();

            if (!Match(TokenType.Comma)) throw new Exception("Except ','");

            var expression = ParseBinary() as IEvaluable;

            if (!Match(TokenType.Comma)) throw new Exception("Except ','");

            var action = ParseStatement();

            if (!Match(TokenType.RightPar)) throw new Exception("Except ')'");

            InLoop = true;

            List<IStatement> statements = new List<IStatement>();

            while (!Match(TokenType.End))
            {
                statements.Add(ParseStatement());
            }

            InLoop = false;

            return new ForExpression(new BlockStatementExpression(statements.ToArray(), true), expression, new BlockStatementExpression(new IStatement[] { action }), variable);
        }

        private IStatement ParseBreak()
        {
            return new BreakExpression();
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

            return new IfExpression(expression, new BlockStatementExpression(statements.ToArray(), InLoop), new BlockStatementExpression(null));
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

            var isArray = false;

            IEvaluable left;
            IEvaluable right;

            left = ParsePrimary() as IEvaluable;

            if (Match(TokenType.LeftBracket))
            {
                isArray = true;

                if (!Match(TokenType.RightBracket)) throw new Exception("Except ]");
            }

            Match(TokenType.Assign);

            if (isArray)
            {
                Match(TokenType.LeftBracket);

                right = ParseArray(type) as IEvaluable;
            }
            else
            {
                right = ParseBinary() as IEvaluable;
            }

            var valueType = type;

            if (left is VariableExpression variable)
            {
                left = new VariableExpression(variable.Name, valueType);
            }

            if (right is ArrayExpression array)
            {
                right = new ArrayExpression(array.Elements, array.Length, Lexer.Tokens.TokenTypeToValueType[type]);
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

            return new FunctionDeclarateExpression(name, new BlockStatementExpression(statements.ToArray(), InLoop), arguments.ToArray(), returnTypes);
        }

        private Expression ParseElementFromArray()
        {
            var name = Current.Value;

            Match(TokenType.Word);

            Match(TokenType.LeftBracket);

            var index = ParseBinary() as IEvaluable;

            Match(TokenType.Number);

            if (!Match(TokenType.RightBracket))
            {
                throw new Exception("Except ']'");
            }

            return new ArrayElementExpression(name, index);
        }

        private Expression ParseArray(TokenType type = TokenType.AnyType)
        {
            List<Expression> elements = new List<Expression>();

            while (!Match(TokenType.RightBracket))
            {
                if (Match(TokenType.Comma))
                {
                    continue;
                }

                var element = ParseBinary() as IEvaluable;

                if (type != TokenType.AnyType && element != null && Lexer.Tokens.ValueTypeToTokenType[element.Eval().Type] == type)
                {
                    throw new Exception($"Except {type} type");
                }

                elements.Add(element as Expression);
            }

            return new ArrayExpression(elements.ToArray(), elements.Count);
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

                if (Match(TokenType.And))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.And, result as IEvaluable, ParseBinary() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Or))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Or, result as IEvaluable, ParseBinary() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Less))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Less, result as IEvaluable, ParseBinary() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.More))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.More, result as IEvaluable, ParseBinary() as IEvaluable);
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
            else if (Require(0, TokenType.Word) && Require(1, TokenType.LeftBracket) && (Require(2, TokenType.Number) || Require(2, TokenType.Plus, TokenType.Minus)))
            {
                return ParseElementFromArray();
            }
            else if(Require(0, TokenType.Word) && Require(1, TokenType.LeftPar))
            {
                return ParseFunctionCall();
            }
            else if (Match(TokenType.Word))
            {
                return new VariableExpression(current.Value, TokenType.AnyType);
            }
            else if (Match(TokenType.LeftPar))
            {
                var result = ParseBinary();
                Match(TokenType.RightPar);
                return result;
            }
            else if (Match(TokenType.LeftBracket))
            {
                return ParseArray();
            }
            else if (Match(TokenType.Not))
            {
                return new NotExpression(ParseBinary() as IEvaluable);
            }

            throw new Exception($"Unknown expression {Current.Value}");
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
