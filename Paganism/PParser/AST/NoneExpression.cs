using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class NoneExpression : Expression, IEvaluable
    {
        public NoneExpression()
        {
            _value = new NoneValue();
        }

        private readonly NoneValue _value;

        public Value Eval() => _value;
    }
}
