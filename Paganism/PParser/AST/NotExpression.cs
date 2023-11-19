using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class NotExpression : Expression, IEvaluable
    {
        public IEvaluable Expression { get; }

        public NotExpression(IEvaluable expression)
        {
            Expression = expression;
        }

        public Value Eval()
        {
            return new BooleanValue(!Expression.Eval().AsBoolean());
        }
    }
}
