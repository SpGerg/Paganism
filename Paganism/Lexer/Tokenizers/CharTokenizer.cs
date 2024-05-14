using Paganism.Exceptions;
using Paganism.Lexer.Enums;

namespace Paganism.Lexer.Tokenizers
{
    public class CharTokenizer : Tokenizer
    {
        public CharTokenizer(string[] text, Lexer lexer) : base(lexer)
        {
            Text = text;
        }

        public string[] Text { get; }

        public override Token Tokenize()
        {
            Position++; //Skip '

            int startPosition = Position;
            int startLine = Line;

            if (Current == '\'')
            {
                throw new LexerException($"Char need contains character", startLine, startPosition, Filepath);
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
                                throw new LexerException($"Unknown identifier '{savedLine[1]}'", startLine, startPosition, Filepath);
                            }
                        }
                        else
                        {
                            throw new LexerException($"Char cant contains more 1 characters", startLine, startPosition, Filepath);
                        }
                    }

                    if (Current == '\'')
                    {
                        Position++;

                        if (savedLine == "\\")
                        {
                            throw new LexerException($"After \\ need identifier (\\n, \\t e.t.c)", startLine, startPosition, Filepath);
                        }

                        return new Token(Utilities.ReplaceEscapeCodes(savedLine), startPosition, startLine, TokenType.Char);
                    }

                    savedLine += Current;
                    Position++;
                }

                Line++;
            }

            throw new LexerException($"Char cant contains more 1 characters", startLine, startPosition, Filepath);
        }
    }
}
