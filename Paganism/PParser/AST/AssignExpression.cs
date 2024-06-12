using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;

namespace Paganism.PParser.AST
{
    public class AssignExpression : BinaryOperatorExpression, IStatement, IDeclaratable
    {
        public AssignExpression(ExpressionInfo info, EvaluableExpression left, EvaluableExpression right, bool isShow = false, bool isReadOnly = false) : base(info, OperatorType.Assign, left, right)
        {
            IsShow = isShow;
            IsReadOnly = isReadOnly;
        }

        public bool IsShow { get; }

        public bool IsReadOnly { get; }

        public void Declarate()
        {
            Assign();
        }

        public void Declarate(string name)
        {
            throw new System.NotImplementedException();
        }

        public void Remove()
        {
            throw new System.NotImplementedException();
        }
    }
}
