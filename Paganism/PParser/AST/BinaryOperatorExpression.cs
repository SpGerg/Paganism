using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using System.Collections.Generic;

namespace Paganism.PParser.AST
{
    public class BinaryOperatorExpression : EvaluableExpression
    {
        public BinaryOperatorExpression(BlockStatementExpression parent, int line, int position, string filepath, BinaryOperatorType type, EvaluableExpression left, EvaluableExpression right) : base(parent, line, position, filepath)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public BinaryOperatorType Type { get; }

        public EvaluableExpression Left { get; }

        public EvaluableExpression Right { get; }

        public override Value Eval(params Argument[] arguments)
        {
            if (Type == BinaryOperatorType.Point)
            {
                return Point();
            }
            else if (Type == BinaryOperatorType.Assign)
            {
                return Assign();
            }

            var left = Left.Eval();
            var right = Right.Eval();

            return Type switch
            {
                BinaryOperatorType.Plus => Addition(left, right),
                BinaryOperatorType.Minus => Minus(left, right),
                BinaryOperatorType.Multiplicative => Addition(left, right),
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

        private Value As(Value left, Value right)
        {
            return right is not TypeValue typeValue
                ? throw new InterpreterException("Right expression must be a type", Line, Position)
                : typeValue.Value switch
                {
                    TypesType.Any => new StringValue(left.AsString()),
                    TypesType.Number => new NumberValue(left.AsNumber()),
                    TypesType.String => new StringValue(left.AsString()),
                    TypesType.Boolean => new BooleanValue(left.AsBoolean()),
                    TypesType.Char => AsChar(left, right),
                    TypesType.None => new NoneValue(),
                    TypesType.Structure => AsStructure(left, typeValue),
                    _ => throw new InterpreterException($"You cant check type {left.Type} and {right.Type}"),
                };
        }

        private Value AsStructure(Value left, TypeValue right)
        {
            if (left is not StructureValue structureValue || right.StructureName == string.Empty || right.StructureName is null)
            {
                throw new InterpreterException($"Cannot cast {left.Type} to Structure", Line, Position);
            }

            foreach (var member in structureValue.Structure.Members)
            {
                if (!member.Value.IsCastable)
                {
                    continue;
                }

                var value = structureValue.Values[member.Key];

                if (value is not StructureValue structureValue1)
                {
                    continue;
                }

                if (structureValue1.Structure.Name != right.StructureName)
                {
                    continue;
                }

                return structureValue1;
            }

            throw new InterpreterException($"Structure with '{structureValue.Structure.Name}' name havent castable member with '{right.StructureName}' type", Line, Position);
        }

        private Value AsChar(Value left, Value right)
        {
            return (left is StringValue stringValue && stringValue.Value.Length == 1) ? new CharValue(left.AsString()[0]) : throw new InterpreterException("Cannot cast string to char. String must be contains only one character.", Line, Position);
        }

        public static StructureValue GetStructure(BinaryOperatorExpression binaryOperatorExpression)
        {
            if (binaryOperatorExpression.Left is VariableExpression variableExpression)
            {
                var left = Variables.Instance.Value.Get(binaryOperatorExpression.Parent, variableExpression.Name) as StructureValue;

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

                if (!structure.Structure.Members[name].IsShow && structure.Structure.StructureDeclarateExpression.Filepath != binary.Filepath)
                {
                    throw new InterpreterException($"You cant access to structure member '{name}' in '{structure.Structure.Name}' structure", binary.Line, binary.Position);
                }

                var member = structure.Values[name];

                if (member is StructureValue structureValue)
                {
                    return structureValue;
                }
                else if (member is FunctionValue functionValue && binary.Right is FunctionCallExpression functionCallExpression)
                {
                    var value = functionValue.Value.Eval(functionCallExpression.Arguments);

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

            var member = structure.Values[name];

            if (!structure.Structure.Members[name].IsShow && structure.Structure.StructureDeclarateExpression.Filepath != binaryOperatorExpression.Filepath)
            {
                throw new InterpreterException($"You cant access to structure member '{name}' in '{structure.Structure.Name}' structure", binaryOperatorExpression.Line, binaryOperatorExpression.Position);
            }

            if (structure is null)
            {
                throw new InterpreterException("Structure is null", binaryOperatorExpression.Line, binaryOperatorExpression.Position);
            }

            if (member is FunctionValue functionValue && binaryOperatorExpression.Right is FunctionCallExpression callExpression)
            {
                return new KeyValuePair<string, Value>(name, functionValue.Value.Eval(callExpression.Arguments));
            }

            return new KeyValuePair<string, Value>(name, member);
        }

        public static Value GetMemberOfStructure(BinaryOperatorExpression binaryOperatorExpression)
        {
            return GetMemberWithKeyOfStructure(binaryOperatorExpression).Value;
        }

        private Value Point()
        {
            var member = GetMemberOfStructure(this);
            return member;
        }

        private Value More(Value left, Value right)
        {
            return new BooleanValue(left.AsNumber() > right.AsNumber());
        }

        private Value Less(Value left, Value right)
        {
            return new BooleanValue(left.AsNumber() < right.AsNumber());
        }

        private Value Or(Value left, Value right)
        {
            return new BooleanValue(left.AsBoolean() || right.AsBoolean());
        }

        private Value And(Value left, Value right)
        {
            return new BooleanValue(left.AsBoolean() && right.AsBoolean());
        }

        private Value Is(Value left, Value right)
        {
            if (right is TypeValue typeValue)
            {
                return left is StructureValue structureValue
                    ? new BooleanValue(typeValue.StructureName == structureValue.Structure.Name)
                    : (Value)new BooleanValue(typeValue.Value == left.Type);
            }

            return right is NoneValue noneValue
                ? new BooleanValue(noneValue.Type == left.Type)
                : left.GetType() != right.GetType()
                ? new BooleanValue(false)
                : (Value)(left.Type switch
                {
                    TypesType.Any => new BooleanValue(left.AsString() == right.AsString()),
                    TypesType.Number => new BooleanValue(left.AsNumber() == right.AsNumber()),
                    TypesType.String => new BooleanValue(left.AsString() == right.AsString()),
                    TypesType.Boolean => new BooleanValue(left.AsBoolean() == right.AsBoolean()),
                    TypesType.Char => new BooleanValue(left.AsString() == right.AsString()),
                    TypesType.None => new BooleanValue(left.Name == right.Name),
                    TypesType.Structure => new BooleanValue(left == right),
                    _ => throw new InterpreterException($"You cant check type {left.Type} and {right.Type}"),
                });
        }

        public Value Minus(Value left, Value right)
        {
            return left.Type switch
            {
                TypesType.Any => new NumberValue(left.AsNumber() - right.AsNumber()),
                TypesType.Number => new NumberValue(left.AsNumber() - right.AsNumber()),
                _ => throw new InterpreterException($"You cant substraction type {left.Type} and {right.Type}"),
            };
        }

        public Value Addition(Value left, Value right)
        {
            return left.Type switch
            {
                TypesType.Any => new NumberValue(left.AsNumber() + right.AsNumber()),
                TypesType.Number => new NumberValue(left.AsNumber() + right.AsNumber()),
                TypesType.String => new StringValue(left.AsString() + right.AsString()),
                TypesType.Type => new StringValue(left.AsString() + right.AsString()),
                _ => throw new InterpreterException($"You cant addition type {left.Type} and {right.Type}"),
            };
        }

        public Value Multiplicative(Value left, Value right)
        {
            return left.Type switch
            {
                TypesType.Any => new NumberValue(left.AsNumber() * right.AsNumber()),
                TypesType.Number => new NumberValue(left.AsNumber() * right.AsNumber()),
                _ => throw new InterpreterException($"You cant multiplicative type {left.Type} and {right.Type}"),
            };
        }

        public Value Division(Value left, Value right)
        {
            return left.Type switch
            {
                TypesType.Any => new NumberValue(left.AsNumber() / right.AsNumber()),
                TypesType.Number => new NumberValue(left.AsNumber() / right.AsNumber()),
                TypesType.Boolean => new NumberValue(left.AsNumber() / right.AsNumber()),
                _ => throw new InterpreterException($"You cant division type {left.Type} and {right.Type}"),
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
                value = Right.Eval();
            }

            if (Left is VariableExpression variableExpression)
            {
                if (variableExpression.Type is null)
                {
                    variableExpression = new VariableExpression(variableExpression.Parent, variableExpression.Line, variableExpression.Position, variableExpression.Filepath,
                        variableExpression.Name, new TypeValue(value.Type, value is TypeValue typeValue ? typeValue.StructureName : string.Empty));
                }
                else
                {
                    if (value.Type != variableExpression.Type.Value)
                    {
                        throw new InterpreterException($"Except {variableExpression.Type.Value} type", variableExpression.Line, variableExpression.Position);
                    }

                    if (variableExpression.Type.Value is TypesType.Structure && value is StructureValue structureValue && variableExpression.Type.StructureName != structureValue.Structure.Name)
                    {
                        throw new InterpreterException($"Except {variableExpression.Type.StructureName} structure type", variableExpression.Line, variableExpression.Position);
                    }
                }

                if (value is FunctionValue)
                {
                    Functions.Instance.Value.Add(variableExpression.Parent, variableExpression.Name, new FunctionInstance(Right as FunctionDeclarateExpression));
                }
                else
                {
                    Variables.Instance.Value.Add(variableExpression.Parent, variableExpression.Name, value);
                }
            }
            else if (Left is BinaryOperatorExpression binary)
            {
                var structure = GetStructure(binary);
                var member = GetMemberWithKeyOfStructure(binary);

                structure.Set(member.Key, value);
            }
            else if (Left is ArrayElementExpression arrayElementExpression)
            {
                var variable2 = Variables.Instance.Value.Get(Parent, arrayElementExpression.Name);

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

            return Right.Eval();
        }
    }
}
