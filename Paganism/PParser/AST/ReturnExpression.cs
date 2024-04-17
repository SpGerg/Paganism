using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class ReturnExpression : EvaluableExpression, IStatement
    {
        public ReturnExpression(ExpressionInfo info, EvaluableExpression value) : base(info)
        {
            Value = value;
        }

        public EvaluableExpression Value { get; }

        public override Value Evaluate(params Argument[] arguments)
        {
            return Value.Evaluate(arguments);
        }
    }
}
