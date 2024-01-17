using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class AwaitExpression : Expression, IStatement, IExecutable
    {
        public AwaitExpression(BlockStatementExpression parent, int position, int line, string filepath, Expression expression) : base(parent, position, line, filepath)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public void Execute(params Argument[] arguments)
        {
            var function = Expression as FunctionCallExpression;

            if (!function.IsAwait)
            {
                throw new InterpreterException("You need async function to use await.", function.Line, function.Position);
            }

            var id = (int)((NumberValue)(function.Eval() as StructureValue).Values["id"]).Value;

            var task = Tasks.Get(id);

            task.Wait();
        }
    }
}
