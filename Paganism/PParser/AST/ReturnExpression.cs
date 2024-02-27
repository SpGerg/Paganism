using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class ReturnExpression : EvaluableExpression, IStatement
    {
        public ReturnExpression(BlockStatementExpression parent, int line, int position, string filepath, EvaluableExpression value) : base(parent, line, position, filepath)
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
