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
        public TryCatchExpression(BlockStatementExpression parent, int position, int line, string filepath, BlockStatementExpression tryExpression, BlockStatementExpression catchExpression) : base(parent, position, line, filepath)
        {
            TryExpression = tryExpression;
            CatchExpression = catchExpression;
        }

        public BlockStatementExpression TryExpression { get; }

        public BlockStatementExpression CatchExpression { get; }

        public override Value Eval(params Argument[] arguments)
        {
            try
            {
                var result = TryExpression.Eval(arguments);

                return result;
            }
            catch (Exception exception)
            {
                var structure = new StructureValue(Parent, Interpreter.Data.Structures.Instance.Value.Get(null, "exception"));
                structure.Set("name", new StringValue(exception.GetType().Name), Filepath);
                structure.Set("description", new StringValue(exception.Message), Filepath);

                Variables.Instance.Value.Set(CatchExpression, "exception", structure);

                return CatchExpression.Eval();
            }
        }

        public void Execute(params Argument[] arguments)
        {
            Eval(arguments);
        }
    }
}
