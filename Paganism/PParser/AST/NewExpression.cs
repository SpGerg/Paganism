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
        public NewExpression(BlockStatementExpression parent, int position, int line, string filepath, string name) : base(parent, position, line, filepath)
        {
            Name = name;
        }

        public string Name { get; }

        public override Value Eval(params Argument[] arguments)
        {
            return new StructureValue(Parent, Name);
        }
    }
}
