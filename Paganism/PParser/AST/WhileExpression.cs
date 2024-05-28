using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class WhileExpression : Expression, IStatement
    {
        public WhileExpression(ExpressionInfo info, BlockStatementExpression blockStatementExpression, BlockStatementExpression elseExpression, EvaluableExpression expression) : base(info)
        {
            BlockStatementExpression = blockStatementExpression;
            ElseExpression = elseExpression;
            Expression = expression;
        }

        public BlockStatementExpression BlockStatementExpression { get; } 

        public BlockStatementExpression ElseExpression { get; }

        public EvaluableExpression Expression { get; }

        public void Execute()
        {
            while (Expression is null || Expression.Evaluate().AsBoolean())
            {
                BlockStatementExpression.Evaluate();
            }
            
            ElseExpression?.Execute();
        }
    }
}
