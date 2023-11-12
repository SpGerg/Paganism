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
                Position++;
            }

            return new BlockStatementExpression(expressions.ToArray());
        }

        private IStatement ParseStatement()
        {
            if (Match(TokenType.Function))
            {
                return ParseDeclarateFunction();
            }

            if (Current.Type == TokenType.Word && Get(1, TokenType.LeftPar)) {
                return ParseFunctionCall();
            }
 
            return null;
        }

        private FunctionCallExpression ParseFunctionCall()
        {
            var name = Current.Value;

            Position++;

            var arguments = new List<Argument>();

            var function = Functions.Get(name);

            if (Match(TokenType.LeftPar))
            {
                foreach (var argument in function.RequiredArguments)
                {
                    while (!Match(TokenType.RightPar))
                    {
                        if (Match(TokenType.Comma))
                        {
                            break;
                        }

                        if (Current.Type == TokenType.Number)
                        {
                            var parsed = ParseAdditive() as BinaryOperatorExpression;

                            Position--;

                            arguments.Add(argument.Create(parsed.Eval()));
                        }
                        else
                        {
                            arguments.Add(argument.Create(new StringValue(Current.Value)));
                        }

                        Position++;
                    }
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
                    expressions.Add(ParseAdditive());

                    Position++;
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

            Position--;

            return new ReturnExpression(expressions.ToArray());
        }

        private FunctionDeclarateExpression ParseDeclarateFunction()
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

                    if (Match(TokenType.StringType, TokenType.NumberType))
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
                if (Match(TokenType.Return))
                {
                    statements.Add(ParseReturn());
                    continue;
                }

                statements.Add(ParseStatement());
                Position++;
            }

            Position--;
            return new FunctionDeclarateExpression(name, new BlockStatementExpression(statements.ToArray()), arguments.ToArray());
        }

        private Expression ParseAdditive()
        {
            var result = ParseMultiplicative();

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Plus))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Plus, (IEvaluable)result, (IEvaluable)ParseMultiplicative());
                    continue;
                }

                if (Match(TokenType.Minus))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Minus, (IEvaluable)result, (IEvaluable)ParseMultiplicative());
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
                    result = new BinaryOperatorExpression(BinaryOperatorType.Multiplicative, (IEvaluable)result, (IEvaluable)ParseMultiplicative());
                    continue;
                }

                if (Match(TokenType.Slash))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Division, (IEvaluable)result, (IEvaluable)ParseMultiplicative());
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

            if (Match(TokenType.LeftPar))
            {
                var result = ParseAdditive();
                Match(TokenType.RightPar);
                return result;
            }

            return null;
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

        private bool Get(int relativePosition, params TokenType[] type)
        {
            var position = Position + relativePosition;

            return type.Contains(Tokens[position].Type);
        }
    }
}
