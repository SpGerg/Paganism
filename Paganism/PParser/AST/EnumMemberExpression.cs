using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class EnumMemberExpression : Expression
    {
        public EnumMemberExpression(BlockStatementExpression parent, int position, int line, string filepath, string name, NumberExpression value, string enumParent) : base(parent, position, line, filepath)
        {
            Name = name;
            Value = value;
            Enum = enumParent;
        }

        public string Name { get; }

        public NumberExpression Value { get; }

        public string Enum { get; }
    }
}
