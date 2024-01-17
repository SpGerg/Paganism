using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class CharExpression : EvaluableExpression
    {
        public CharExpression(BlockStatementExpression parent, int line, int position, string filepath, char value) : base(parent, line, position, filepath)
        {
            Value = value;
            _value = new CharValue(Value);
        }

        public char Value { get; }

        private readonly CharValue _value;

        public override Value Eval(params Argument[] arguments)
        {
            return _value;
        }
    }
}
