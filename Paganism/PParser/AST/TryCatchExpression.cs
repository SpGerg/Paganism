using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class TryCatchExpression : EvaluableExpression, IStatement, IExecutable
    {
        public TryCatchExpression(ExpressionInfo info, BlockStatementExpression tryExpression, BlockStatementExpression catchExpression) : base(info)
        {
            TryExpression = tryExpression;
            CatchExpression = catchExpression;
        }

        public BlockStatementExpression TryExpression { get; }

        public BlockStatementExpression CatchExpression { get; }

        public override Value Evaluate(params Argument[] arguments)
        {
            try
            {
                var result = TryExpression.Evaluate(arguments);

                return result;
            }
            catch (Exception exception)
            {
                var structure = new StructureValue(ExpressionInfo, Interpreter.Data.Structures.Instance.Value.Get(null, "exception"));
                structure.Set("name", new StringValue(ExpressionInfo, exception.GetType().Name), ExpressionInfo.Filepath);
                structure.Set("description", new StringValue(ExpressionInfo, exception.Message), ExpressionInfo.Filepath);

                Variables.Instance.Value.Set(CatchExpression, "exception", structure);

                return CatchExpression.Evaluate();
            }
        }

        public void Execute(params Argument[] arguments)
        {
            Evaluate(arguments);
        }
    }
}
