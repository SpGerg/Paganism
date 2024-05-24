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
                    return new FunctionTypeValue(functionDeclarateExpression.ExpressionInfo, functionDeclarateExpression, functionDeclarateExpression.IsAsync);
                case FunctionCallExpression functionCallExpression:
                    var function = functionCallExpression.GetFunction();

                    var type = function.ReturnType is null ? new TypeValue(ExpressionInfo, Enums.TypesType.None, string.Empty) : function.ReturnType;

                    return type;
                case NewExpression newExpression:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Structure, newExpression.Name);
                case TypeValue typeValue:
                    return typeValue;
                case Value value2:

                    if (value2 is FunctionValue functionValue)
                    {
                        return new FunctionTypeValue(functionValue.ExpressionInfo, functionValue.Value.ReturnType, functionValue.Value.Arguments, functionValue.Value.IsAsync);
                    }

                    return new TypeValue(ExpressionInfo, value2.Type, value2.GetTypeName());
                case NotExpression:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Boolean, string.Empty);
                case UnaryExpression:
                    return new TypeValue(ExpressionInfo, Enums.TypesType.Number, string.Empty);
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
