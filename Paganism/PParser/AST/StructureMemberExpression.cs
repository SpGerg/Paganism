using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class StructureMemberExpression : Expression, IStatement
    {
        public StructureMemberExpression(string structure, TypesType type, string name, bool isShow = false, bool isCastable = false)
        {
            Structure = structure;
            Type = type;
            Name = name;
            IsShow = isShow;
            IsCastable = isCastable;
        }

        public string Structure { get; }

        public TypesType Type { get; }

        public string Name { get; }

        public bool IsShow { get; }

        public bool IsCastable { get; }
    }
}
