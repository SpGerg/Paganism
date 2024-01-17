using Paganism.PParser.AST.Interfaces;

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
