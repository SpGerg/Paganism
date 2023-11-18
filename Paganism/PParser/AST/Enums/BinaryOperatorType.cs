using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST.Enums
{
    public enum BinaryOperatorType
    {
        None = 0,
        Plus = 1,
        Minus = 2,
        Division = 3,
        Multiplicative = 4,
        Remainder = 5,
        Assign = 6,
        Is = 7
    }
}
