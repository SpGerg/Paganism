using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Exceptions
{
    public class InterpreterException : Exception
    {
        public InterpreterException()
        {
        }

        public InterpreterException(string message)
            : base(message)
        {
        }
    }
}
