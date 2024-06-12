using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;

namespace Paganism.PParser
{
    public class Argument
    {
        public Argument(string name, TypeValue typeValue, EvaluableExpression value = null, bool isRequired = false, bool isArray = false)
        {
            Name = name;
            Type = typeValue;
            IsRequired = isRequired;
            Value = value;
            IsArray = isArray;

            if (Value is ArrayExpression)
            {
                IsArray = true;
            }

            if (Value is ArrayElementExpression elementExpression && elementExpression.Evaluate() is ArrayValue)
            {
                IsArray = true;
            }

            if (value is not null and not FunctionCallExpression)
            {
                Type = value.GetTypeValue();
            }
        }

        public Argument(string name, TypesType requiredValue, EvaluableExpression value = null, bool isRequired = false, bool isArray = false, string typeName = null)
            : this(name, new TypeValue(ExpressionInfo.EmptyInfo, requiredValue, typeName), value, isRequired, isArray)
        {
        }

        public string Name { get; set; }

        public EvaluableExpression Value { get; set; }

        public TypeValue Type { get; }

        public bool IsArray { get; }

        public bool IsRequired { get; }

        public bool Is(Argument argument)
        {
            return argument.Type.Is(Type) && argument.IsArray == IsArray && argument.IsRequired == IsRequired;
        }
    }
}
