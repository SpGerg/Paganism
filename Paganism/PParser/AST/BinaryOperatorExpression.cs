using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class BinaryOperatorExpression : Expression, IEvaluable
    {
        public BinaryOperatorExpression(BinaryOperatorType type, IEvaluable left, IEvaluable right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public BinaryOperatorType Type { get; }

        public IEvaluable Left { get; }

        public IEvaluable Right { get; }

        public static KeyValuePair<string, Value> GetStructure(Expression left, Expression right, StructureValue parent)
        {
            if (right is BinaryOperatorExpression binary)
            {
                var structure = parent.Values[(binary.Left as VariableExpression).Name] as StructureValue;

                return GetStructure(binary.Left as Expression, binary.Right as Expression, structure);
            }

            var name = (left as VariableExpression).Name;

            return new KeyValuePair<string, Value>(name, parent);
        }

        public Value Eval()
        {
            if (Type == BinaryOperatorType.Point) return Point();

            var left = Left.Eval();
            var right = Right.Eval();

            return Type switch
            {
                BinaryOperatorType.Plus => Addition(left, right),
                BinaryOperatorType.Minus => Minus(left, right),
                BinaryOperatorType.Multiplicative => Addition(left, right),
                BinaryOperatorType.Division => Division(left, right),
                BinaryOperatorType.Assign => Assign(left, right),
                BinaryOperatorType.Is => Is(left, right),
                BinaryOperatorType.And => And(left, right),
                BinaryOperatorType.Or => Or(left, right),
                BinaryOperatorType.Less => Less(left, right),
                BinaryOperatorType.More => More(left, right),
                _ => null,
            };
        }

        private Value Point()
        {
            if (Left is BinaryOperatorExpression binaryOperator)
            {
                return GetMember((Left as VariableExpression).Eval() as StructureValue, binaryOperator).Value;
            }

            return GetMember((Left as VariableExpression).Eval() as StructureValue, Right as Expression).Value;
        }

        public KeyValuePair<string, Value> PointKeyValuePair()
        {
            if (Left is BinaryOperatorExpression binaryOperator)
            {
                var leftKey = (binaryOperator.Left as VariableExpression).Name;

                return GetMember(Variables.Get(leftKey) as StructureValue, binaryOperator);
            }

            var key = (Left as VariableExpression).Name;
            var member = GetMember((Variables.Get(key) as StructureValue), Right as Expression);

            return new KeyValuePair<string, Value>(member.Key, member.Value);
        }

        private KeyValuePair<string, Value> GetMember(StructureValue structure, Expression expression)
        {
            if (structure == null)
            {
                throw new InterpreterException("Structure is none");
            }

            if (expression is not BinaryOperatorExpression binaryOperator)
            {
                var name = (expression as VariableExpression).Name;

                if (!structure.Values.TryGetValue(name, out var value))
                {
                    throw new InterpreterException($"Structure member with {name} name in '{structure.Structure.Name}' structure not found");
                }

                return new KeyValuePair<string, Value>(name, value);
            }

            if (binaryOperator.Right is not BinaryOperatorExpression binaryOperatorRight)
            {
                var name2 = (binaryOperator.Left as VariableExpression).Name;
                var value = structure.Values[(binaryOperator.Left as VariableExpression).Name];

                if (value is StructureValue structureValue)
                {
                    if (!structureValue.Values.TryGetValue(name2, out var value2))
                    {
                        throw new InterpreterException($"Structure member with {name2} name in '{structure.Structure.Name}' structure not found");
                    }

                    return new KeyValuePair<string, Value>(name2, value2);
                }

                return new KeyValuePair<string, Value>(name2, value);
            }

            var name3 = (binaryOperator.Left as VariableExpression).Name;

            if (!structure.Values.TryGetValue(name3, out var value3))
            {
                throw new InterpreterException($"Structure member with {name3} name in '{structure.Structure.Name}' structure not found");
            }

            return GetMember(value3 as StructureValue, binaryOperatorRight);
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

        public Value Assign(Value left, Value right)
        {
            if (Left is not VariableExpression)
            {
                throw new InterpreterException("Except variable");
            }

            return Right.Eval();
        }
    }
}
