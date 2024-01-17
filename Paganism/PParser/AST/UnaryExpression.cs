using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class UnaryExpression : EvaluableExpression
    {
        public UnaryExpression(BlockStatementExpression parent, int line, int position, string filepath, EvaluableExpression expression, BinaryOperatorType @operator) : base(parent, line, position, filepath)
        {
            Expression = expression;
            Operator = @operator;
        }

        public EvaluableExpression Expression { get; }

        public BinaryOperatorType Operator { get; }

        public override Value Eval(params Argument[] arguments)
        {
            switch (Operator)
            {
                case BinaryOperatorType.Plus:
                    return Expression.Eval();
                case BinaryOperatorType.Minus:
                    return new NumberValue(-Expression.Eval().AsNumber());
            }

            return Expression.Eval();
        }
    }
}
