using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class BooleanExpression : EvaluableExpression
    {
        public BooleanExpression(BlockStatementExpression parent, int line, int position, string filepath, bool value) : base(parent, line, position, filepath)
        {
            Value = value;
            _value = new BooleanValue(value);
        }

        public bool Value { get; set; }

        private readonly BooleanValue _value;

        public override Value Eval(params Argument[] arguments) => _value;
    }
}
