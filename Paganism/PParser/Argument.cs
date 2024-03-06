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

            if (Value is ArrayElementExpression elementExpression && elementExpression.Eval() is ArrayValue)
            {
                IsArray = true;
            }

            if (value is not null)
            {
                Type = value.GetTypeValue();
            }
        }

        public Argument(string name, TypesType requiredValue, EvaluableExpression value = null, bool isRequired = false, bool isArray = false, string typeName = null) : this(name, new TypeValue(new ExpressionInfo(), requiredValue, typeName), value, isRequired, isArray)
        {
        }

        public string Name { get; set; }

        public EvaluableExpression Value { get; set; }

        public TypeValue Type { get; }

        public bool IsArray { get; }

        public bool IsRequired { get; }

        public override bool Equals(object obj)
        {
            if (Type.Value is TypesType.Any)
            {
                return true;
            }

            if (obj is Argument argument)
            {
                return Type.IsType(argument.Type) && IsArray == argument.IsArray;
            }

            return base.Equals(obj);
        }
    }
}
