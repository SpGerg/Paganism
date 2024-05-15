using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;

namespace Paganism.PParser.AST
{
    public class AssignExpression : BinaryOperatorExpression, IStatement
    {
        public AssignExpression(ExpressionInfo info, EvaluableExpression left, EvaluableExpression right, bool isShow = false) : base(info, OperatorType.Assign, left, right)
        {
            IsShow = isShow;
        }

        public bool IsShow { get; }
    }
}
