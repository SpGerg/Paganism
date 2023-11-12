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
        public ReturnExpression(Expression[] values)
        {
            Values = values;
        }

        public Expression[] Values { get; }

        public void Execute(params Argument[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}
