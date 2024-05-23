using Paganism.PParser;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Paganism.Interpreter.Data.Instances
{
    public abstract class Instance
    {
        public Instance(InstanceInfo instanceInfo)
        {
            Info = instanceInfo;
        }

        public abstract string InstanceName { get; }

        public InstanceInfo Info { get; }

        public static Instance Create(Value value)
        {
            switch (value)
            {
                case FunctionValue functionValue:
                    return new FunctionInstance(functionValue.Value.Info, new PParser.AST.FunctionDeclarateExpression(ExpressionInfo.EmptyInfo,
                        functionValue.Name, functionValue.Value.Statement, functionValue.Value.Arguments,
                        functionValue.Value.IsAsync, functionValue.Value.Info, functionValue.Value.ReturnType));
                case StructureValue structureValue:
                    return new StructureInstance(new PParser.AST.StructureDeclarateExpression(ExpressionInfo.EmptyInfo,
                        structureValue.Structure.Name, structureValue.Structure.Members.Values.ToArray(), structureValue.Structure.Info));
            }

            return null;
        }

        public static Instance ToInstance(object value)
        {
            if (value is FunctionValue functionValue)
            {
                return new FunctionInstance(functionValue.Value.Info, functionValue.Value);
            }
            else if (value is StructureValue structureValue)
            {
                var functions = new Dictionary<string, Func<Argument[], Value>>();

                foreach (var member in structureValue.Values)
                {
                    if (member.Value is not FunctionValue functionValue2)
                    {
                        continue;
                    }

                    if (functionValue2.Func is null)
                    {
                        continue;
                    }

                    functions.Add(member.Key, functionValue2.Func);
                }

                return new StructureInstance(structureValue.Structure.StructureDeclarateExpression, functions);
            }

            return null;
        }
    }
}
