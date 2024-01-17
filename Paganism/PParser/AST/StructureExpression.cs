namespace Paganism.PParser.AST
{
    public class StructureExpression : Expression
    {
        public StructureExpression(BlockStatementExpression parent, int line, int position, string filepath, string value) : base(parent, line, position, filepath)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
