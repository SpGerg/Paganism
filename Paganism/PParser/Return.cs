using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser
{
    public class Return
    {
        public Return(TypesType type, string structureName)
        {
            Type = type;
            StructureName = structureName;
        }

        public TypesType Type { get; }

        public string StructureName { get; }
    }
}
