using Paganism.PParser;

namespace Paganism.Exceptions
{
    public class InterpreterException : PaganismException
    {
        public InterpreterException()
        {
        }

        protected InterpreterException(string message)
            : base(message, "Interpreter")
        {
        }


        public InterpreterException(string message, ExpressionInfo expressionInfo)
            : base(message, expressionInfo.Line, expressionInfo.Position, expressionInfo.Filepath, "Interpreter")
        {
        }

        public InterpreterException(string message, int line, int position, string filepath)
            : base(message, line, position, filepath, "Interpreter")
        {
        }

        public override string Name => "Interpreter";
    }
}
