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
        public static Lazy<Enums> Instance { get; } = new();

        public override string Name => "Enum";
    }
}
