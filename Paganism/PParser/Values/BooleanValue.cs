using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class BooleanValue : Value
    {
        public BooleanValue(bool value)
        {
            Value = value;
        }

        public override string Name => "Boolean";

        public override TypesType Type => TypesType.Boolean;

        public bool Value { get; set; }

        public override TypesType[] CanCastTypes { get; } = new[] { TypesType.Number };

        public override void Set(object value)
        {
            if (value is Value objectValue)
            {
                Value = objectValue.AsBoolean();
                return;
            }

            Value = (bool)value;
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
    }
}
