using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class ObjectValue : Value
    {
        public ObjectValue(ExpressionInfo info, StructureValue value) : base(info)
        {
            Value = value;
        }

        public override string Name => "Object";

        public override TypesType Type => TypesType.Object;

        public StructureValue Value { get; set; }

        public override TypesType[] CanCastTypes { get; } = new TypesType[]
        {
            TypesType.Structure,
            TypesType.String
        };

        public override string AsString() => Value.AsString();

        public override bool Is(TypeValue typeValue)
        {
            return typeValue.Type is TypesType.Structure or TypesType.Object;
        }

        public override bool Is(Value value)
        {
            return value is StructureValue;
        }
    }
}
