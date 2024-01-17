using Paganism.Lexer.Enums;

namespace Paganism.Lexer
{
    public class Token
    {
        public Token(string value, int position, int line, TokenType type)
        {
            Value = value;
            Position = position;
            Line = line;
            Type = type;
        }

        public string Value { get; }

        public int Position { get; }

        public int Line { get; }

        public TokenType Type { get; }
    }
}
