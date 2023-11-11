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

        public IStatement Run()
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
            Console.WriteLine(Current.Type);
            Console.WriteLine(Current.Value);

            if (Current.Type == TokenType.Function)
            {
                return ParseDeclarateFunction();
            }

            if (Current.Type == TokenType.Word) {
                return ParseFunctionCall();
            }
 
            return null;
        }

        private FunctionCallExpression ParseFunctionCall()
        {
            var name = Current.Value;

            Position++;

            var arguments = new List<Argument>();

            if (Match(TokenType.LeftPar))
            {
                while (!Match(TokenType.RightPar))
                {
                    arguments.Add(new Argument("test", TokenType.NumberType, true));

                    Position++;
                }
            }

            return new FunctionCallExpression(name, arguments.ToArray());
        }

        private FunctionDeclarateExpression ParseDeclarateFunction()
        {
            Position++; //Skip function

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

            return new FunctionDeclarateExpression(name, ParseStatement(), arguments.ToArray());
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

            /*
            if (Match(TokenType.LeftPar))
            {
                var result = ParseExpression();
                Match(TokenType.RightPar);
                return result;
            }
            */

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
