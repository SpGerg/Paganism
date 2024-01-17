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
        public StructureMemberExpression(BlockStatementExpression parent, int line, int position, string filepath, string structure, string structureTypeName, TypesType type, string name, bool isShow = false, bool isCastable = false) : base(parent, line, position, filepath)
        {
            Structure = structure;
            StructureTypeName = structureTypeName;
            Type = type;
            Name = name;
            IsShow = isShow;
            IsCastable = isCastable;
        }

        public string Structure { get; }

        public string StructureTypeName { get; }

        public TypesType Type { get; }

        public string Name { get; }

        public bool IsShow { get; }

        public bool IsCastable { get; }
    }
}
