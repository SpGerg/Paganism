using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser
{
    public class Argument
    {
        public Argument(string name, TokenType requiredValue, bool isRequired, IEvaluable value = null)
        {
            Name = name;
            Type = requiredValue;
            IsRequired = isRequired;
            Value = value;
        }

        public string Name { get; set; }

        public IEvaluable Value { get; set; }

        public TokenType Type { get; }

        public bool IsRequired { get; }
    }
}
