using Paganism.PParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism
{
    public static class Functions
    {
        private static Dictionary<string, FunctionDeclarateExpression> DeclaratedFunctions { get; } = new Dictionary<string, FunctionDeclarateExpression>();

        public static void Add(FunctionDeclarateExpression functionDeclarate)
        {
            DeclaratedFunctions.Add(functionDeclarate.Name, functionDeclarate);
        }

        public static FunctionDeclarateExpression Get(string name)
        {
            if (!DeclaratedFunctions.TryGetValue(name, out FunctionDeclarateExpression result))
            {
                throw new Exception($"Function with {name} name not found");
            }

            return result;
        }
    }
}
