using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class FunctionValue : Value
    {
        public FunctionValue(FunctionInstance va)
        {
            Value = Value;
        }

        public override string Name => "Function";

        public override TypesType Type => TypesType.Function;

        public FunctionInstance Value { get; }

        public override string AsString()
        {
            var result = $"{Name}: ";
            result += "[";

            foreach (var argument in Value.Arguments)
            {
                result += $"[{argument.Name}: {argument.Type}], ";
            }

            result += "]";

            return result;
        }
    }
}
