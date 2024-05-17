using Paganism.Interpreter.Data.Instances;
using System;

namespace Paganism.Interpreter.Data
{
    public class Variables : DataStorage<VariableInstance>
    {
        public override string Name => "Variable";

        public static Variables Instance => Lazy.Value;

        private static Lazy<Variables> Lazy { get; } = new();
    }
}
