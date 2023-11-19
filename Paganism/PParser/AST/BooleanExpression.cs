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
            _value = new BooleanValue(value);
        }

        public bool Value { get; set; }

        private readonly BooleanValue _value;

        public Value Eval() => _value;
    }
}
