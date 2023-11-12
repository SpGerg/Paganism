using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class FunctionDeclarateExpression : Expression, IStatement, IExecutable
    {
        public FunctionDeclarateExpression(string name, IStatement statement, Argument[] requiredArguments)
        {
            Name = name;
            Statement = statement;
            RequiredArguments = requiredArguments;

            Functions.Add(this);
        }

        public string Name { get; }

        public IStatement Statement { get; }

        public Argument[] RequiredArguments { get; }

        public void Execute(params Argument[] arguments)
        {
            if (Statement == null) return;

            if (Statement is IExecutable executable)
            {
                executable.Execute();
            }
        }
    }
}
