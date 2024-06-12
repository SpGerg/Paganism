using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class ForExpression : LoopExpression, IStatement
    {
        public ForExpression(ExpressionInfo info, BlockStatementExpression blockStatementExpression, BlockStatementExpression elseExpression, BlockStatementExpression action, EvaluableExpression expression, IStatement variable) : base(info, blockStatementExpression, elseExpression)
        {
            Variable = variable;
            Expression = expression;
            Executable = action;
        }

        public IStatement Variable { get; }

        public override IExecutable Executable { get; }

        public EvaluableExpression Expression { get; }

        public override bool IsContinue() => Expression is null || Expression.Evaluate().AsBoolean();

        public override Value Execute(params Argument[] arguments)
        {
            var variable = Variable as AssignExpression;

            VariableExpression variableExpression = null;

            if (variable is not null)
            {
                variableExpression = variable.Left as VariableExpression;

                variableExpression.Set(ExpressionInfo, variable.Right.Evaluate());
            }

            base.Execute(arguments);

            variableExpression?.Remove();

            return new VoidValue(ExpressionInfo);
        }
    }
}
