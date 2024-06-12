using Paganism.PParser;

namespace Paganism.Exceptions
{
    public class LexerException : PaganismException
    {
        public LexerException()
        {
        }

        protected LexerException(string message)
            : base(message, "Lexer")
        {
        }

        public LexerException(string message, ExpressionInfo expressionInfo)
            : base(message, expressionInfo.Line, expressionInfo.Position, expressionInfo.Filepath, "Lexer")
        {
        }

        public LexerException(string message, int line, int position, string filepath)
            : base(message, line, position, filepath, "Lexer")
        {
        }

        public override string Name => "Lexer";
    }
}
