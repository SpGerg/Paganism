using Paganism.Lexer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Lexer.Tokenizers
{
    public class KeywordTokenizer : Tokenizer
    {
        public KeywordTokenizer(List<Token> tokens, TokenType previousTokenType, string[] text, int position, int line)
        {
            Tokens = tokens;
            Text = text;
            PreviousTokenType = previousTokenType;
            Position = position;
            Line = line;
        }

        public List<Token> Tokens { get; }

        public string[] Text { get; }

        public TokenType PreviousTokenType { get; }

        public char Current => Text[Line][Position];

        public int Position { get; private set; }

        public int Line { get; private set; }

        public override Token Tokenize()
        {
            int startPosition = Position;
            int startLine = Line;

            string savedLine = string.Empty;

            if (Current == ' ') Position++;

            Tokens.Add(new Token(Paganism.Lexer.Tokens.KeywordsType.FirstOrDefault(keyword => keyword.Value == PreviousTokenType).Key, Position - (savedLine.Length - 1), Line, PreviousTokenType));

            if (PreviousTokenType == TokenType.Function)
            {
                while (Position < Text[Line].Length)
                {
                    if (Current == '(')
                    {
                        if (string.IsNullOrWhiteSpace(savedLine)) break;

                        Tokens.Add(new Token(savedLine, startPosition, startLine, TokenType.Word));
                        Tokens.Add(new Token(Current.ToString(), Position, Line, TokenType.LeftPar));
                        savedLine = string.Empty;
                        break;
                    }

                    if (Current == ' ')
                    {
                        throw new Exception($"Contains space in function name. Line: {startLine}, position: {startPosition}");
                    }

                    savedLine += Current;
                    Position++;
                }

                while (Position < Text[Line].Length)
                {
                    if (Current == ',' || Current == ')')
                    {
                        Tokens.Add(new Token(savedLine, Position, Line, TokenType.Word));
                        Tokens.Add(new Token(Current.ToString(), Position, Line, TokenType.Comma));

                        Position++;

                        savedLine = string.Empty;
                    }

                    savedLine += Current;
                    Position++;
                }
            }

            return null;
        }
    }
}
