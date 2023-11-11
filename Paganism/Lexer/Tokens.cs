using Paganism.Lexer.Enums;
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
            { "end", TokenType.End }
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
            { ",", TokenType.Comma },
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
