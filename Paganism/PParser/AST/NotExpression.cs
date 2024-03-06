using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class NotExpression : EvaluableExpression
    {
        public EvaluableExpression Expression { get; }

        public NotExpression(ExpressionInfo info, EvaluableExpression expression) : base(info)
        {
            Expression = expression;
        }

        public override Value Eval(params Argument[] arguments)
        {
            return new BooleanValue(ExpressionInfo, !Expression.Eval().AsBoolean());
        }
    }
}
