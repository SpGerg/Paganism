using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

namespace Paganism.Interpreter.Data.Instances
{
    public class StructureMemberInstance : Instance
    {
        public StructureMemberInstance(StructureMemberExpression structureMember)
        {
            Name = structureMember.Name;
            Parent = structureMember.Structure;
            Type = structureMember.Type;
            IsShow = structureMember.IsShow;
            IsCastable = structureMember.IsCastable;
        }

        public StructureMemberInstance(string name, string parent, TypesType type, bool isShow, bool isCastable)
        {
            Name = name;
            Parent = parent;
            Type = type;
            IsShow = isShow;
            IsCastable = isCastable;
        }

        public override string InstanceName => "Structure member";

        public string Name { get; }

        public string Parent { get; }

        public TypesType Type { get; }

        public bool IsShow { get; }

        public bool IsCastable { get; }
    }
}
