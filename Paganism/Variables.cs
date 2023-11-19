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
        private static Dictionary<string, Value> DeclaratedVariables { get; } = new Dictionary<string, Value>();

        public static void Add(string name, Value value)
        {
            DeclaratedVariables.Add(name, value);
        }

        public static void Remove(string name)
        {
            DeclaratedVariables.Remove(name);
        }

        public static void Clear()
        {
            DeclaratedVariables.Clear();
        }

        public static Value Get(string name)
        {
            if (!DeclaratedVariables.TryGetValue(name, out Value value))
            {
                return null;
            }

            return Value.Create(value);
        }

        public static void Set(string name, Value value)
        {
            if (!DeclaratedVariables.ContainsKey(name))
            {
                return;
            }

            DeclaratedVariables[name] = value;
        }
    }
}
