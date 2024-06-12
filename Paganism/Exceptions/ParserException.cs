using Paganism.PParser;

namespace Paganism.Exceptions
{
    public class ParserException : PaganismException
    {
        public ParserException()
        {
        }

        protected ParserException(string message)
            : base(message, "Parser")
        {
        }

        public ParserException(string message, ExpressionInfo expressionInfo)
            : base(message, expressionInfo.Line, expressionInfo.Position, expressionInfo.Filepath, "Parser")
        {
        }

        public ParserException(string message, int line, int position, string filepath)
            : base(message, line, position, filepath, "Parser")
        {
        }

        public override string Name => "Parser";
    }
}
