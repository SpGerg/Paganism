using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class FunctionTypeValue : TypeValue
    {
        public FunctionTypeValue(ExpressionInfo info, TypesType value, string typeName, Argument[] arguments, bool isAsync) : base(info, value, typeName)
        {
            Arguments = arguments;

            ReturnType = new TypeValue(info, value, typeName);
            IsAsync = isAsync;
        }

        public FunctionTypeValue(ExpressionInfo info, TypeValue typeValue, Argument[] arguments, bool isAsync) : this(info, typeValue.Value, typeValue.TypeName, arguments, isAsync) { }

        public FunctionTypeValue(ExpressionInfo info, FunctionDeclarateExpression functionDeclarateExpression, bool isAsync) : this(info, functionDeclarateExpression.ReturnType, functionDeclarateExpression.Arguments, isAsync) { }

        public bool IsAsync { get; }

        public TypeValue ReturnType { get; }

        public Argument[] Arguments { get; }

        public bool IsArguments(Argument[] arguments)
        {
            for (int i = 0; i < Arguments.Length; i++)
            {
                var argument = Arguments[i];

                if (arguments.Length - 1 < i)
                {
                    return false;
                }

                if (!argument.Type.Is(arguments[i].Type))
                {
                    return false;
                }

                if (argument.IsRequired != argument.IsRequired)
                {
                    return false;
                }

                if (argument.IsArray != argument.IsArray)
                {
                    return false;
                }
            }

            return true;
        }

        public new string AsString()
        {
            var message = $"Function ({(IsAsync ? "async" : "not async")}) ({ReturnType}) (";

            foreach (var argument in Arguments)
            {
                message += argument.Type.ToString() + ", ";
            }

            return message + ")";
        }

        public new string ToString() => AsString();
    }
}
