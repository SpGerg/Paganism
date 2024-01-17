using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class BreakExpression : Expression, IStatement, IExecutable
    {
        public BreakExpression(BlockStatementExpression parent, int position, int line, string filepath) : base(parent, line, position, filepath) { }

        public bool IsBreaked { get; private set; }

        public bool IsLoop { get; set; }

        public void Execute(params Argument[] arguments)
        {
            if (IsLoop)
            {
                IsBreaked = true;
            }
        }
    }
}
