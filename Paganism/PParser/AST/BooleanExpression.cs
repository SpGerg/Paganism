using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class BooleanExpression : EvaluableExpression
    {
        public BooleanExpression(BlockStatementExpression parent, int line, int position, string filepath, bool value) : base(parent, line, position, filepath)
        {
            Value = value;
            _value = new BooleanValue(value);
        }

        public bool Value { get; set; }

        private readonly BooleanValue _value;

        public override Value Eval(params Argument[] arguments)
        {
            return _value;
        }
    }
}
