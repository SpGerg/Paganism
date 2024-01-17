using Paganism.Lexer.Enums;
using Paganism.Lexer.Tokenizers;
using System.Collections.Generic;

namespace Paganism.Lexer
{
    public class Lexer
    {
        public Lexer(string[] text)
        {
            string[] result = new string[text.Length + 1];

            for (int i = 0; i < text.Length; i++)
            {
                result[i] = text[i] += "\n";
            }

            result[result.Length - 1] = string.Empty;

            Text = result;
        }

        public string[] Text { get; }

        public char Current => Text[Line][Position];

        public int Position { get; private set; }

        public int Line { get; private set; }

        public Token[] Run()
        {
            List<Token> tokens = new();

            string savedLine = string.Empty;

            while (Line < Text.Length)
            {
                while (Position < Text[Line].Length)
                {
                    if (Current == '\t')
                    {
                        Position++;
                        continue;
                    }

                    if (Tokens.KeywordsType.TryGetValue(savedLine, out TokenType tokenType))
                    {
                        //It is check need because variable name can like 'format' and we check next symbol, if it is space or operator we add keyword to tokens
                        if (Current == '\0' || Current == ' ' || Current == '\n' || Tokens.OperatorsType.ContainsKey(Current.ToString()))
                        {
                            tokens.Add(new Token(savedLine, Position, Line, tokenType));
                            savedLine = string.Empty;
                            continue;
                        }

                        savedLine += Current;
                        Position++;
                        continue;
                    }
                    else if (Tokens.OperatorsType.ContainsKey(Current.ToString()))
                    {
                        OperatorTokenizer tokenizer = new(Current.ToString(), tokens, savedLine, Position, Line);
                        Token token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);

                        Position++;
                        continue;
                    }
                    else if (Current == '\"')
                    {
                        StringTokenizer tokenizer = new(Text, Position, Line);
                        Token token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);

                        continue;
                    }
                    else if (Current == '\'')
                    {
                        CharTokenizer tokenizer = new(Text, Position, Line);
                        Token token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);

                        continue;
                    }

                    else if (char.IsDigit(Current) && (string.IsNullOrEmpty(savedLine) || string.IsNullOrWhiteSpace(savedLine)))
                    {
                        NumberTokenizer tokenizer = new(Text, Position, Line);
                        Token token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);

                        Position++;
                        continue;
                    }
                    else if (Current == ' ')
                    {
                        if (string.IsNullOrEmpty(savedLine) || string.IsNullOrWhiteSpace(savedLine))
                        {
                            Position++;

                            continue;
                        }

                        string replacedLine = savedLine.Replace(" ", string.Empty);

                        if (Tokens.KeywordsType.ContainsKey(replacedLine))
                        {
                            savedLine = replacedLine;
                            continue;
                        }

                        tokens.Add(new Token(replacedLine, Position, Line, TokenType.Word));
                        savedLine = string.Empty;
                    }

                    savedLine += Current;
                    Position++;
                }

                if (!string.IsNullOrEmpty(savedLine) && !string.IsNullOrWhiteSpace(savedLine))
                {
                    tokens.Add(new Token(savedLine.Replace("\n", string.Empty).Replace(" ", string.Empty), Position, Line, TokenType.Word));
                }

                savedLine = string.Empty;
                Position = 0;
                Line++;
            }

            return tokens.ToArray();
        }
    }
}
