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

            if (Statement == null || Statement.Statements == null) return;

            if (ReturnTypes.Length > 0 && Statement.Statements.FirstOrDefault(statementInBlock => statementInBlock is ReturnExpression) == default)
            {
                throw new Exception($"Function with {Name} name must return value");
            }

            if (ReturnTypes.Length == 0 && Statement.Statements.FirstOrDefault(statementInBlock => statementInBlock is ReturnExpression) != default)
            {
                throw new Exception($"Except return value type in function with {Name} name");
            }
        }

        public string Name { get; }

        public BlockStatementExpression Statement { get; }

        public Argument[] RequiredArguments { get; }

        public TokenType[] ReturnTypes { get; }

        private static Type Console => AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.Name == "Console");

        public void Create()
        {
            Functions.Add(this);
        }

        public void Remove()
        {
            Functions.Remove(Name);
        }

        public Expression[] ExecuteAndReturn(params Argument[] arguments)
        {
            if (Name == "call_lang")
            {
                Type findedClass = null;

                if (arguments[0].Value.Eval().AsString() == Console.FullName)
                {
                    findedClass = Console;
                }

                var method = findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(string) });
                method.Invoke(null, new object[] { arguments[2].Value.Eval().AsString() });

                return new Expression[0];
            }
            else if (Name == "create")
            {
                return new Expression[] { new StructureExpression(arguments[0].Value.Eval().AsString()) };
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
