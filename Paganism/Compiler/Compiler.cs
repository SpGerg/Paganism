using Paganism.PParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Compiler
{
    public class Compiler
    {
        public Compiler(BlockStatementExpression blockStatement)
        {
            BlockStatement = blockStatement;
        }

        public BlockStatementExpression BlockStatement { get; }

        public void Run()
        {
            foreach (var statement in BlockStatement.Statements)
            {
                if (statement == null) continue;

                if (statement is FunctionCallExpression callExpression)
                {
                    callExpression.Execute(callExpression.Arguments);
                }
            }
        }
    }
}
