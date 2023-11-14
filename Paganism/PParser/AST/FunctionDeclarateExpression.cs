using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        }

        public string Name { get; }

        public IStatement Statement { get; }

        public Argument[] RequiredArguments { get; }

        public void Create()
        {
            Functions.Add(this);
        }

        public void Remove()
        {
            Functions.Remove(this);
        }

        public void Execute(params Argument[] arguments)
        {
            if (Name == "call_lang")
            {
                var findedClass = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.FullName == arguments[0].Value.AsString());
                var method = findedClass.GetMethod(arguments[1].Value.AsString(), new Type[] { typeof(string) });
                method.Invoke(null, new object[] { arguments[2].Value.AsString() });

                return;
            }

            if (Statement == null) return;

            if (Statement is IExecutable executable)
            {
                executable.Execute();
            }
        }
    }
}
