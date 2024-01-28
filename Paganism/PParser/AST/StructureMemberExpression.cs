using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class StructureMemberExpression : Expression, IStatement
    {
        public StructureMemberExpression(BlockStatementExpression parent, int line, int position, string filepath, string structure, string typeName, TypesType type, string name, bool isShow = false, bool isDelegate = false, Argument[] arguments = null, bool isCastable = false) : base(parent, line, position, filepath)
        {
            Structure = structure;
            TypeName = typeName;
            Type = type;
            Name = name;
            IsShow = isShow;
            IsDelegate = isDelegate;
            Arguments = arguments;
            IsCastable = isCastable;
        }

        public string Structure { get; }

        public string TypeName { get; }

        public TypesType Type { get; }

        public string Name { get; }

        public bool IsDelegate { get; }

        public Argument[] Arguments { get; }

        public bool IsShow { get; }

        public bool IsCastable { get; }

        public string GetRequiredType()
        {
            return TypeName == string.Empty || TypeName is null ? Type.ToString() : $"{TypeName} ({Type})";
        }
    }
}
