using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class VoidValue : Value
    {
        public VoidValue(ExpressionInfo info) : base(info)
        {
        }

        public override string Name => "Void";

        public override TypesType Type => TypesType.Void;

        public override bool Equals(object obj)
        {
            return obj is null;
        }
    }
}
