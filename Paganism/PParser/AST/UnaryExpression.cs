using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class UnaryExpression : EvaluableExpression
    {
        public UnaryExpression(ExpressionInfo info, EvaluableExpression expression, BinaryOperatorType @operator) : base(info)
        {
            Expression = expression;
            Operator = @operator;
        }

        public EvaluableExpression Expression { get; }

        public BinaryOperatorType Operator { get; }

        public override Value Eval(params Argument[] arguments)
        {
            return Operator switch
            {
                BinaryOperatorType.Plus => Expression.Eval(),
                BinaryOperatorType.Minus => new NumberValue(ExpressionInfo, - Expression.Eval().AsNumber()),
                _ => Expression.Eval(),
            };
        }
    }
}
