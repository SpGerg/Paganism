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
            BlockStatement.Execute();
        }
    }
}
