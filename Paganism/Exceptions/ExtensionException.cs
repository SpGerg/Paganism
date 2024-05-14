using Paganism.PParser;

namespace Paganism.Exceptions
{
    internal class ExtensionException : PaganismException
    {
        public ExtensionException()
        {
        }

        public ExtensionException(string message)
            : base(message, "Extension")
        {
        }

        public ExtensionException(string message, ExpressionInfo expressionInfo)
            : base(message, expressionInfo.Line, expressionInfo.Position, expressionInfo.Filepath, "Extension")
        {
        }

        public ExtensionException(string message, int line, int position, string filepath)
            : base(message, line, position, filepath, "Extension")
        {
        }

        public override string Name => "Extension";
    }
}
