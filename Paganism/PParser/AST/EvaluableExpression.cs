using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public abstract class EvaluableExpression : Expression
    {
        protected EvaluableExpression(BlockStatementExpression parent, int position, int line, string filepath) : base(parent, position, line, filepath)
        {
        }

        public abstract Value Eval(params Argument[] arguments);
    }
}
