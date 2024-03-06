using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class CharValue : Value
    {
        public CharValue(ExpressionInfo info, char value) : base(info)
        {
            Value = value;
        }

        public override string Name => "Char";

        public override TypesType Type => TypesType.Char;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.Char
        };

        public char Value { get; }

        public override string AsString()
        {
            return Value.ToString();
        }
    }
}
