using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public abstract class LoopExpression : EvaluableExpression
    {
        public LoopExpression(ExpressionInfo info, BlockStatementExpression blockStatementExpression, BlockStatementExpression elseExpression) : base(info)
        {
            BlockStatementExpression = blockStatementExpression;
            ElseExpression = elseExpression;
        }

        public BlockStatementExpression BlockStatementExpression { get; }

        public BlockStatementExpression ElseExpression { get; }

        public virtual IExecutable Executable { get; }

        public abstract bool IsContinue();

        public virtual Value Execute(params Argument[] arguments)
        {
            for (;IsContinue();Executable?.Execute(arguments))
            {
                var result = BlockStatementExpression.Evaluate();

                if (result is not VoidValue)
                {
                    return result;
                }

                if (BlockStatementExpression.IsBreaked)
                {
                    return new VoidValue(ExpressionInfo);
                }
            }

            return ElseExpression?.Evaluate();
        }

        public override Value Evaluate(params Argument[] arguments) => Execute(arguments);
    }
}
