using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class TypeValue : Value
    {
        public TypeValue(TypesType value, string structureName)
        {
            Value = value;
            StructureName = structureName;
        }

        public override string Name => "Type";

        public override TypesType Type => TypesType.Type;

        public string StructureName { get; }

        public TypesType Value { get; }

        public override string AsString()
        {
            return Value.ToString();
        }
    }
}
