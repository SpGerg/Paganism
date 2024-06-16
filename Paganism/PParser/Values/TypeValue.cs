using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;

namespace Paganism.PParser.Values
{
    public class TypeValue : Value, ISettable
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

        public string TypeName { get; protected set; }

        public TypesType Value { get; protected set; }

        public override string AsString()
        {
            if (this is FunctionTypeValue functionType)
            {
                return functionType.AsString();
            }

            return TypeName == string.Empty || TypeName is null ? Value.ToString() : $"{TypeName} ({Value})";
        }

        public override bool Is(TypeValue typeValue)
        {
            return (typeValue.Value is TypesType.Any || Value is TypesType.Any) ||
                    (typeValue.Value is TypesType.Object && Value is TypesType.Structure) ||
                    (typeValue.Value == Value && typeValue.TypeName == TypeName);
        }

        public override bool Is(Value value)
        {
            if (value is not TypeValue typeValue)
            {
                return false;
            }

            return Is(typeValue);
        }

        public override string ToString()
        {
            return AsString();
        }

        public void Set(Value value)
        {
            if (value is not TypeValue typeValue)
            {
                return;
            }

            Value = typeValue.Value;
            TypeName = typeValue.TypeName;
        }
    }
}
