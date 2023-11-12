using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class StringExpression : Expression
    {
        public StringExpression(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
