using Paganism.PParser.AST.Interfaces;

namespace Paganism.PParser.AST
{
    public class ReturnExpression : Expression, IStatement
    {
        public ReturnExpression(BlockStatementExpression parent, int line, int position, string filepath, EvaluableExpression value) : base(parent, line, position, filepath)
        {
            Value = value;
        }

        public EvaluableExpression Value { get; }
    }
}
