using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class NoneExpression : EvaluableExpression
    {
        public NoneExpression(BlockStatementExpression parent, int line, int position, string filepath) : base(parent, line, position, filepath)
        {
            _value = new NoneValue();
        }

        private readonly NoneValue _value;

        public override Value Eval(params Argument[] arguments) => _value;
    }
}
