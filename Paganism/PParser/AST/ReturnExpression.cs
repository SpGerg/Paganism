using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class ReturnExpression : Expression, IStatement
    {
        public ReturnExpression(BlockStatementExpression parent, int line, int position, string filepath, Expression[] values) : base(parent, line, position, filepath)
        {
            Values = values;
        }

        public Expression[] Values { get; }
    }
}
