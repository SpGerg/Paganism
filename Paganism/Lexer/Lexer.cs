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

            var savedLine = string.Empty;

            while (Line < Text.Length)
            {
                while (Position < Text[Line].Length)
                {
                    savedLine += Current;

                    if (Current == '\"')
                    {
                        var tokenizer = new StringTokenizer(Text, Position, Line);
                        var token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);
                    }
                    else if (char.IsDigit(Current))
                    {
                        var tokenizer = new NumberTokenizer(Text, Position, Line);
                        var token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);
                    }
                    else if (Tokens.OperatorsType.ContainsKey(Current.ToString()))
                    {
                        var tokenizer = new OperatorTokenizer(Current.ToString(), tokens, savedLine.Remove(savedLine.Length - 1), Position, Line);
                        var token = tokenizer.Tokenize();

                        savedLine = string.Empty;
                        tokens.Add(token);
                    }

                    Position++;
                }

                savedLine = string.Empty;
                Position = 0;
                Line++;
            }

            return tokens.ToArray();
        }
    }
}
