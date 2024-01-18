using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class NoneExpression : EvaluableExpression
    {
        public NoneExpression(BlockStatementExpression parent, int line, int position, string filepath) : base(parent, line, position, filepath)
        {
            _value = new NoneValue();
        }

        private readonly NoneValue _value;

        public override Value Eval(params Argument[] arguments)
        {
            return _value;
        }
    }
}
