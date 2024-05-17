using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class StringValue : Value
    {
        public StringValue(ExpressionInfo info, string value) : base(info)
        {
            Value = value;
        }

        public override string Name => "String";

        public override TypesType Type => TypesType.String;

        public override TypesType[] CanCastTypes { get; } = new TypesType[0];

        public string Value { get; set; }

        public override void Set(object value)
        {
            if (value is Value objectValue)
            {
                Value = objectValue.AsString();
                return;
            }

            Value = (string)value;
        }

        public override string AsString()
        {
            return Value;
        }
    }
}
