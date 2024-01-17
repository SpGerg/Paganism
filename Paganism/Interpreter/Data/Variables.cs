using Paganism.PParser.AST;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data
{
    public class Variables : DataStorage<Value>
    {
        public static Lazy<Variables> Instance { get; } = new();

        public override string Name => "Variable";
    }
}
