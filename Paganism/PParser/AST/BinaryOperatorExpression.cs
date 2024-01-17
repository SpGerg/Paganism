using Paganism.Exceptions;
using Paganism.Interpreter.Data;
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
            if (right is not TypeValue typeValue)
            {
                throw new InterpreterException("Right expression must be a type", Line, Position);
            }

            return typeValue.Value switch
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
                var name = (binary.Right as VariableExpression).Name.Replace("()", string.Empty);
                var member = structure.Values[name] as StructureValue;

                if (!structure.Structure.Members[name].IsShow && structure.Structure.StructureDeclarateExpression.Filepath != binary.Filepath)
                {
                    throw new InterpreterException($"You cant access to structure member '{name}' in '{structure.Structure.Name}' structure", binary.Line, binary.Position);
                }

                return member;
            }

            return null;
        }

        public static KeyValuePair<string, Value> GetMemberWithKeyOfStructure(BinaryOperatorExpression binaryOperatorExpression)
        {
            var structure = GetStructure(binaryOperatorExpression);
            var name = (binaryOperatorExpression.Right as VariableExpression).Name.Replace("()", string.Empty);
            var member = structure.Values[name];

            if (!structure.Structure.Members[name].IsShow && structure.Structure.StructureDeclarateExpression.Filepath != binaryOperatorExpression.Filepath)
            {
                throw new InterpreterException($"You cant access to structure member '{name}' in '{structure.Structure.Name}' structure", binaryOperatorExpression.Line, binaryOperatorExpression.Position);
            }

            if (structure is null)
            {
                throw new InterpreterException("Structure is null", binaryOperatorExpression.Line, binaryOperatorExpression.Position);
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
                if (left is StructureValue structureValue)
                {
                    return new BooleanValue(typeValue.StructureName == structureValue.Structure.Name);
                }

                return new BooleanValue(typeValue.Value == left.Type);
            }

            if (right is NoneValue noneValue)
            {
                return new BooleanValue(noneValue.Type == left.Type);
            }

            if (left.GetType() != right.GetType()) return new BooleanValue(false);

            return left.Type switch
            {
                TypesType.Any => new BooleanValue(left.AsString() == right.AsString()),
                TypesType.Number => new BooleanValue(left.AsNumber() == right.AsNumber()),
                TypesType.String => new BooleanValue(left.AsString() == right.AsString()),
                TypesType.Boolean => new BooleanValue(left.AsBoolean() == right.AsBoolean()),
                TypesType.Char => new BooleanValue(left.AsString() == right.AsString()),
                TypesType.None => new BooleanValue(left.Name == right.Name),
                TypesType.Structure => new BooleanValue(left == right),
                _ => throw new InterpreterException($"You cant check type {left.Type} and {right.Type}"),
            };
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
            if (Left is VariableExpression variableExpression)
            {
                Variables.Instance.Value.Add(variableExpression.Parent, variableExpression.Name, Right.Eval());
            }
            else if (Left is BinaryOperatorExpression binary)
            {
                var structure = GetStructure(binary);
                var member = GetMemberWithKeyOfStructure(binary);

                structure.Set(member.Key, Right.Eval());
            }

            return Right.Eval();
        }
    }
}
