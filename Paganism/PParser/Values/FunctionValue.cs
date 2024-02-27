using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

#pragma warning disable CS0659
namespace Paganism.PParser.Values
{
    public class FunctionValue : Value
    {
        public FunctionValue(FunctionDeclarateExpression value)
        {
            Value = value;
        }

        public override string Name => "Function";

        public override TypesType Type => TypesType.Function;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String
        };

        public FunctionDeclarateExpression Value { get; set; }

        public override void Set(object value)
        {
            if (value is FunctionValue functionValue)
            {
                Value = functionValue.Value;
                return;
            }
        }

        public override string AsString()
        {
            if (Value is null)
            {
                return Name + " (Function)";
            }

            var result = $"{Name} (Function): ";
            result += "[";

            foreach (var argument in Value.RequiredArguments)
            {
                result += $"[{argument.Name}: {argument.Type}], ";
            }

            result += "]";

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is not FunctionValue functionValue)
            {
                return false;
            }

            if (Value.Name != functionValue.Value.Name)
            {
                return false;
            }

            if (Value.ReturnType.IsType(functionValue.Value.ReturnType))
            {
                return false;
            }

            if (Value.IsShow != functionValue.Value.IsShow)
            {
                return false;
            }

            if (Value.IsAsync != functionValue.Value.IsAsync)
            {
                return false;
            }

            if (Value.RequiredArguments.Length != functionValue.Value.RequiredArguments.Length)
            {
                return false;
            }

            for (int i = 0; i < Value.RequiredArguments.Length; i++)
            {
                var argument = Value.RequiredArguments[i];
                var functionArgument = functionValue.Value.RequiredArguments[i];

                if (argument.Type.IsType(functionArgument.Type))
                {
                    return false;
                }

                if (argument.IsArray != functionValue.Value.RequiredArguments[i].IsArray)
                {
                    return false;
                }

                if (argument.IsRequired != functionValue.Value.RequiredArguments[i].IsRequired)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
