using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Extensions;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Paganism.PParser.AST
{
    public class BinaryOperatorExpression : EvaluableExpression, IStatement
    {
        public BinaryOperatorExpression(ExpressionInfo info, BinaryOperatorType type, EvaluableExpression left, EvaluableExpression right) : base(info)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public BinaryOperatorType Type { get; }

        public EvaluableExpression Left { get; }

        public EvaluableExpression Right { get; }

        public static StructureValue GetStructure(BinaryOperatorExpression binaryOperatorExpression)
        {
            if (binaryOperatorExpression.Left is VariableExpression variableExpression)
            {
                var left = Variables.Instance.Get(binaryOperatorExpression.ExpressionInfo.Parent, variableExpression.Name, variableExpression.ExpressionInfo) as StructureValue;

                return left;
            }

            if (binaryOperatorExpression.Left is BinaryOperatorExpression binary)
            {
                var structure = GetStructure(binary);

                var name = string.Empty;

                if (binary.Right is VariableExpression variableExpression2)
                {
                    name = variableExpression2.Name;
                }
                else if (binary.Right is FunctionCallExpression functionCallExpression)
                {
                    name = functionCallExpression.FunctionName;
                }

                if (!structure.Structure.Members[name].Info.IsShow &&
                    structure.Structure.StructureDeclarateExpression.ExpressionInfo.Filepath != binary.ExpressionInfo.Filepath)
                {
                    throw new InterpreterException($"You cant access to structure member '{name}' in '{structure.Structure.Name}' structure",
                        binaryOperatorExpression.ExpressionInfo);
                }

                var member = structure.Values[name];

                if (member is StructureValue structureValue)
                {
                    return structureValue;
                }
                else if (member is FunctionValue functionValue && binary.Right is FunctionCallExpression functionCallExpression)
                {
                    var value = functionValue.Value.Evaluate(functionCallExpression.Arguments);

                    if (value is not StructureValue structureValue1)
                    {
                        return null;
                    }

                    return structureValue1;
                }
            }

            return null;
        }

        public static KeyValuePair<string, Value> GetMemberWithKeyOfStructure(BinaryOperatorExpression binaryOperatorExpression)
        {
            var structure = GetStructure(binaryOperatorExpression);
            var name = string.Empty;

            if (binaryOperatorExpression.Right is VariableExpression variableExpression)
            {
                name = variableExpression.Name.Replace("()", string.Empty);
            }
            else if (binaryOperatorExpression.Right is ArrayElementExpression arrayElementExpression)
            {
                name = arrayElementExpression.Name;
            }
            else if (binaryOperatorExpression.Right is FunctionCallExpression functionCallExpression)
            {
                name = functionCallExpression.FunctionName;
            }

            if (!structure.Values.ContainsKey(name))
            {
                return new();
            }

            var member = structure.Values[name];

            if (!structure.Structure.Members[name].Info.IsShow &&
                structure.Structure.StructureDeclarateExpression.ExpressionInfo.Filepath != binaryOperatorExpression.ExpressionInfo.Filepath)
            {
                throw new InterpreterException($"You cant access to structure member '{name}' in '{structure.Structure.Name}' structure",
                    binaryOperatorExpression.ExpressionInfo);
            }

            if (structure is null)
            {
                throw new InterpreterException("Structure is null",
                    binaryOperatorExpression.ExpressionInfo);
            }

            if (member is FunctionValue functionValue && binaryOperatorExpression.Right is FunctionCallExpression callExpression)
            {
                if (functionValue.Value is not null)
                {
                    return new KeyValuePair<string, Value>(name, functionValue.Value.Evaluate(callExpression.Arguments));
                }
                else
                {
                    if (structure.Structure.Functions.TryGetValue(name, out var value))
                    {
                        return new KeyValuePair<string, Value>(name, value.Invoke(callExpression.Arguments));
                    }
                }
            }

            return new KeyValuePair<string, Value>(name, member);
        }

        public static Value GetMemberOfStructure(BinaryOperatorExpression binaryOperatorExpression)
        {
            return GetMemberWithKeyOfStructure(binaryOperatorExpression).Value;
        }

        public override Value Evaluate(params Argument[] arguments)
        {
            if (Type is BinaryOperatorType.Point)
            {
                return Point();
            }
            else if (Type is BinaryOperatorType.Assign)
            {
                return Assign();
            }

            var left = Left.Evaluate();
            var right = Right.Evaluate();

            return Type switch
            {
                BinaryOperatorType.Plus => Addition(left, right),
                BinaryOperatorType.Minus => Minus(left, right),
                BinaryOperatorType.Multiplicative => Multiplicative(left, right),
                BinaryOperatorType.Division => Division(left, right),
                BinaryOperatorType.Is => Is(left, right),
                BinaryOperatorType.And => And(left, right),
                BinaryOperatorType.Or => Or(left, right),
                BinaryOperatorType.Less => Less(left, right),
                BinaryOperatorType.More => More(left, right),
                BinaryOperatorType.As => As(left, right),
                _ => null,
            };
        }

        public TypeValue GetBinaryValueType()
        {
            if (Type is BinaryOperatorType.As)
            {
                return new TypeValue(ExpressionInfo, (Right as TypeValue).Value, (Right as TypeValue).TypeName);
            }

            return Left.GetTypeValue();
        }

        private Value As(Value left, Value right)
        {
            return right is not TypeValue typeValue
                ? throw new InterpreterException("Right expression must be a type", ExpressionInfo)
                : typeValue.Value switch
                {
                    TypesType.Any => new StringValue(ExpressionInfo, left.AsString()),
                    TypesType.Number => new NumberValue(ExpressionInfo, left.AsNumber()),
                    TypesType.String => new StringValue(ExpressionInfo, left.AsString()),
                    TypesType.Boolean => new BooleanValue(ExpressionInfo, left.AsBoolean()),
                    TypesType.Char => AsChar(left, right),
                    TypesType.None => new NoneValue(ExpressionInfo.EmptyInfo),
                    TypesType.Structure => AsStructure(left, typeValue),
                    _ => throw new InterpreterException($"You cant check type {left.Type} and {right.Type}", ExpressionInfo),
                };
        }

        private Value AsStructure(Value left, TypeValue right)
        {
            if (left is not StructureValue structureValue || right.TypeName == string.Empty || right.TypeName is null)
            {
                throw new InterpreterException($"Cannot cast {left.Type} to Structure", ExpressionInfo);
            }

            foreach (var member in structureValue.Structure.Members)
            {
                if (!member.Value.Info.IsCastable)
                {
                    continue;
                }

                var value = structureValue.Values[member.Key];

                if (value is not StructureValue structureValue1)
                {
                    continue;
                }

                if (structureValue1.Structure.Name != right.TypeName)
                {
                    continue;
                }

                return structureValue1;
            }

            throw new InterpreterException($"Structure with '{structureValue.Structure.Name}' name havent castable member with '{right.TypeName}' type", ExpressionInfo);
        }

        private Value AsChar(Value left, Value right)
        {
            if (left is StringValue stringValue && stringValue.Value.Length == 1)
            {
                return new CharValue(ExpressionInfo, left.AsString()[0]);
            }
            else
            {
                throw new InterpreterException("Cannot cast string to char. String must be contains only one character.",
                ExpressionInfo);
            }
        }

        private Value Point()
        {
            if (Left is VariableExpression variableExpression && Interpreter.Data.Enums.Instance.TryGet(ExpressionInfo.Parent, variableExpression.Name, out var value, variableExpression.ExpressionInfo))
            {
                if (Right is not VariableExpression variableExpression1)
                {
                    throw new InterpreterException("Except enum member name", variableExpression.ExpressionInfo);
                }

                return new EnumValue(ExpressionInfo, value.Members[variableExpression1.Name]);
            }

            if (Left is VariableExpression leftVariableExpression && Right is FunctionCallExpression functionCallExpression)
            {
                //thats part of foxworn code, this is code broken.
                //it will be broke somethjing if im delete code?
                /*
                Dictionary<string, dynamic> InternalFunctionExtension;
                switch (leftVariableExpression.Type.Value)
                {
                    case TypesType.String:
                        InternalFunctionExtension = Extension.StringExtension; break;
                    default:
                        throw new ExtensionException($"Cannot use any point function for a {leftVariableExpression.Type.Value} variable!");
                }

                if (InternalFunctionExtension.ContainsKey(functionCallExpression.FunctionName) && Extension.TryGet(InternalFunctionExtension, functionCallExpression.FunctionName, out dynamic ExtensionElement))
                {
                    if (ExtensionElement is ExtensionExecutor ExtensionExecutor) 
                    {
                        return ExtensionExecutor.Action(leftVariableExpression, functionCallExpression.Arguments);
                    }
                    if (ExtensionElement is FunctionInstance FunctionInstance)
                    {
                        List<Argument> Arguments = new()
                        {
                            new("var", TypesType.Any, leftVariableExpression, true, false)
                        };
                        foreach (Argument Argument in functionCallExpression.Arguments)
                        {
                            Arguments.Add(Argument);
                        }
                        return FunctionInstance.ExecuteAndReturn(Arguments.ToArray());
                    }
                }
                */
            }

            var member = GetMemberWithKeyOfStructure(this);

            return member.Value;
        }

        private Value More(Value left, Value right)
        {
            return new BooleanValue(ExpressionInfo, left.AsNumber() > right.AsNumber());
        }

        private Value Less(Value left, Value right)
        {
            return new BooleanValue(ExpressionInfo, left.AsNumber() < right.AsNumber());
        }

        private Value Or(Value left, Value right)
        {
            return new BooleanValue(ExpressionInfo, left.AsBoolean() || right.AsBoolean());
        }

        private Value And(Value left, Value right)
        {
            return new BooleanValue(ExpressionInfo, left.AsBoolean() && right.AsBoolean());
        }

        private Value Is(Value left, Value right)
        {
            if (right is TypeValue typeValue)
            {
                return left is StructureValue structureValue
                    ? new BooleanValue(ExpressionInfo, typeValue.TypeName == structureValue.Structure.Name)
                    : (Value)new BooleanValue(ExpressionInfo, typeValue.Value == left.Type);
            }

            return right is NoneValue noneValue
                ? new BooleanValue(ExpressionInfo, noneValue.Type == left.Type)
                : left.GetType() != right.GetType()
                ? new BooleanValue(ExpressionInfo, false)
                : (Value)(left.Type switch
                {
                    TypesType.Any => new BooleanValue(ExpressionInfo, left.AsString() == right.AsString()),
                    TypesType.Number => new BooleanValue(ExpressionInfo, left.AsNumber() == right.AsNumber()),
                    TypesType.String => new BooleanValue(ExpressionInfo, left.AsString() == right.AsString()),
                    TypesType.Boolean => new BooleanValue(ExpressionInfo, left.AsBoolean() == right.AsBoolean()),
                    TypesType.Char => new BooleanValue(ExpressionInfo, left.AsString() == right.AsString()),
                    TypesType.Enum => new BooleanValue(ExpressionInfo, (left as EnumValue).Member == (right as EnumValue).Member),
                    TypesType.None => new BooleanValue(ExpressionInfo, left.Name == right.Name),
                    TypesType.Structure => new BooleanValue(ExpressionInfo, left == right),
                    _ => throw new InterpreterException($"You cant check type {left.Type} and {right.Type}", ExpressionInfo),
                });
        }

        public Value Minus(Value left, Value right)
        {
            return left.Type switch
            {
                TypesType.Any => new NumberValue(ExpressionInfo, left.AsNumber() - right.AsNumber()),
                TypesType.Number => new NumberValue(ExpressionInfo, left.AsNumber() - right.AsNumber()),
                _ => throw new InterpreterException($"You cant substraction type {left.Type} and {right.Type}", ExpressionInfo),
            };
        }

        public Value Addition(Value left, Value right)
        {
            return left.Type switch
            {
                TypesType.Any => new NumberValue(ExpressionInfo, left.AsNumber() + right.AsNumber()),
                TypesType.Number => new NumberValue(ExpressionInfo, left.AsNumber() + right.AsNumber()),
                TypesType.String => new StringValue(ExpressionInfo, left.AsString() + right.AsString()),
                TypesType.Type => new StringValue(ExpressionInfo, left.AsString() + right.AsString()),
                TypesType.None => new StringValue(ExpressionInfo, left.AsString() + right.AsString()),
                _ => throw new InterpreterException($"You cant addition type {left.Type} and {right.Type}", ExpressionInfo),
            };
        }

        public Value Multiplicative(Value left, Value right)
        {
            return left.Type switch
            {
                TypesType.Any => new NumberValue(ExpressionInfo, left.AsNumber() * right.AsNumber()),
                TypesType.Number => new NumberValue(ExpressionInfo, left.AsNumber() * right.AsNumber()),
                _ => throw new InterpreterException($"You cant multiplicative type {left.Type} and {right.Type}", ExpressionInfo),
            };
        }

        public Value Division(Value left, Value right)
        {
            return left.Type switch
            {
                TypesType.Any => new NumberValue(ExpressionInfo, left.AsNumber() / right.AsNumber()),
                TypesType.Number => new NumberValue(ExpressionInfo, left.AsNumber() / right.AsNumber()),
                TypesType.Boolean => new NumberValue(ExpressionInfo, left.AsNumber() / right.AsNumber()),
                _ => throw new InterpreterException($"You cant division type {left.Type} and {right.Type}", ExpressionInfo),
            };
        }

        public Value Assign()
        {
            Value value = null;

            if (Right is FunctionDeclarateExpression function)
            {
                value = Value.Create(function);
            }
            else
            {
                value = Right.Evaluate();
            }

            if (Left is VariableExpression variableExpression)
            {
                if (!Variables.Instance.TryGet(ExpressionInfo.Parent, variableExpression.Name, out var result, ExpressionInfo.EmptyInfo))
                {
                    if (variableExpression.Type is null)
                    {
                        variableExpression = new VariableExpression(ExpressionInfo,
                            variableExpression.Name, new TypeValue(ExpressionInfo, value.Type, value is TypeValue typeValue ? typeValue.TypeName : string.Empty));
                    }
                }
                else
                {
                    if (value.Type != result.Type)
                    {
                        throw new InterpreterException($"Except {result.Type} type",
                            variableExpression.ExpressionInfo);
                    }

                    if (result is StructureValue structureValue1 && value is StructureValue structureValue && structureValue1.Structure.Name != structureValue.Structure.Name)
                    {
                        throw new InterpreterException($"Except {structureValue1.Structure.Name} structure type",
                            variableExpression.ExpressionInfo);
                    }
                }

                if (value is FunctionValue)
                {
                    Functions.Instance.Set(variableExpression.ExpressionInfo.Parent, variableExpression.Name, new FunctionInstance(Right as FunctionDeclarateExpression));
                }

                Variables.Instance.Set(variableExpression.ExpressionInfo.Parent, variableExpression.Name, value);
            }
            else if (Left is BinaryOperatorExpression binary)
            {
                var structure = GetStructure(binary);
                var member = GetMemberWithKeyOfStructure(binary);

                structure.Set(member.Key, value, ExpressionInfo.Filepath);
            }
            else if (Left is ArrayElementExpression arrayElementExpression)
            {
                var variable2 = Variables.Instance.Get(ExpressionInfo.Parent, arrayElementExpression.Name, arrayElementExpression.ExpressionInfo);

                if (variable2 is not NoneValue)
                {
                    var array = variable2 as ArrayValue;

                    var eval = arrayElementExpression.EvalWithKey();

                    if (eval.Value is NoneValue)
                    {
                        array.Set(eval.Key, value);
                    }
                    else
                    {
                        eval.Value.Set(value);
                    }
                }

                return variable2;
            }

            return value;
        }
    }
}
