using Paganism.Lexer.Enums;
using Paganism.Lexer.Tokenizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Lexer
{
    public class Lexer
    {
        public Lexer(string[] text)
        {
            Text = text;
        }

        public string[] Text { get; }

        public char Current => Text[Line][Position];

        public int Position { get; private set; }

        public int Line { get; private set; }

        public Token[] Run()
        {
            List<Token> tokens = new List<Token>();

            while (Line < Text.Length)
            {
                while (Position < Text[Line].Length)
                {
                    if (Current == '\"')
                    {
                        var tokenizer = new StringTokenizer(Text, Position, Line);
                        var token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        tokens.Add(token);
                    }

                    Position++;
                }

                Line++;
            }

            return tokens.ToArray();
        }
    }
}
