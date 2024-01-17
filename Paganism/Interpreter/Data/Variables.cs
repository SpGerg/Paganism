using Paganism.PParser.Values;
using System;

namespace Paganism.Interpreter.Data
{
    public class Variables : DataStorage<Value>
    {
        public static Lazy<Variables> Instance { get; } = new();

        public override string Name => "Variable";
    }
}
