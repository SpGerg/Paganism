using Paganism.Lexer.Enums;
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
        public FunctionDeclarateExpression(string name, BlockStatementExpression statement, Argument[] requiredArguments, params TokenType[] returnTypes)
        {
            Name = name;
            Statement = statement;
            RequiredArguments = requiredArguments;
            ReturnTypes = returnTypes;
        }

        public string Name { get; }

        public BlockStatementExpression Statement { get; }

        public Argument[] RequiredArguments { get; }

        public TokenType[] ReturnTypes { get; }

        public void Create()
        {
            Functions.Add(this);
        }

        public void Remove()
        {
            Functions.Remove(this);
        }

        public Expression[] ExecuteAndReturn(params Argument[] arguments)
        {
            if (Name == "call_lang")
            {
                var findedClass = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.FullName == arguments[0].Value.Eval().AsString());
                var method = findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(string) });
                method.Invoke(null, new object[] { arguments[2].Value.Eval().AsString() });

                return new Expression[0];
            }

            if (Statement == null) return new Expression[0];

            return Statement.ExecuteAndReturn(arguments);
        }

        public void Execute(params Argument[] arguments)
        {
            ExecuteAndReturn(arguments);
        }
    }
}
