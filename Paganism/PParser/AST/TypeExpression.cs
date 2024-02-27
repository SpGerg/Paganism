using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class TypeExpression : EvaluableExpression
    {
        public TypeExpression(BlockStatementExpression parent, int line, int position, string filepath, TypesType type, string structureName) : base(parent, line, position, filepath)
        {
            Value = type;
            TypeName = structureName;
            _value = new TypeValue(type, structureName);
        }

        public string TypeName { get; }

        public TypesType Value { get; }

        private readonly TypeValue _value;

        public override Value Eval(params Argument[] arguments)
        {
            return _value;
        }
    }
}
