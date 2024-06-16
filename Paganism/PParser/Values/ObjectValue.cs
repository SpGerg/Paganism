using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;

namespace Paganism.PParser.Values
{
    public class ObjectValue : Value, ISettable
    {
        public ObjectValue(ExpressionInfo info, StructureValue value) : base(info)
        {
            Value = value;
        }

        public override string Name => "Object";

        public override TypesType Type => TypesType.Object;

        public override TypesType[] CanCastTypes { get; } = new TypesType[]
        {
            TypesType.Structure,
            TypesType.String
        };

        public StructureValue Value { get; private set; }

        public override string AsString() => Value.AsString();

        public override bool Is(TypeValue typeValue)
        {
            return typeValue.Type is TypesType.Structure or TypesType.Object;
        }

        public override bool Is(Value value)
        {
            return value is StructureValue;
        }

        public void Set(Value value)
        {
            if (value is StructureValue structureValue)
            {
                Value = structureValue;
            }
            else if (value is ObjectValue objectValue)
            {
                Value = objectValue.Value;
            }
        }
    }
}
