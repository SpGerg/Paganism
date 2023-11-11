using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class FunctionDeclarateExpression : Expression, IStatement
    {
        public FunctionDeclarateExpression(string name, IStatement statement, Argument[] arguments)
        {
            Name = name;
            Statement = statement;
            Arguments = arguments;
        }

        public string Name { get; }

        public IStatement Statement { get; }

        public Argument[] Arguments { get; }

        public void Execute(params Value[] arguments)
        {
            Statement.Execute(arguments);
        }
    }
}
