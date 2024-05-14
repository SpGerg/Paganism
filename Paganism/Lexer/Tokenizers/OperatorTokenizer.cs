using Paganism.Lexer.Enums;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Paganism.Lexer.Tokenizers
{
    public class OperatorTokenizer : Tokenizer
    {
        public OperatorTokenizer(string @operator, List<Token> tokens, string savedLine, Lexer lexer) : base(lexer)
        {
            Operator = @operator;
            Tokens = tokens;
            SavedLine = savedLine;
        }

        public string Operator { get; }

        public List<Token> Tokens { get; }

        public string SavedLine { get; }

        public override Token Tokenize()
        {
            if (!string.IsNullOrWhiteSpace(SavedLine))
            {
                var line = Regex.Replace(SavedLine, @"\s+", "");

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
