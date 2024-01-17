using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

namespace Paganism.PParser
{
    public class Argument
    {
        public Argument(string name, TypesType requiredValue, EvaluableExpression value = null, bool isRequired = false, bool isArray = false, string structureName = null)
        {
            Name = name;
            Type = requiredValue;
            IsRequired = isRequired;
            Value = value;
            IsArray = isArray;
            StructureName = structureName;
        }

        public string Name { get; set; }

        public EvaluableExpression Value { get; set; }

        public TypesType Type { get; }

        public bool IsArray { get; }

        public bool IsRequired { get; }

        public string StructureName { get; }
    }
}
