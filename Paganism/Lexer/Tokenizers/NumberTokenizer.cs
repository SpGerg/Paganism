using Paganism.Lexer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Lexer.Tokenizers
{
    public class NumberTokenizer : Tokenizer
    {
        public NumberTokenizer(string[] text, int position, int line)
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

            string line = string.Empty;

            while (Line < Text.Length)
            {
                while (Position < Text[Line].Length)
                {
                    if (!char.IsDigit(Current) && Current != '.')
                    {
                        if (Current == '.' && line.Contains('.'))
                        {
                            throw new Exception($"Two points in number. Line: {Line}, position: {Position}");
                        }

                        Position++; //Skip " again
                        return new Token(line, startPosition, startLine, TokenType.Number);
                    }

                    line += Current;

                    Position++;
                }

                Line++;
            }

            throw new Exception($"Number is infinity. Line: {startLine}, position: {startPosition}");
        }
    }
}
