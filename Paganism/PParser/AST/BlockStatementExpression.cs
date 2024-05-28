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
            if (Statements == null)
            {
                return null;
            }

            IsBreaked = false;

            Value result = null;

            for (int i = 0; i < Statements.Length; i++)
            {
                if (IsBreaked)
                {
                    break;
                }

                var statement = Statements[i];

                switch (statement)
                {
                    case ReturnExpression returnExpression:
                        result = returnExpression.Value.Evaluate();
                        IsBreaked = true;
                        break;
                    case UnaryExpression unaryExpression:
                        unaryExpression.Evaluate();
                        break;
                    case WhileExpression whileExpression:
                        whileExpression.Execute();
                        break;
                    case BreakExpression breakExpression:
                        breakExpression.IsLoop = IsLoop;
                        breakExpression.Execute();

                        if (breakExpression.IsBreaked)
                        {
                            i = Statements.Length;
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

                        if (IsLoop && (tryCatchExpression.TryExpression.IsBreaked || tryCatchExpression.CatchExpression.IsBreaked))
                        {
                            IsBreaked = true;
                            break;
                        }

                        if (value2 != null)
                        {
                            IsBreaked = true;
                            return value2;
                        }

                        break;
                    case IfExpression ifExpression:
                        var value = ifExpression.Evaluate();

                        if (IsLoop && (ifExpression.BlockStatement.IsBreaked || ifExpression.ElseBlockStatement.IsBreaked))
                        {
                            IsBreaked = true;
                            break;
                        }

                        if (value != null)
                        {
                            IsBreaked = true;
                            return value;
                        }

                        break;
                    case FunctionCallExpression functionCallExpression:
                        functionCallExpression.Execute();
                        break;
                    case ForExpression forExpression:
                        var variable = forExpression.Variable as AssignExpression;

                        if (variable != null)
                        {
                            (variable.Left as VariableExpression).Set(ExpressionInfo, variable.Right.Evaluate());
                        }

                        var result2 = forExpression.Evaluate();

                        Variables.Instance.Remove(forExpression.ExpressionInfo.Parent, (variable.Left as VariableExpression).Name);

                        if (result2 is not NoneValue)
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
