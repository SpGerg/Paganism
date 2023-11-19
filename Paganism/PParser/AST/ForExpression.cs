using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class ForExpression : Expression, IStatement, IExecutable
    {
        public ForExpression(BlockStatementExpression statement, IEvaluable expression, BlockStatementExpression action, IStatement variable)
        {
            Expression = expression;
            Action = action;
            Variable = variable;
            Statement = statement;
        }

        public IEvaluable Expression { get; }

        public BlockStatementExpression Action { get; }

        public IStatement Variable { get; }

        public BlockStatementExpression Statement { get; }

        public void Execute(params Argument[] arguments)
        {
            while (Expression.Eval().AsBoolean())
            {
                Statement.Execute();

                if (Statement.IsBreaked) break;

                Action.Execute();
            }
        }
    }
}
