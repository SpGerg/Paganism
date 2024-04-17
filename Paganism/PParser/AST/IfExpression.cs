using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class IfExpression : EvaluableExpression, IStatement, IExecutable
    {
        public IfExpression(ExpressionInfo info, EvaluableExpression expression, BlockStatementExpression blockStatement, BlockStatementExpression elseBlockStatement) : base(info)
        {
            Expression = expression;
            BlockStatement = blockStatement;
            ElseBlockStatement = elseBlockStatement;
        }

        public EvaluableExpression Expression { get; }

        public BlockStatementExpression BlockStatement { get; }

        public BlockStatementExpression ElseBlockStatement { get; }

        public override Value Evaluate(params Argument[] arguments)
        {
            var result = Expression.Evaluate().AsBoolean();

            if (result)
            {
                return BlockStatement.Evaluate();
            }

            if (ElseBlockStatement != null)
            {
                return ElseBlockStatement.Evaluate();
            }

            return null;
        }

        public void Execute(params Argument[] arguments)
        {
            var result = Expression.Evaluate().AsBoolean();

            if (result)
            {
                BlockStatement.Execute();
            }

            if (ElseBlockStatement != null)
            {
                ElseBlockStatement.Execute();
            }
        }
    }
}
