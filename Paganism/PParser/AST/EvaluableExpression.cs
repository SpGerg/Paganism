using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public abstract class EvaluableExpression : Expression
    {
        protected EvaluableExpression(ExpressionInfo info) : base(info)
        {
        }

        public abstract Value Evaluate(params Argument[] arguments);

        private TypeValue GetTypeValue(EvaluableExpression evaluableExpression)
        {
            switch (evaluableExpression)
            {
                case ArrayElementExpression arrayElementExpression:
                    var value = arrayElementExpression.Evaluate();

                    return new TypeValue(ExpressionInfo, value.Type, value.GetTypeName());
                case ArrayExpression:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Array, string.Empty);
                case BinaryOperatorExpression binaryOperatorExpression:
                    return binaryOperatorExpression.GetBinaryValueType();
                case FunctionDeclarateExpression functionDeclarateExpression:
                    return functionDeclarateExpression.ReturnType;
                case FunctionCallExpression functionCallExpression:
                    var function = functionCallExpression.GetFunction();

                    var type = function.ReturnType is null ? new TypeValue(ExpressionInfo, Enums.TypesType.None, string.Empty) : function.ReturnType;

                    return type;
                case NewExpression newExpression:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Structure, newExpression.Name);
                case NoneValue:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.None, string.Empty);
                case NumberValue:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Number, string.Empty);
                case StringValue:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.String, string.Empty);
                case NotExpression:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Boolean, string.Empty);
                case UnaryExpression:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Number, string.Empty);
                case TypeValue typeValue:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Type, typeValue.TypeName);
                case ReturnExpression returnExpression:
                    var value3 = GetTypeValue(returnExpression.Value);

                    return new TypeValue(ExpressionInfo, value3.Type, value3.TypeName);
            }

            return new TypeValue(ExpressionInfo, Enums.TypesType.Any, string.Empty);
        }

        public TypeValue GetTypeValue()
        {
            return GetTypeValue(this);
        }
    }
}
