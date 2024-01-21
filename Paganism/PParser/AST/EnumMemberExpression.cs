using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class EnumMemberExpression : Expression
    {
        public EnumMemberExpression(BlockStatementExpression parent, int position, int line, string filepath, string name, NumberExpression value) : base(parent, position, line, filepath)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public NumberExpression Value { get; }
    }
}
