using Paganism.Exceptions;
using Paganism.Lexer.Enums;

namespace Paganism.Lexer.Tokenizers
{
    public class StringTokenizer : Tokenizer
    {
        public StringTokenizer(string[] text, Lexer lexer) : base(lexer) { Text = text; }

        public string[] Text { get; }

        public override Token Tokenize()
        {
            Position++; //Skip "

            int startPosition = Position;
            int startLine = Line;

            string savedLine = string.Empty;

            while (Line < Text.Length)
            {
                while (Position < Text[Line].Length)
                {
                    if (Current == '\"')
                    {
                        Position++;
                        return new Token(Utilities.ReplaceEscapeCodes(savedLine), startPosition, startLine, TokenType.String);
                    }

                    savedLine += Current;
                    Position++;
                }

                Line++;
            }

            throw new LexerException($"String is not ended", startLine, startPosition, Filepath);
        }
    }
}
