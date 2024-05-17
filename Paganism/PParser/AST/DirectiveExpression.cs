using Paganism.PParser.AST.Interfaces;

namespace Paganism.PParser.AST
{
    public class DirectiveExpression : Expression, IStatement
    {
        public DirectiveExpression(ExpressionInfo info) : base(info) { }
    }
}
