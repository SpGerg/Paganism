using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using Paganism.Structures;

namespace Paganism.PParser.AST
{
    public class StructureMemberExpression : Expression, IStatement
    {
        public StructureMemberExpression(ExpressionInfo info, string structure, TypeValue typeValue, string name, bool isShow = false, bool isReadOnly = false, bool isAsync = false, bool isDelegate = false, Argument[] arguments = null, bool isCastable = false) : base(info)
        {
            Structure = structure;
            Type = typeValue;
            Name = name;
            Info = new StructureMemberInfo(isDelegate, arguments, isReadOnly, isShow, isCastable, isAsync);
        }

        public StructureMemberExpression(ExpressionInfo expressionInfo, string structure, TypeValue typeValue, string name,
            StructureMemberInfo info) : base(expressionInfo)
        {
            Structure = structure;
            Type = typeValue;
            Name = name;
            Info = info;
        }

        public string Structure { get; }

        public TypeValue Type { get; }

        public string Name { get; }

        public StructureMemberInfo Info { get; }
    }
}
