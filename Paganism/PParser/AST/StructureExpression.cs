using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class StructureExpression : Expression, IEvaluable
    {
        public StructureExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public Value Eval()
        {
            return new StructureValue(Structures.Get(Name));
        }
    }
}
