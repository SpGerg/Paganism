using Paganism.PParser;
using Paganism.PParser.Values;
using System.Linq;

namespace Paganism.Interpreter.Data.Instances
{
    public abstract class Instance
    {
        public abstract string InstanceName { get; }

        public Instance Create(Value value)
        {
            switch (value)
            {
                case FunctionValue functionValue:
                    return new FunctionInstance(new PParser.AST.FunctionDeclarateExpression(new ExpressionInfo(),
                        functionValue.Name, functionValue.Value.Statement, functionValue.Value.RequiredArguments,
                        functionValue.Value.IsAsync, functionValue.Value.IsShow, functionValue.Value.ReturnType));
                case StructureValue structureValue:
                    return new StructureInstance(new PParser.AST.StructureDeclarateExpression(new ExpressionInfo(), 
                        structureValue.Structure.Name, structureValue.Structure.Members.Values.ToArray()));
            }

            return null;
        }
    }
}
