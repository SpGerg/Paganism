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

        public override Value Eval(params Argument[] arguments)
        {
            return Value.Eval(arguments);
        }
    }
}
