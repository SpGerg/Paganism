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
        Word = 6,
        Plus = 7,
        Minus = 8,
        Slash = 9,
        Star = 10,
        Assign = 11,
        LeftPar = 12,
        RightPar = 13,
        Function = 14,
        End = 15,
        Comma = 16
    }
}
