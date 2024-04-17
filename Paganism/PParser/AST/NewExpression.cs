using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class NewExpression : EvaluableExpression
    {
        public NewExpression(ExpressionInfo info, string name) : base(info)
        {
            Name = name;
        }

        public string Name { get; }

        public override Value Evaluate(params Argument[] arguments)
        {
            return new StructureValue(ExpressionInfo, ExpressionInfo.Parent, Name);
        }
    }
}
