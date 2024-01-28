using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

namespace Paganism.PParser
{
    public class Argument
    {
        public Argument(string name, TypesType requiredValue, EvaluableExpression value = null, bool isRequired = false, bool isArray = false, string typeName = null)
        {
            Name = name;
            Type = requiredValue;
            IsRequired = isRequired;
            Value = value;
            IsArray = isArray;
            TypeName = typeName;
        }

        public string Name { get; set; }

        public EvaluableExpression Value { get; set; }

        public TypesType Type { get; }

        public bool IsArray { get; }

        public bool IsRequired { get; }

        public string TypeName { get; }
    }
}
