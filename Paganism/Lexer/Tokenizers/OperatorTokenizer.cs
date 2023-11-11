using Paganism.Lexer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public int Position { get; private set; }

        public int Line { get; private set; }

        public override Token Tokenize()
        {
            if (!string.IsNullOrWhiteSpace(SavedLine))
            {
                Tokens.Add(new Token(Regex.Replace(SavedLine, @"\s+", ""), Position - (SavedLine.Length - 1), Line, TokenType.Word));
            }

            return new Token(Operator, Position, Line, Paganism.Lexer.Tokens.OperatorsType[Operator]);
        }
    }
}
