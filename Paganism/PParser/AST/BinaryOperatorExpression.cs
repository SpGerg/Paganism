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

        public Value Eval()
        {
            var left = Left.Eval();
            var right = Right.Eval();

            if (left == null)
            {
                throw new Exception("Left expression is null");
            }

            if (right == null)
            {
                throw new Exception("Right expression is null");
            }

            if (left.Type != right.Type)
            {
                throw new InvalidOperationException($"Invalid calculate {left.Type} and {right.Type}");
            }

            switch (Type)
            {
                case BinaryOperatorType.Plus:
                    return Addition(left, right);
                case BinaryOperatorType.Minus:
                    return Minus(left, right);
                case BinaryOperatorType.Multiplicative:
                    return Addition(left, right);
                case BinaryOperatorType.Division:
                    return Division(left, right);
                case BinaryOperatorType.Assign:
                    return Assign(left, right);
                case BinaryOperatorType.Is:
                    return Is(left, right);
            }

            return null;
        }

        private Value Is(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Any:
                    return new BooleanValue(left.AsNumber() == right.AsNumber());
                case StandartValueType.Number:
                    return new BooleanValue(left.AsNumber() == right.AsNumber());
                case StandartValueType.String:
                    return new BooleanValue(left.AsString() == right.AsString());
                case StandartValueType.Boolean:
                    return new BooleanValue(left.AsBoolean() == right.AsBoolean());
            }

            throw new Exception($"You cant substraction type {left.Type} and {right.Type}");
        }

        public Value Minus(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Any:
                    return new NumberValue(left.AsNumber() - right.AsNumber());
                case StandartValueType.Number:
                    return new NumberValue(left.AsNumber() - right.AsNumber());
                    /*
                case StandartValueType.String:
                    return new StringValue(left.AsString() + right.AsString());
                case StandartValueType.Boolean:
                    return new NumberValue(left.AsNumber() + right.AsNumber());
                    */
            }

            throw new Exception($"You cant substraction type {left.Type} and {right.Type}");
        }

        public Value Addition(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Any:
                    return new NumberValue(left.AsNumber() + right.AsNumber());
                case StandartValueType.Number:
                    return new NumberValue(left.AsNumber() + right.AsNumber());
                case StandartValueType.String:
                    return new StringValue(left.AsString() + right.AsString());
            }

            throw new Exception($"You cant addition type {left.Type} and {right.Type}");
        }

        public Value Multiplicative(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Any:
                    return new NumberValue(left.AsNumber() * right.AsNumber());
                case StandartValueType.Number:
                    return new NumberValue(left.AsNumber() * right.AsNumber());
            }

            throw new Exception($"You cant multiplicative type {left.Type} and {right.Type}");
        }

        public Value Division(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Any:
                    return new NumberValue(left.AsNumber() / right.AsNumber());
                case StandartValueType.Number:
                    return new NumberValue(left.AsNumber() / right.AsNumber());
                case StandartValueType.Boolean:
                    return new NumberValue(left.AsNumber() / right.AsNumber());
            }

            throw new Exception($"You cant division type {left.Type} and {right.Type}");
        }

        public Value Assign(Value left, Value right)
        {
            if (Left is not VariableExpression variableExpression)
            {
                throw new Exception("Except variable");
            }

            if (Right is FunctionCallExpression functionCall)
            {
                var function = Functions.Get(functionCall.FunctionName);

                if (function == null)
                {
                    throw new Exception($"Function with {functionCall.FunctionName} name not found");
                }

                if (function.ReturnTypes.Length <= 0)
                {
                    throw new Exception("Function return void");
                }

                return functionCall.Eval();
            }

            throw new Exception("Except variable or function call");
        }
    }
}
