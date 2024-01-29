using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

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
    }
}
