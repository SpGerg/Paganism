using Paganism.Lexer.Enums;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Paganism.Lexer.Tokenizers
{
    public class OperatorTokenizer : Tokenizer
    {
        public OperatorTokenizer(string @operator, List<Token> tokens, string savedLine, int position, int line)
        {
            Operator = @operator;
            Tokens = tokens;
            SavedLine = savedLine;
            Position = position;
            Line = line;
        }

        public string Operator { get; }

        public List<Token> Tokens { get; }

        public string SavedLine { get; }

        public int Position { get; private set; }

        public int Line { get; private set; }

        public override Token Tokenize()
        {
            if (!string.IsNullOrWhiteSpace(SavedLine))
            {
                string line = Regex.Replace(SavedLine, @"\s+", "");

                if (Paganism.Lexer.Tokens.KeywordsType.TryGetValue(line, out TokenType result))
                {
                    Tokens.Add(new Token(line, Position - (SavedLine.Length - 1), Line, result));
                }
                else
                {
                    Tokens.Add(new Token(line, Position - (SavedLine.Length - 1), Line, TokenType.Word));
                }
            }

            return new Token(Operator, Position, Line, Paganism.Lexer.Tokens.OperatorsType[Operator]);
        }
    }
}
