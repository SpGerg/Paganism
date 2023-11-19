using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class NumberExpression : Expression, IEvaluable
    {
        public NumberExpression(double value)
        {
            Value = value;
            _value = new NumberValue(value);
        }

        public double Value { get; set; }

        private readonly NumberValue _value;

        public Value Eval() => _value;
    }
}
