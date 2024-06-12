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

            if (ElseBlockStatement is not null)
            {
                return ElseBlockStatement.Evaluate();
            }

            return new VoidValue(ExpressionInfo);
        }

        public void Execute(params Argument[] arguments)
        {
            Evaluate(arguments);
        }
    }
}
