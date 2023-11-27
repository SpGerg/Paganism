using Paganism.Exceptions;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data
{
    public class Structures
    {
        public static Dictionary<string, StructureInstance> DeclaratedStructures { get; } = new Dictionary<string, StructureInstance>();

        public static void Add(StructureDeclarateExpression structureDeclarate)
        {
            DeclaratedStructures.Add(structureDeclarate.Name, new StructureInstance(structureDeclarate));
        }

        public static void Remove(string name)
        {
            DeclaratedStructures.Remove(name);
        }

        public static void Clear()
        {
            DeclaratedStructures.Clear();
        }

        public static StructureInstance Get(string name)
        {
            if (!DeclaratedStructures.TryGetValue(name, out StructureInstance result))
            {
                throw new InterpreterException($"Structure with {name} name not found");
            }

            return result;
        }
    }
}
