using Paganism.PParser.AST;

namespace Paganism.PParser
{
    public readonly struct ExpressionInfo
    {
        public ExpressionInfo(BlockStatementExpression parent, int line, int position, string filepath)
        {
            Parent = parent;
            Line = line;
            Position = position;
            Filepath = filepath;
        }

        public ExpressionInfo()
        {
            Parent = null;
            Line = -1;
            Position = -1;
            Filepath = string.Empty;
        }

        public static ExpressionInfo EmptyInfo { get; } = new ExpressionInfo();

        public BlockStatementExpression Parent { get; }

        public int Line { get; }

        public int Position { get; }

        public string Filepath { get; }
    }
}
