namespace Paganism.Exceptions
{
    public class InterpreterException : PaganismException
    {
        public InterpreterException()
        {
        }
        public InterpreterException(string message)
            : base(message, "Interpreter")
        {
        }


        public InterpreterException(string message, int line, int position)
            : base(message, line, position, "Interpreter")
        {
        }

        public override string Name => "Interpreter";
    }
}
