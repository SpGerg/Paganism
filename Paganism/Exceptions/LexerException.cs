using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Exceptions
{
    public class LexerException : PaganismException
    {
        public LexerException()
        {
        }

        public LexerException(string message) 
            : base(message, "Lexer")
        {
        }

        public LexerException(string message, int line, int position)
            : base(message, line, position, "Lexer")
        {
        }

        public override string Name => "Lexer";
    }
}
