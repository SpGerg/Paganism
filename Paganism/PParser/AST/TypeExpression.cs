using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class TypeExpression : EvaluableExpression
    {
        public TypeExpression(BlockStatementExpression parent, int line, int position, string filepath, TypesType type, string structureName) : base(parent, line, position, filepath)
        {
            Value = type;
            StructureName = structureName;
            _value = new TypeValue(type, structureName);
        }

        public string StructureName { get; }

        public TypesType Value { get; }

        private readonly TypeValue _value;

        public override Value Eval(params Argument[] arguments) => _value;
    }
}
