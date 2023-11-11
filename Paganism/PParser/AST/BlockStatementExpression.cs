using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class BlockStatementExpression : Expression, IStatement
    {
        public BlockStatementExpression(IStatement[] statements)
        {
            Statements = statements;
        }

        public IStatement[] Statements { get; }

        public void Execute(params Value[] arguments)
        {
            foreach (var statement in Statements)
            {
                statement.Execute();
            }
        }
    }
}
