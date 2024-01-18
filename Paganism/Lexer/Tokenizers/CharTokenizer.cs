using Paganism.Exceptions;
using Paganism.Lexer.Enums;

namespace Paganism.Lexer.Tokenizers
{
    public class CharTokenizer : Tokenizer
    {
        public CharTokenizer(string[] text, int position, int line)
        {
            Text = text;
            Line = line;
            Position = position;
        }

        public string[] Text { get; }

        public char Current => Text[Line][Position];

        public int Line { get; private set; }

        public int Position { get; private set; }

        public override Token Tokenize()
        {
            Position++; //Skip '

            int startPosition = Position;
            int startLine = Line;

            if (Current == '\'')
            {
                throw new LexerException($"Char need contains character", startLine, startPosition);
            }

            string savedLine = string.Empty;

            while (Line < Text.Length)
            {
                while (Position < Text[Line].Length)
                {
                    if (Position - startPosition > 1)
                    {
                        if (savedLine[0] == '\\')
                        {
                            var code = Utilities.ReplaceEscapeCode(savedLine);

                            if (code == savedLine)
                            {
                                throw new LexerException($"Unknown identifier '{savedLine[1]}'", startLine, startPosition);
                            }
                        }
                        else
                        {
                            throw new LexerException($"Char cant contains more 1 characters", startLine, startPosition);
                        }
                    }

                    if (Current == '\'')
                    {
                        Position++;

                        return savedLine == "\\"
                        {
                            throw new LexerException($"After \\ need identifier (\\n, \\t e.t.c)", startLine, startPosition);
                        }

                        return new Token(Utilities.ReplaceEscapeCodes(savedLine), startPosition, startLine, TokenType.Char);
                    }

                    savedLine += Current;
                    Position++;
                }

                Line++;
            }

            throw new LexerException($"Char cant contains more 1 characters", startLine, startPosition);
        }
    }
}
