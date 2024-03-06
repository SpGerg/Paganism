using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class DirectiveExpression : Expression, IStatement
    {
        public DirectiveExpression(ExpressionInfo info) : base(info) { }
    }
}
