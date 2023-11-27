using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message, int line, int position)
            : base(message + $" Line: {line+1}, position: {position+1}")
        {
        }
    }
}
