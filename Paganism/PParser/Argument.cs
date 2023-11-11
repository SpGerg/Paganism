using Paganism.Lexer.Enums;
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
        public Argument(string name, TokenType requiredValue, bool isRequired)
        {
            Name = name;
            RequiredType = requiredValue;
            IsRequired = isRequired;
        }

        public string Name { get; set; }

        public Value Value { get; set; }

        public TokenType RequiredType { get; }

        public bool IsRequired { get; }

        public Argument Create(Value value)
        {
            var argument = new Argument(Name, RequiredType, IsRequired);
            argument.Value = value;

            return argument;
        }
    }
}
