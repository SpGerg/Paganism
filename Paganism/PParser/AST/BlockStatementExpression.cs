using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class BlockStatementExpression : Expression, IStatement, IExecutable
    {
        public BlockStatementExpression(IStatement[] statements)
        {
            Statements = statements;
        }

        public IStatement[] Statements { get; }

        public void Execute(params Argument[] arguments)
        { 
            ExecuteAndReturn(arguments);
        }

        public Expression[] ExecuteAndReturn(params Argument[] arguments)
        {
            if (Statements == null) return null;

            var createdVariables = new HashSet<VariableExpression>();
            var createdFunctions = new HashSet<FunctionDeclarateExpression>();
            Expression[] result = new Expression[0];

            foreach (var statement in Statements)
            {
                if (statement is ReturnExpression returnExpression)
                {
                    result = returnExpression.Values;
                }
                else if (statement is BinaryOperatorExpression assign)
                {
                    if (assign.Left is VariableExpression variableExpression)
                    {
                        if (Variables.Get(variableExpression.Name) != null)
                        {
                            Variables.Set(variableExpression.Name, assign.Right.Eval());
                            continue;
                        }

                        createdVariables.Add(variableExpression);
                        Variables.Add(variableExpression.Name, assign.Right.Eval());
                    }
                }
                else if(statement is FunctionDeclarateExpression functionDeclarate)
                {
                    createdFunctions.Add(functionDeclarate);
                    functionDeclarate.Create();
                }
                else if (statement is FunctionCallExpression callExpression)
                {
                    callExpression.Execute(callExpression.Arguments);
                }
                else if (statement is IfExpression ifExpression)
                {
                    ifExpression.Execute();
                }
            }

            foreach (var variable in createdVariables)
            {
                Variables.Remove(variable.Name);
            }

            foreach (var function in createdFunctions)
            {
                Functions.Remove(function.Name);
            }

            return result;
        }
    }
}
