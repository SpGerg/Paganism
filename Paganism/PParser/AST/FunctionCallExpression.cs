using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class FunctionCallExpression : Expression, IStatement
    {
        public FunctionCallExpression(string functionName, Argument[] arguments)
        {
            FunctionName = functionName;
            Arguments = arguments;
        }

        public string FunctionName { get; }

        public Argument[] Arguments { get; }

        public void Execute(params Value[] arguments)
        {
            Functions.Get(FunctionName).Execute(arguments);
        }
    }
}
