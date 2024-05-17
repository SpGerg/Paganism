using Paganism.Interpreter.Data.Instances;
using System;

namespace Paganism.Interpreter.Data
{
    public class Enums : DataStorage<EnumInstance>
    {
        public override string Name => "Enum";

        public static Enums Instance => Lazy.Value;

        private static Lazy<Enums> Lazy { get; } = new();
    }
}
