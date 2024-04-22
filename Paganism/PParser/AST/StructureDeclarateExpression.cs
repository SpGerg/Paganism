using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;

namespace Paganism.PParser.AST
{
    public class StructureDeclarateExpression : EvaluableExpression, IStatement, IDeclaratable
    {
        public StructureDeclarateExpression(ExpressionInfo info, string name, StructureMemberExpression[] members) : base(info)
        {
            Name = name;
            Members = members;
        }

        public string Name { get; }

        public StructureMemberExpression[] Members { get; }

        public void Declarate()
        {
            Interpreter.Data.Structures.Instance.Value.Set(ExpressionInfo.Parent, Name, new StructureInstance(this));
        }

        public void Remove()
        {
            Interpreter.Data.Structures.Instance.Value.Remove(ExpressionInfo.Parent, Name);
        }

        public override Value Evaluate(params Argument[] arguments)
        {
            return new StructureValue(ExpressionInfo, new StructureInstance(this));
        }
    }
}
