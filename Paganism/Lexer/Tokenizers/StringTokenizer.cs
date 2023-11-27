using Paganism.Exceptions;
using Paganism.Lexer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Lexer.Tokenizers
{
    public class StringTokenizer : Tokenizer
    {
        public StringTokenizer(string[] text, int position, int line)
        {
            Text = text;
            Position = position;
            Line = line;
        }

        public string[] Text { get; }

        public char Current => Text[Line][Position];

        public int Position { get; private set; }

        public int Line { get; private set; }

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

            throw new LexerException($"String is not ended", startLine, startPosition);
        }
    }
}
