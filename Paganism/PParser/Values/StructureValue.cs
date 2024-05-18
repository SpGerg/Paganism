using Paganism.Exceptions;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0659
namespace Paganism.PParser.Values
{
    public class StructureValue : Value
    {
        public StructureValue(ExpressionInfo info, string name, Dictionary<string, StructureMemberExpression> members, InstanceInfo instanceInfo) : base(info)
        {
            Values = new Dictionary<string, Value>();

            Structure = new StructureInstance(new StructureDeclarateExpression(ExpressionInfo, name, members.Values.ToArray(), instanceInfo));

            foreach (var member in members)
            {
                if (member.Value.Info.IsDelegate)
                {
                    Values.Add(member.Key, new FunctionValue(info, null));
                }
                else
                {
                    Values.Add(member.Key, new NoneValue(ExpressionInfo.EmptyInfo));
                }
            }
        }

        public StructureValue(ExpressionInfo info, StructureInstance structureInstance) : this(info, structureInstance.Name, structureInstance.Members, structureInstance.StructureDeclarateExpression.Info)
        {
            Structure = structureInstance;
        }

        public StructureValue(ExpressionInfo info, BlockStatementExpression expression, string name) : base(info)
        {
            Structure = Interpreter.Data.Structures.Instance.Get(expression, name, ExpressionInfo);

            Values = new Dictionary<string, Value>();

            Structure = new StructureInstance(new StructureDeclarateExpression(ExpressionInfo, name, Structure.Members.Values.ToArray(), Structure.Info));

            foreach (var member in Structure.Members)
            {
                if (member.Value.Info.IsDelegate)
                {
                    Values.Add(member.Key, new FunctionValue(info, null));
                }
                else
                {
                    Values.Add(member.Key, new NoneValue(ExpressionInfo.EmptyInfo));
                }
            }
        }

        public override string Name => "Structure";

        public override TypesType Type => TypesType.Structure;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String
        };

        public Dictionary<string, Value> Values { get; }

        public StructureInstance Structure { get; }

        public void Set(string key, Value value, string filePath)
        {
            if (!Values.ContainsKey(key))
            {
                throw new InterpreterException($"Unknown member with '{key}' name.", value.ExpressionInfo);
            }

            var member = Structure.Members[key];

            if (member.Info.IsReadOnly && filePath != member.ExpressionInfo.Filepath)
            {
                throw new InterpreterException($"You cant access to structure member '{key}' in '{Structure.Name}' structure", value.ExpressionInfo);
            }

            if (!member.Type.Is(value.GetTypeValue()))
            {
                throw new InterpreterException($"Except '{member.Type}' type", value.ExpressionInfo);
            }

            if (Values.TryGetValue(key, out var result))
            {
                if (result is NoneValue || (result is FunctionValue functionValue && functionValue.Value is null))
                {
                    Values.Remove(key);

                    Values[key] = value;
                    return;
                }

                result.Set(value);
            }

            Values[key] = result;
        }

        public override string AsString()
        {
            var result = string.Empty;

            result += $"{Structure.Name} ({Name}): {{ ";

            foreach (var item in Structure.Members)
            {
                var value = Values[item.Key];

                try
                {
                    result += $"{item.Key} ({(value is NoneValue ? Structure.Members[item.Key].Type.AsString() : value.AsString())}), ";
                }
                catch
                {
                    result += $"{item.Key} ({(value is NoneValue ? Structure.Members[item.Key].Type.AsString() : value.AsString())}), ";
                }
            }

            result += "}";

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is not StructureValue structureValue)
            {
                return false;
            }

            if (structureValue.Structure.Name != Structure.Name)
            {
                return false;
            }

            return true;
        }
    }
}
