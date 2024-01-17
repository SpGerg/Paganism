using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class StringExpression : EvaluableExpression
    {
        public StringExpression(BlockStatementExpression parent, int line, int position, string filepath, string value) : base(parent, line, position, filepath)
        {
            Value = value;
            _value = new StringValue(Value);
        }

        public string Value { get; }

        private readonly StringValue _value;

        public override Value Eval(params Argument[] arguments) => _value;
    }
}
