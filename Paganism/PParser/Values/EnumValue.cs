using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

#pragma warning disable CS0659
namespace Paganism.PParser.Values
{
    public class EnumValue : Value
    {
        public EnumValue(ExpressionInfo info, EnumMemberExpression member) : base(info)
        {
            Member = member;
        }

        public override string Name => "Enum";

        public override TypesType Type => TypesType.Enum;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.Number,
            TypesType.String
        };

        public EnumMemberExpression Member { get; }

        public override string AsString()
        {
            return $"{Member.Name}: {Member.Value.Value}";
        }

        public override double AsNumber()
        {
            return Member.Value.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is not EnumValue enumValue)
            {
                return false;
            }

            if (Member.Enum != enumValue.Member.Enum)
            {
                return false;
            }

            return true;
        }

        public override bool Is(TypeValue typeValue)
        {
            return Member.Enum == typeValue.TypeName;
        }

        public override bool Is(Value value)
        {
            if (value is NumberValue numberValue)
            {
                return Member.Value.AsNumber() == numberValue.AsNumber();
            }

            if (value is not EnumValue enumValue)
            {
                return false;
            }

            return enumValue.Member == Member;
        }
    }
}
