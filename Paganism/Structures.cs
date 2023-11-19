using Paganism.PParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism
{
    public class Structures
    {
        private static Dictionary<string, StructureDeclarateExpression> DeclaratedStructures { get; } = new Dictionary<string, StructureDeclarateExpression>();

        static Structures()
        {
            
        }

        public static void Add(StructureDeclarateExpression functionDeclarate)
        {
            DeclaratedStructures.Add(functionDeclarate.Name, functionDeclarate);
        }

        public static void Remove(string name)
        {
            DeclaratedStructures.Remove(name);
        }

        public static void Clear()
        {
            DeclaratedStructures.Clear();
        }

        public static StructureDeclarateExpression Get(string name)
        {
            if (!DeclaratedStructures.TryGetValue(name, out StructureDeclarateExpression result))
            {
                throw new Exception($"Function with {name} name not found");
            }

            return result;
        }
    }
}
