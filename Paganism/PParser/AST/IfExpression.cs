using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class IfExpression : Expression, IStatement, IExecutable
    {
        public IfExpression(IEvaluable expression, BlockStatementExpression blockStatement, BlockStatementExpression elseBlockStatement)
        {
            Expression = expression;
            BlockStatement = blockStatement;
            ElseBlockStatement = elseBlockStatement;
        }

        public IEvaluable Expression { get; }

        public BlockStatementExpression BlockStatement { get; }

        public BlockStatementExpression ElseBlockStatement { get; }

        public void Execute(params Argument[] arguments)
        {
            var result = Expression.Eval().AsBoolean();

            if (result)
            {
                BlockStatement.Execute(arguments);
            }
            else
            {
                ElseBlockStatement.Execute(arguments);
            }
        }
    }
}
