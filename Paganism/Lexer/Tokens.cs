using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Lexer
{
    public static class Tokens
    {
        public static IReadOnlyDictionary<string, TokenType> KeywordsType { get; } = new Dictionary<string, TokenType>()
        {
            { "function", TokenType.Function },
            { "end", TokenType.End },
            { "string", TokenType.StringType },
            { "number", TokenType.NumberType },
            { "any", TokenType.AnyType },
            { "and", TokenType.And },
            { "return", TokenType.Return },
            { "none", TokenType.NoneType },
        };

        public static IReadOnlyDictionary<string, TokenType> OperatorsType { get; } = new Dictionary<string, TokenType>()
        {
            { "+", TokenType.Plus },
            { "-", TokenType.Minus },
            { "*", TokenType.Star },
            { "/", TokenType.Slash },
            { ";", TokenType.Semicolon },
            { ":", TokenType.Colon },
            { "(", TokenType.LeftPar },
            { ")", TokenType.RightPar },
            { "[", TokenType.LeftBracket },
            { "]", TokenType.RightBracket },
            { ",", TokenType.Comma },
            { "=", TokenType.Assign }
        };

        public static IReadOnlyDictionary<StandartValueType, TokenType> ValueTypeToTokenType { get; } = new Dictionary<StandartValueType, TokenType>()
        {
            { StandartValueType.String, TokenType.StringType },
            { StandartValueType.Number, TokenType.NumberType },
            { StandartValueType.Any, TokenType.AnyType }
        };

        public static IReadOnlyDictionary<string, TokenType> BinaryOperatorsType { get; } = new Dictionary<string, TokenType>()
        {
            { "+", TokenType.Plus },
            { "-", TokenType.Minus },
            { "*", TokenType.Star },
            { "/", TokenType.Slash },
        };
    }
}
