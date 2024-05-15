using Paganism.Interpreter.Data.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data
{
    public class Enums : DataStorage<EnumInstance>
    {
        public override string Name => "Enum";

        public static Enums Instance => Lazy.Value;

        private static Lazy<Enums> Lazy { get; } = new();
    }
}
