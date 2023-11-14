using Paganism.PParser.AST;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism
{
    public class Variables
    {
        private static Dictionary<string, Value> CreatedVariables { get; } = new Dictionary<string, Value>();

        public static void Add(string name, Value value)
        {
            CreatedVariables.Add(name, value);
        }

        public static void Remove(string name)
        {
            CreatedVariables.Remove(name);
        }

        public static Value Get(string name)
        {
            if (!CreatedVariables.TryGetValue(name, out Value value))
            {
                return null;
            }

            return value;
        }

        public static void Set(string name, Value value)
        {
            if (!CreatedVariables.ContainsKey(name))
            {
                return;
            }

            CreatedVariables[name] = value;
        }
    }
}
