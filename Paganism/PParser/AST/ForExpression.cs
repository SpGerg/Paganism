using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class ForExpression : EvaluableExpression, IStatement, IExecutable
    {
        public ForExpression(ExpressionInfo info, BlockStatementExpression statement, EvaluableExpression expression, BlockStatementExpression action, IStatement variable) : base(info)
        {
            Expression = expression;
            Action = action;
            Variable = variable;
            Statement = statement;
        }

        public EvaluableExpression Expression { get; }

        public BlockStatementExpression Action { get; }

        public IStatement Variable { get; }

        public BlockStatementExpression Statement { get; }

        public override Value Evaluate(params Argument[] arguments)
        {
            while (Expression == null || Expression.Evaluate().AsBoolean())
            {
                var result = Statement.Evaluate();

                if (result != null)
                {
                    return result;
                }

                if (Statement.IsBreaked)
                {
                    break;
                }

                Action.Execute();
            }

            return new VoidValue(ExpressionInfo);
        }

        public void Execute(params Argument[] arguments)
        {
            Evaluate();
        }
    }
}
