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
        public BlockStatementExpression(IStatement[] statements, bool isLoop = false)
        {
            Statements = statements;
            IsLoop = isLoop;
        }

        public IStatement[] Statements { get; }

        public bool IsLoop { get; }

        public bool IsBreaked { get; private set; }

        public void Execute(params Argument[] arguments)
        { 
            ExecuteAndReturn(arguments);
        }

        public Expression[] ExecuteAndReturn(params Argument[] arguments)
        {
            if (Statements == null) return null;

            var createdVariables = new HashSet<VariableExpression>();
            var createdStructures = new HashSet<StructureDeclarateExpression>();
            var createdFunctions = new HashSet<FunctionDeclarateExpression>();
            Expression[] result = new Expression[0];

            for (int i = 0;i < Statements.Length;i++)
            {
                if (IsLoop && IsBreaked) break;

                var statement = Statements[i];

                switch (statement)
                {
                    case ReturnExpression returnExpression:
                        result = returnExpression.Values;
                        break;
                    case BreakExpression:
                        if (IsLoop)
                        {
                            i = Statements.Length;
                            IsBreaked = true;
                            break;
                        }
                        break;
                    case BinaryOperatorExpression assignExpression:
                        if (assignExpression.Left is VariableExpression variableExpression)
                        {
                            if (Variables.Get(variableExpression.Name) != null)
                            {
                                Variables.Set(variableExpression.Name, assignExpression.Right.Eval());
                                break;
                            }

                            createdVariables.Add(variableExpression);
                            Variables.Add(variableExpression.Name, assignExpression.Right.Eval());
                        }
                        break;
                    case FunctionDeclarateExpression functionDeclarate:
                        createdFunctions.Add(functionDeclarate);
                        functionDeclarate.Create();
                        break;
                    case StructureDeclarateExpression structureDeclarate:
                        createdStructures.Add(structureDeclarate);
                        structureDeclarate.Create();
                        break;
                    case IfExpression ifExpression:
                        ifExpression.Execute();

                        if (IsLoop && (ifExpression.BlockStatement.IsBreaked || ifExpression.ElseBlockStatement.IsBreaked))
                        {
                            IsBreaked = true;
                            break;
                        }

                        break;
                    case FunctionCallExpression functionCallExpression:
                        functionCallExpression.Execute();
                        break;
                    case ForExpression forExpression:
                        var variable = forExpression.Variable as AssignExpression;

                        Variables.Add((variable.Left as VariableExpression).Name, variable.Right.Eval());

                        forExpression.Execute();

                        Variables.Remove((variable.Left as VariableExpression).Name);
                        break;
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

            foreach (var structure in createdStructures)
            {
                Structures.Remove(structure.Name);
            }

            return result;
        }
    }
}
