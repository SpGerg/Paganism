using Paganism.PParser.AST.Enums;

#pragma warning disable CS0659
namespace Paganism.PParser.Values
{
    public class TypeValue : Value
    {
        public TypeValue(ExpressionInfo info, TypesType value, string typeName) : base(info)
        {
            Value = value;
            TypeName = typeName is null ? string.Empty : typeName;
        }

        public override string Name => "Type";

        public override TypesType Type => TypesType.Type;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String
        };

        public string TypeName { get; }

        public TypesType Value { get; }

        public override string AsString()
        {
            if (this is FunctionTypeValue functionType)
            {
                return functionType.AsString();
            }

            return TypeName == string.Empty || TypeName is null ? Value.ToString() : $"{TypeName} ({Value})";
        }

        public override string ToString()
        {
            return AsString();
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
