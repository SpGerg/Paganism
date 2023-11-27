using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class CharExpression : Expression, IEvaluable
    {
        public CharExpression(char value)
        {
            Value = value;
            _value = new CharValue(Value);
        }

        public char Value { get; }

        private readonly CharValue _value;

        public Value Eval() => _value;
    }
}
