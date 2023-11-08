using Paganism.Lexer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int Position { get; }

        public int Line { get; }

        public override Token Tokenize()
        {
            if (Operator != ")" && !string.IsNullOrWhiteSpace(SavedLine))
            {
                Tokens.Add(new Token(SavedLine, Position - (SavedLine.Length - 1), Line, TokenType.Word));
            }

            return new Token(Operator, Position, Line, Paganism.Lexer.Tokens.OperatorsType[Operator]);
        }
    }
}
