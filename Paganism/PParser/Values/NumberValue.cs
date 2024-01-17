using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class NumberValue : Value
    {
        public NumberValue(double value)
        {
            Value = value;
        }

        public override string Name => "Number";

        public override TypesType Type => TypesType.Number;

        public double Value { get; set; }

        public override void Set(object value)
        {
            if (value is Value objectValue)
            {
                Value = objectValue.AsNumber();
                return;
            }

            Value = (double)value;
        }

        public override double AsNumber()
        {
            return Value;
        }

        public override string AsString()
        {
            return Value.ToString();
        }
    }
}
