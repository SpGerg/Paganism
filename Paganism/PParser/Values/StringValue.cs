using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;

namespace Paganism.PParser.Values
{
    public class StringValue : Value, ISettable
    {
        public StringValue(ExpressionInfo info, string value) : base(info)
        {
            Value = value;
        }

        public override string Name => "String";

        public override TypesType Type => TypesType.String;

        public override TypesType[] CanCastTypes { get; } = new TypesType[0];

        public string Value { get; private set; }

        public override string AsString()
        {
            return Value;
        }

        public override bool Is(TypeValue typeValue)
        {
            return Type == typeValue.Value;
        }

        public override bool Is(Value value)
        {
            if (value is not StringValue stringValue)
            {
                return false;
            }

            return Value == stringValue.Value;
        }

        public void Set(Value value)
        {
            Value = value.AsString();
        }
    }
}
