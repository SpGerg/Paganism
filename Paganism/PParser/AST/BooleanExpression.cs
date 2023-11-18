using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class BooleanExpression : Expression, IEvaluable
    {
        public BooleanExpression(bool value)
        {
            Value = value;
        }

        public bool Value { get; set; }

        public Value Eval()
        {
            return new BooleanValue(Value);
        }
    }
}
