using Paganism.Exceptions;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public abstract class Expression
    {
        protected Expression(BlockStatementExpression parent, int position, int line, string filepath)
        {
            Parent = parent;
            Position = position;
            Line = line;
            Filepath = filepath;
        }

        public BlockStatementExpression Parent { get; protected set; }

        public int Position { get; protected set; }

        public int Line { get; protected set; }

        public string Filepath { get; protected set; }
    }
}
