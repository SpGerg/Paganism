using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class ForExpression : EvaluableExpression, IStatement, IExecutable
    {
        public ForExpression(BlockStatementExpression parent, int line, int position, string filepath, BlockStatementExpression statement, EvaluableExpression expression, BlockStatementExpression action, IStatement variable) : base(parent, line, position, filepath)
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

        public override Value Eval(params Argument[] arguments)
        {
            while (Expression == null || Expression.Eval().AsBoolean())
            {
                Value result = Statement.ExecuteAndReturn();

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

            return new NoneValue();
        }

        public void Execute(params Argument[] arguments)
        {
            _ = Eval();
        }
    }
}
