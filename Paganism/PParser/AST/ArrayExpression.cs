using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class ArrayExpression : EvaluableExpression
    {
        public ArrayExpression(BlockStatementExpression parent, int line, int position, string filepath, Expression[] elements, int length) : base(parent, line, position, filepath)
        {
            Elements = elements;
            Length = length;
        }

        public Expression[] Elements { get; }

        public int Length { get; }

        public override Value Eval(params Argument[] arguments)
        {
            return new ArrayValue(Elements);
        }
    }
}
