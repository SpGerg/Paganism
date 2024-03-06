namespace Paganism.PParser.AST
{
    public class StructureExpression : Expression
    {
        public StructureExpression(ExpressionInfo info, string value) : base(info)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
