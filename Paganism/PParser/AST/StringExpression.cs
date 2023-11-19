using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class StringExpression : Expression, IEvaluable
    {
        public StringExpression(string value)
        {
            Value = value;
            _value = new StringValue(Value);
        }

        public string Value { get; }

        private readonly StringValue _value;

        public Value Eval() => _value;
    }
}
