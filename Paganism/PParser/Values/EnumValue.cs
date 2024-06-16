using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;

namespace Paganism.PParser.Values
{
    public class EnumValue : Value, ISettable
    {
        public EnumValue(ExpressionInfo info, EnumMemberExpression member) : base(info)
        {
            Value = member;
        }

        public override string Name => "Enum";

        public override TypesType Type => TypesType.Enum;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.Number,
            TypesType.String
        };

        public EnumMemberExpression Value { get; private set; }

        public override string AsString()
        {
            return $"{Value.Name}: {Value.Value.Value}";
        }

        public override double AsNumber()
        {
            return Value.Value.Value;
        }

        public override bool Is(TypeValue typeValue)
        {
            return Value.Enum == typeValue.TypeName;
        }

        public override bool Is(Value value)
        {
            if (value is NumberValue numberValue)
            {
                return Value.Value.AsNumber() == numberValue.AsNumber();
            }

            if (value is not EnumValue enumValue)
            {
                return false;
            }

            return enumValue.Value == Value;
        }

        public void Set(Value value)
        {
            if (value is not EnumValue enumValue)
            {
                return;
            }

            Value = enumValue.Value;
        }
    }
}
