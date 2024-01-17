using Paganism.PParser.AST.Enums;

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
