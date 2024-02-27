using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public abstract class EvaluableExpression : Expression
    {
        protected EvaluableExpression(BlockStatementExpression parent, int position, int line, string filepath) : base(parent, position, line, filepath)
        {
        }

        public abstract Value Eval(params Argument[] arguments);

        private TypeValue GetTypeValue(EvaluableExpression evaluableExpression)
        {
            switch (evaluableExpression)
            {
                case ArrayElementExpression arrayElementExpression:
                    var value = arrayElementExpression.Eval();

                    return new TypeValue(value.Type, value.GetTypeName());
                case ArrayExpression:
                    return new TypeValue(Enums.TypesType.Array, string.Empty);
                case BinaryOperatorExpression binaryOperatorExpression:
                    return binaryOperatorExpression.GetBinaryValueType();
                case FunctionDeclarateExpression functionDeclarateExpression:
                    return new TypeValue(functionDeclarateExpression.ReturnType.Value, functionDeclarateExpression.ReturnType.TypeName);
                case FunctionCallExpression functionCallExpression:
                    var function = functionCallExpression.GetFunction();

                    var type = function.ReturnType is null ? new TypeValue(Enums.TypesType.None, string.Empty) : function.ReturnType;

                    return type;
                case NewExpression newExpression:
                    return new TypeValue(Enums.TypesType.Structure, newExpression.Name);
                case NoneExpression:
                    return new TypeValue(Enums.TypesType.None, string.Empty);
                case NumberExpression:
                    return new TypeValue(Enums.TypesType.Number, string.Empty);
                case StringExpression:
                    return new TypeValue(Enums.TypesType.String, string.Empty);
                case NotExpression:
                    return new TypeValue(Enums.TypesType.Boolean, string.Empty);
                case UnaryExpression:
                    return new TypeValue(Enums.TypesType.Number, string.Empty);
                case TypeExpression typeExpression:
                    return new TypeValue(Enums.TypesType.Type, typeExpression.TypeName);
                case ReturnExpression returnExpression:
                    var value3 = GetTypeValue(returnExpression.Value);

                    return new TypeValue(value3.Type, value3.TypeName);
            }

            return new TypeValue(Enums.TypesType.Any, string.Empty);
        }

        public TypeValue GetTypeValue()
        {
            return GetTypeValue(this);
        }
    }
}
