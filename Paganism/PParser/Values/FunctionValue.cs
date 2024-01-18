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

        public FunctionDeclarateExpression Value { get; }

        public override string AsString()
        {
            var result = $"{Name}: ";
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
