using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class BlockStatementExpression : Expression, IStatement, IExecutable
    {
        public BlockStatementExpression(IStatement[] statements)
        {
            Statements = statements;
        }

        public IStatement[] Statements { get; }

        public void Execute(params Argument[] arguments)
        {
            foreach (var statement in Statements)
            {
                if (statement is IExecutable executable)
                {
                    executable.Execute();
                }   
            }
        }
    }
}
