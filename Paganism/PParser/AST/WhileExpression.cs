using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class WhileExpression : LoopExpression, IStatement
    {
        public WhileExpression(ExpressionInfo info, BlockStatementExpression blockStatementExpression, BlockStatementExpression elseExpression, EvaluableExpression expression) : base(info, blockStatementExpression, elseExpression)
        {
            Expression = expression;
        }

        public EvaluableExpression Expression { get; }

        public override bool IsContinue() => Expression is null || Expression.Evaluate().AsBoolean();
    }
}
