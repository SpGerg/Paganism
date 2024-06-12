using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class StructureDeclarateExpression : EvaluableExpression, IStatement, IDeclaratable, IAccessible
    {
        public StructureDeclarateExpression(ExpressionInfo info, string name, StructureMemberExpression[] members, InstanceInfo instanceInfo) : base(info)
        {
            Name = name;
            Members = members;
            Info = instanceInfo;
        }

        public string Name { get; }

        public StructureMemberExpression[] Members { get; }

        public InstanceInfo Info { get; }

        public void Declarate()
        {
            Declarate(Name);
        }

        public void Declarate(string name)
        {
            var structureInstance = new StructureInstance(this);

            Interpreter.Data.Structures.Instance.Set(ExpressionInfo, ExpressionInfo.Parent, name, structureInstance);
            Variables.Instance.Set(ExpressionInfo, ExpressionInfo.Parent, name,
                new VariableInstance(new InstanceInfo(true, true, string.Empty),
                new StructureValue(ExpressionInfo, this),
                structureInstance.StructureDeclarateExpression.GetTypeValue()));
        }

        public void Remove()
        {
            Interpreter.Data.Structures.Instance.Remove(ExpressionInfo.Parent, Name);
        }

        public override Value Evaluate(params Argument[] arguments)
        {
            return new StructureValue(ExpressionInfo, new StructureInstance(this));
        }
    }
}
