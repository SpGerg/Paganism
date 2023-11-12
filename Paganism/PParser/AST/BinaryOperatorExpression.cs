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

            if (left.Type != right.Type)
            {
                throw new InvalidOperationException($"Invalid calculate {left.Type} and {right.Type}");
            }

            switch (Type)
            {
                case BinaryOperatorType.Plus:
                    return Addition(left, right);
                case BinaryOperatorType.Minus:
                    return Multiplicative(left, right);
                case BinaryOperatorType.Multiplicative:
                    return Addition(left, right);
                case BinaryOperatorType.Division:
                    return Addition(left, right);
            }

            return null;
        }

        public Value Minus(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Number:
                    return new NumberValue(left.AsNumber() - right.AsNumber());
                    /*
                case StandartValueType.String:
                    return new StringValue(left.AsString() + right.AsString());
                case StandartValueType.Boolean:
                    return new NumberValue(left.AsNumber() + right.AsNumber());
                    */
            }

            return null;
        }

        public Value Addition(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Number:
                    return new NumberValue(left.AsNumber() + right.AsNumber());
                    /*
                case StandartValueType.String:
                    return new StringValue(left.AsString() + right.AsString());
                case StandartValueType.Boolean:
                    return new NumberValue(left.AsNumber() + right.AsNumber());
                    */
            }

            return null;
        }

        public Value Multiplicative(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Number:
                    return new NumberValue(left.AsNumber() * right.AsNumber());
                    /*
                case StandartValueType.String:
                    return new StringValue(left.AsString() + right.AsString());
                case StandartValueType.Boolean:
                    return new NumberValue(left.AsNumber() + right.AsNumber());
                    */
            }

            return null;
        }

        public Value Division(Value left, Value right)
        {
            switch (left.Type)
            {
                case StandartValueType.Number:
                    return new NumberValue(left.AsNumber() / right.AsNumber());
                    /*
                case StandartValueType.String:
                    return new StringValue(left.AsString() + right.AsString());
                case StandartValueType.Boolean:
                    return new NumberValue(left.AsNumber() + right.AsNumber());
                    */
            }

            return null;
        }
    }
}
