using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class BooleanExpression : Expression
    {
        public BooleanExpression(bool value)
        {
            Value = value;
        }

        public bool Value { get; set; }
    }
}
