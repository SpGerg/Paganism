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
            Interpreter.Data.Structures.Instance.Set(ExpressionInfo, ExpressionInfo.Parent, Name, new StructureInstance(this));
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
