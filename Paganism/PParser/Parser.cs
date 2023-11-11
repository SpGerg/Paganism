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

        public Expression[] Run()
        {
            var expressions = new List<Expression>();

            while (Position < Tokens.Length)
            {
                expressions.Add(ParseExpression());
                Position++;
            }

            return expressions.ToArray();
        }

        private Expression ParseExpression()
        {
            return ParseAdditive();
        }

        private Expression ParseAdditive()
        {
            var result = ParseMultiplicative();

            while (true)
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

            while (true)
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
                var result = ParseExpression();
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
    }
}
