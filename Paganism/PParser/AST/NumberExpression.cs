using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class NumberExpression : EvaluableExpression
    {
        public NumberExpression(BlockStatementExpression parent, int line, int position, string filepath, double value) : base(parent, line, position, filepath)
        {
            Value = value;
            _value = new NumberValue(value);
        }

        public double Value { get; set; }

        private readonly NumberValue _value;

        public override Value Eval(params Argument[] arguments) => _value;
    }
}
