using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;

namespace Paganism.PParser.Values
{
    public class BooleanValue : Value, ISettable
    {
        public BooleanValue(ExpressionInfo info, bool value) : base(info)
        {
            Value = value;
        }

        public override string Name => "Boolean";

        public override TypesType Type => TypesType.Boolean;

        public bool Value { get; private set; }

        public override TypesType[] CanCastTypes { get; } = new[] { TypesType.Number };

        public void Set(Value value)
        {
            Value = value.AsBoolean();
        }

        public override double AsNumber()
        {
            return Value ? 1 : 0;
        }

        public override string AsString()
        {
            return Value.ToString();
        }

        public override bool AsBoolean()
        {
            return Value;
        }

        public override bool Is(TypeValue typeValue)
        {
            return Type == typeValue.Value;
        }

        public override bool Is(Value value)
        {
            if (value is not BooleanValue booleanValue)
            {
                return false;
            }

            return booleanValue.Value == Value;
        }
    }
}
