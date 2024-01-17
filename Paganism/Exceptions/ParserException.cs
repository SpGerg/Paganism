using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Exceptions
{
    public class ParserException : PaganismException
    {
        public ParserException()
        {
        }

        public ParserException(string message)
            : base(message,  "Parser")
        {
        }

        public ParserException(string message, int line, int position)
            : base(message, line, position, "Parser")
        {
        }

        public override string Name => "Parser";
    }
}
