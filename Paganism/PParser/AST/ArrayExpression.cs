using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class ArrayExpression : EvaluableExpression
    {
        public ArrayExpression(ExpressionInfo info, Expression[] elements, int length) : base(info)
        {
            Elements = elements;
            Length = length;
        }

        public Expression[] Elements { get; }

        public int Length { get; }

        public override Value Eval(params Argument[] arguments)
        {
            return new ArrayValue(ExpressionInfo, Elements);
        }
    }
}
