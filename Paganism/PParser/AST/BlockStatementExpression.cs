using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Enums;
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
        public BlockStatementExpression(IStatement[] statements, bool isLoop = false, bool isClearing = true)
        {
            Statements = statements;
            IsLoop = isLoop;
            IsClearing = isClearing;
        }

        public IStatement[] Statements { get; }

        public bool IsLoop { get; }

        public bool IsClearing { get; set; }

        public bool IsBreaked { get; private set; }

        public void Execute(params Argument[] arguments)
        { 
            ExecuteAndReturn(arguments);
        }

        public Value ExecuteAndReturn(params Argument[] arguments)
        {
            if (Statements == null) return null;

            var createdVariables = new HashSet<VariableExpression>();
            var createdStructures = new HashSet<StructureDeclarateExpression>();
            var createdFunctions = new HashSet<FunctionDeclarateExpression>();
            Value result = null;

            for (int i = 0;i < Statements.Length;i++)
            {
                if (IsLoop && IsBreaked) break;

                var statement = Statements[i];

                switch (statement)
                {
                    case ReturnExpression returnExpression:
                        result = (returnExpression.Values[0] as IEvaluable).Eval();
                        break;
                    case BreakExpression:
                        if (IsLoop)
                        {
                            i = Statements.Length;
                            IsBreaked = true;
                            break;
                        }

                        break;
                    case BinaryOperatorExpression binaryOperator:
                        if (binaryOperator.Type == BinaryOperatorType.Assign)
                        {
                            if (binaryOperator.Left is ArrayElementExpression arrayElement)
                            {
                                var variable2 = Variables.Get(arrayElement.Name);

                                if (variable2 is not NoneValue)
                                {
                                    var array = (variable2 as ArrayValue);

                                    var eval = arrayElement.EvalWithKey();

                                    if (eval.Value is NoneValue)
                                    {
                                        array.Set(eval.Key, binaryOperator.Right.Eval());
                                    }
                                    else
                                    {
                                        eval.Value.Set(binaryOperator.Right.Eval());
                                    }
                                    
                                    break;
                                }
                            }
                            else if (binaryOperator.Left is VariableExpression variableExpression)
                            {
                                var variable2 = Variables.Get(variableExpression.Name);

                                if (variable2 is not NoneValue)
                                {
                                    if (variable2.Type != binaryOperator.Right.Eval().Type)
                                    {
                                        throw new InterpreterException($"Except variable with {variable2.Type} type");
                                    }

                                    Variables.Set(variableExpression.Name, binaryOperator.Right.Eval());
                                    break;
                                }

                                var left = (binaryOperator.Left as VariableExpression);
                                var right = binaryOperator.Right.Eval();

                                var rightType = right.Type;

                                if ((left.Type != TypesType.Any && rightType != TypesType.None) && left.Type != right.Type)
                                {
                                    throw new InterpreterException($"Except variable with {left.Type} type");
                                }

                                createdVariables.Add(variableExpression);
                                Variables.Add(variableExpression.Name, binaryOperator.Right.Eval());
                            }

                            if (binaryOperator.Left is BinaryOperatorExpression binary)
                            {
                                if (binary.Type != BinaryOperatorType.Point)
                                {
                                    throw new InterpreterException("Left expression must be structure member");
                                }

                                if (binary.Left is not BinaryOperatorExpression && binary.Right is not BinaryOperatorExpression)
                                {
                                    var left = Variables.Get((binary.Left as VariableExpression).Name) as StructureValue;
                                    left.Set((binary.Right as VariableExpression).Name, binaryOperator.Right.Eval());
                                    break;
                                }

                                var parent = Variables.Get((binary.Left as VariableExpression).Name) as StructureValue;
                                var child = BinaryOperatorExpression.GetStructure(binary.Left as Expression, binary.Right as Expression, parent);
                                var member = binary.PointKeyValuePair();

                                Variables.DeclaratedVariables.Count();

                                if (parent == child.Value)
                                {
                                    Variables.Set((binaryOperator.Left as VariableExpression).Name, binaryOperator.Right.Eval());
                                    break;
                                }

                                if (member.Value is NoneValue)
                                {
                                    (child.Value as StructureValue).Values[member.Key] = binaryOperator.Right.Eval();
                                    break;
                                }

                                member.Value.Set(binaryOperator.Right.Eval());
                            }
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

                        if (variable != null)
                        {
                            Variables.Add((variable.Left as VariableExpression).Name, variable.Right.Eval());
                        }          

                        forExpression.Execute();

                        Variables.Remove((variable.Left as VariableExpression).Name);
                        break;
                }
            }

            if (IsClearing)
            {
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
            }
            

            return result;
        }
    }
}
