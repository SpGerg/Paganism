using System;

namespace Paganism.Exceptions
{
    public abstract class PaganismException : Exception
    {
        public abstract string Name { get; }

        public string OriginalMessage { get; }

        public PaganismException()
        {
        }

        protected PaganismException(string message, string name)
            : base($"{name} error: {message} ")
        {
            OriginalMessage = message;
        }


        public PaganismException(string message, int line, int position, string filepath, string name)
            : base($"{filepath}, {name} error: {message + (message.EndsWith(".") ? string.Empty : '.')} Line: {line + 1}, position: {position + 1}")
        {
            OriginalMessage = message;
        }
    }
}
