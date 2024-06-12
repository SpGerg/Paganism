using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class BlockStatementExpression : EvaluableExpression, IStatement, IExecutable
    {
        public BlockStatementExpression(ExpressionInfo info, IStatement[] statements, bool isLoop = false, bool isClearing = true) : base(info)
        {
            Statements = statements;
            IsLoop = isLoop;
            IsClearing = isClearing;
        }

        public IStatement[] Statements { get; set; }

        public bool IsLoop { get; }

        public bool IsClearing { get; set; }

        public bool IsBreaked { get; private set; }

        public void Execute(params Argument[] arguments)
        {
            Evaluate(arguments);
        }

        public override Value Evaluate(params Argument[] arguments)
        {
            if (Statements is null)
            {
                return new VoidValue(ExpressionInfo);
            }

            IsBreaked = false;

            Value result = new VoidValue(ExpressionInfo);

            foreach (var statement in Statements)
            {
                if (IsBreaked)
                {
                    break;
                }

                switch (statement)
                {
                    case ReturnExpression returnExpression:
                        result = returnExpression.Value.Evaluate();
                        IsBreaked = true;
                        break;
                    case UnaryExpression unaryExpression:
                        unaryExpression.Evaluate();
                        break;
                    case BreakExpression:
                        IsBreaked = IsLoop;

                        if (IsLoop)
                        {
                            return new VoidValue(ExpressionInfo);
                        }

                        break;
                    case AwaitExpression awaitExpression:
                        awaitExpression.Execute();
                        break;
                    case BinaryOperatorExpression binaryOperatorExpression:
                        binaryOperatorExpression.Evaluate();
                        break;
                    case IDeclaratable declaratable:
                        declaratable.Declarate();
                        break;
                    case TryCatchExpression tryCatchExpression:
                        var value2 = tryCatchExpression.Evaluate();

                        if (IsLoop)
                        {
                            IsBreaked = tryCatchExpression.TryExpression.IsBreaked || tryCatchExpression.CatchExpression.IsBreaked;
                            break;
                        }

                        if (value2 is not null)
                        {
                            IsBreaked = true;
                            return value2;
                        }

                        break;
                    case IfExpression ifExpression:
                        var value = ifExpression.Evaluate();

                        if (IsLoop)
                        {
                            IsBreaked = ifExpression.BlockStatement.IsBreaked || ifExpression.ElseBlockStatement.IsBreaked;
                            break;
                        }

                        if (value is not null)
                        {
                            IsBreaked = true;
                            return value;
                        }

                        break;
                    case FunctionCallExpression functionCallExpression:
                        functionCallExpression.Execute();
                        break;
                    case LoopExpression loopExpression:          
                        var result2 = loopExpression.Evaluate();

                        if (result2 is not VoidValue)
                        {
                            IsBreaked = true;
                            return result2;
                        }

                        break;
                }
            }

            if (IsClearing)
            {
                Variables.Instance.Clear(this);
                Functions.Instance.Clear(this);
                Interpreter.Data.Enums.Instance.Clear(this);
                Interpreter.Data.Structures.Instance.Clear(this);
            }

            return result;
        }
    }
}
