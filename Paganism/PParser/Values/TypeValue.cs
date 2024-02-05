using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class TypeValue : Value
    {
        public TypeValue(TypesType value, string typeName)
        {
            Value = value;
            TypeName = typeName;
        }

        public override string Name => "Type";

        public override TypesType Type => TypesType.Type;

        public string TypeName { get; }

        public TypesType Value { get; }

        public override string AsString()
        {
            return TypeName == string.Empty || TypeName is null ? Value.ToString() : $"{TypeName} ({Value})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not TypeValue typeValue)
            {
                return false;
            }

            if (typeValue.Value != Value)
            {
                return false;
            }

            if (typeValue.TypeName != TypeName)
            {
                return false;
            }

            return true;
        }
    }
}
