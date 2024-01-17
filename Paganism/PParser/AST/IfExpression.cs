using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class IfExpression : EvaluableExpression, IStatement, IExecutable
    {
        public IfExpression(BlockStatementExpression parent, int line, int position, string filepath, EvaluableExpression expression, BlockStatementExpression blockStatement, BlockStatementExpression elseBlockStatement) : base(parent, line, position, filepath)
        {
            Expression = expression;
            BlockStatement = blockStatement;
            ElseBlockStatement = elseBlockStatement;
        }

        public EvaluableExpression Expression { get; }

        public BlockStatementExpression BlockStatement { get; }

        public BlockStatementExpression ElseBlockStatement { get; }

        public override Value Eval(params Argument[] arguments)
        {
            bool result = Expression.Eval().AsBoolean();

            return result ? BlockStatement.ExecuteAndReturn() : ElseBlockStatement?.ExecuteAndReturn();
        }

        public void Execute(params Argument[] arguments)
        {
            bool result = Expression.Eval().AsBoolean();

            if (result)
            {
                BlockStatement.Execute();
            }

            ElseBlockStatement?.Execute();
        }
    }
}
