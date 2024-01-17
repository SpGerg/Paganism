using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var result = Expression.Eval().AsBoolean();

            if (result)
            {
                return BlockStatement.ExecuteAndReturn();
            }
            
            if (ElseBlockStatement != null)
            {
                return ElseBlockStatement.ExecuteAndReturn();
            }

            return null;
        }

        public void Execute(params Argument[] arguments)
        {
            var result = Expression.Eval().AsBoolean();

            if (result)
            {
                BlockStatement.Execute();
            }

            if (ElseBlockStatement != null)
            {
                ElseBlockStatement.Execute();
            }
        }
    }
}
