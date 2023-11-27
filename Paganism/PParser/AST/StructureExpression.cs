using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class StructureExpression : Expression
    {
        public StructureExpression(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
