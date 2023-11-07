using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Lexer.Enums
{
    public enum TokenType
    {
        None = 0,
        String = 1,
        Number = 2,
        Boolean = 3,
        Colon = 4,
        Semicolon = 5,
        Word = 6
    }
}
