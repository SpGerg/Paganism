using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    internal class DirectiveExpression : Expression, IStatement
    {
        public DirectiveExpression(BlockStatementExpression parent, int position, int line, string filepath) : base(parent, position, line, filepath)
        {
            Parent = parent;
            Position = position;
            Line = line;
            Filepath = filepath;
        }
    }
}
