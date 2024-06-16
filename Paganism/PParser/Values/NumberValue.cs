using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;

namespace Paganism.PParser.Values
{
    public class NumberValue : Value, ISettable
    {
        public NumberValue(ExpressionInfo info, double value) : base(info)
        {
            Value = value;
        }

        public override string Name => "Number";

        public override TypesType Type => TypesType.Number;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String,
            TypesType.Boolean
        };

        public double Value { get; private set; }

        public override double AsNumber()
        {
            return Value;
        }

        public override string AsString()
        {
            return Value.ToString().Replace(',', '.');
        }

        public override bool AsBoolean()
        {
            return Value == 1;
        }

        public override bool Is(TypeValue typeValue)
        {
            return Type == typeValue.Value;
        }

        public override bool Is(Value value)
        {
            if (value is not NumberValue numberValue)
            {
                return false;
            }

            return numberValue.Value == Value;
        }

        public void Set(Value value)
        {
            Value = value.AsNumber();
        }
    }
}
