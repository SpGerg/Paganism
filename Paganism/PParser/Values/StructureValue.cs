using Paganism.Exceptions;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;
using Paganism.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Paganism.PParser.Values
{
    public class StructureValue : Value, ISettable
    {
        public StructureValue(ExpressionInfo info, string name, Dictionary<string, StructureMemberExpression> members, InstanceInfo instanceInfo) : base(info)
        {
            Values = new Dictionary<string, Value>();

            Structure = new StructureInstance(new StructureDeclarateExpression(ExpressionInfo, name, members.Values.ToArray(), instanceInfo));

            foreach (var member in members)
            {
                if (member.Value.Type is FunctionTypeValue functionTypeValue)
                {
                    Values.Add(member.Key, new FunctionValue(info, new FunctionDeclarateExpression(info, member.Key, null, functionTypeValue.Arguments, true,
                        new InstanceInfo(member.Value.Info.IsShow, member.Value.Info.IsShow, member.Value.ExpressionInfo.Filepath), functionTypeValue.ReturnType)));
                }
                else
                {
                    Values.Add(member.Key, new NoneValue(ExpressionInfo.EmptyInfo));
                }
            }

            AddStandartFunctions();
        }

        public StructureValue(ExpressionInfo info, StructureDeclarateExpression structureDeclarateExpression) : base(info)
        {
            Values = new Dictionary<string, Value>();
            Structure = new StructureInstance(structureDeclarateExpression);

            foreach (var member in structureDeclarateExpression.Members)
            {
                if (member.Type is FunctionTypeValue functionTypeValue)
                {
                    Values.Add(member.Name, new FunctionValue(info, new FunctionDeclarateExpression(info, member.Name, null, functionTypeValue.Arguments, true,
                        new InstanceInfo(member.Info.IsShow, member.Info.IsShow, member.ExpressionInfo.Filepath), functionTypeValue.ReturnType)));
                }
                else
                {
                    Values.Add(member.Name, new NoneValue(ExpressionInfo.EmptyInfo));
                }
            }

            AddStandartFunctions();
        }

        public StructureValue(ExpressionInfo info, Value value) : base(info)
        {
            Values = new Dictionary<string, Value>();

            var members = new List<StructureMemberExpression>();

            var memberInfo = new StructureMemberInfo(false, true, false);

            var name = string.Empty;

            switch (value)
            {
                case StructureValue structureValue:
                    var membersValues = new List<Value>();
                    var membersNames = new List<StringValue>();
                    var membersTypes = new List<TypeValue>();
                    name = structureValue.Structure.Name;

                    var arrayType = new TypeValue(value.ExpressionInfo, TypesType.Array, string.Empty);

                    foreach (var member in structureValue.Values)
                    {
                        membersValues.Add(member.Value);
                        membersNames.Add(new StringValue(value.ExpressionInfo, member.Key));
                        membersTypes.Add(member.Value.GetTypeValue());
                    }

                    members.Add(new StructureMemberExpression(value.ExpressionInfo, value.Name,
                        arrayType, "members_names", memberInfo));
                    members.Add(new StructureMemberExpression(value.ExpressionInfo, value.Name,
                        arrayType, "members_values", memberInfo));
                    members.Add(new StructureMemberExpression(value.ExpressionInfo, value.Name,
                       arrayType, "members_types", memberInfo));

                    Values.Add("members_names", new ArrayValue(value.ExpressionInfo, membersNames.ToArray()));
                    Values.Add("members_values", new ArrayValue(value.ExpressionInfo, membersValues.ToArray()));
                    Values.Add("members_types", new ArrayValue(value.ExpressionInfo, membersTypes.ToArray()));
                    break;
                case FunctionValue functionValue:
                    GetFunction(functionValue.GetTypeValue() as FunctionTypeValue, value.ExpressionInfo,
                        functionValue.Name, members);
                    break;
                case FunctionTypeValue functionTypeValue:
                    GetFunction(functionTypeValue, value.ExpressionInfo, "Function", members);
                    break;
            }

            Structure = new StructureInstance(
                new StructureDeclarateExpression(value.ExpressionInfo, name, members.ToArray(), InstanceInfo.Empty));
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
                if (member.Value.Type is FunctionTypeValue functionTypeValue)
                {
                    Values.Add(member.Key, new FunctionValue(info, new FunctionDeclarateExpression(info, member.Key, null, functionTypeValue.Arguments, true,
                        new InstanceInfo(member.Value.Info.IsShow, member.Value.Info.IsShow, member.Value.ExpressionInfo.Filepath), functionTypeValue.ReturnType)));
                }
                else
                {
                    Values.Add(member.Key, new NoneValue(ExpressionInfo.EmptyInfo));
                }
            }

            AddStandartFunctions();
        }

        public override string Name => "Structure";

        public override TypesType Type => TypesType.Structure;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String
        };

        public Dictionary<string, Value> Values { get; private set; }

        public StructureInstance Structure { get; private set; }

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

            if (Values.TryGetValue(key, out _))
            {
                Values.Remove(key);

                Values[key] = value;
            }
            else
            {
                Values.Add(key, value);
            }
        }

        public StructureValue GetCastableMember(TypeValue typeValue)
        {
            foreach (var member in Structure.Members)
            {
                if (!member.Value.Info.IsCastable)
                {
                    continue;
                }

                var value = Values[member.Key];

                if (value is not StructureValue structureValue1)
                {
                    continue;
                }

                if (!structureValue1.Is(typeValue))
                {
                    continue;
                }

                return structureValue1;
            }

            return null;
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

        private void GetFunction(FunctionTypeValue functionTypeValue, ExpressionInfo expressionInfo, string name, List<StructureMemberExpression> structureMembers)
        {
            var argumentsTypes = new List<TypeValue>();
            var argumentsNames = new List<StringValue>();

            var memberInfo = new StructureMemberInfo(false, true, false);

            foreach (var argument in functionTypeValue.Arguments)
            {
                argumentsTypes.Add(argument.Type);
                argumentsNames.Add(new StringValue(expressionInfo, argument.Name));

                structureMembers.Add(new StructureMemberExpression(expressionInfo, name,
                    argument.Type, "arguments_names", memberInfo));
                structureMembers.Add(new StructureMemberExpression(expressionInfo, name,
                    argument.Type, "arguments_types", memberInfo));
            }

            structureMembers.Add(new StructureMemberExpression(expressionInfo, name,
                    new TypeValue(expressionInfo, TypesType.Boolean, string.Empty), "is_async", memberInfo));
            structureMembers.Add(new StructureMemberExpression(expressionInfo, name,
                    functionTypeValue.ReturnType, "return_type", memberInfo));

            Values.Add("is_async", new BooleanValue(expressionInfo, functionTypeValue.IsAsync));
            Values.Add("arguments_names", new ArrayValue(expressionInfo, argumentsNames.ToArray()));
            Values.Add("arguments_types", new ArrayValue(expressionInfo, argumentsTypes.ToArray()));
            Values.Add("return_type", functionTypeValue.ReturnType);
        }

        private void AddStandartFunctions()
        {
            Values.Add("toString", new FunctionValue(ExpressionInfo, new FunctionDeclarateExpression(ExpressionInfo, "toString",
                null, new Argument[0], false, InstanceInfo.Empty,
                new TypeValue(ExpressionInfo, TypesType.String, string.Empty)),
                (Argument[] arguments) =>
                {
                    return new StringValue(ExpressionInfo, AsString());
                }));

            Values.Add("getHashCode", new FunctionValue(ExpressionInfo, new FunctionDeclarateExpression(ExpressionInfo, "getHashCode",
                null, new Argument[0], false, InstanceInfo.Empty,
                new TypeValue(ExpressionInfo, TypesType.Number, string.Empty)),
                (Argument[] arguments) =>
                {
                    return new NumberValue(ExpressionInfo, GetHashCode());
                }));

            Values.Add("equals", new FunctionValue(ExpressionInfo, new FunctionDeclarateExpression(ExpressionInfo, "equals",
                null, new Argument[] { new Argument("target", TypesType.Any, null, true) }, false, InstanceInfo.Empty,
                new TypeValue(ExpressionInfo, TypesType.Boolean, string.Empty)),
                (Argument[] arguments) =>
                {
                    return new BooleanValue(ExpressionInfo, Is(arguments[0].Value.GetTypeValue()));
                }));
        }

        public override bool Is(TypeValue typeValue)
        {
            return typeValue.Value is TypesType.Object ||
                    Structure.Name == typeValue.TypeName ||
                    GetCastableMember(typeValue) is not null;
        }

        public override bool Is(Value value)
        {
            if (value is not StructureValue structureValue)
            {
                return false;
            }

            for (var i = 0;i < structureValue.Values.Count;i++)
            {
                if (i > Values.Count - 1)
                {
                    return false;
                }

                var member = structureValue.Values.ElementAt(i);
                var member1 = Values.ElementAt(i);

                if (member.Key != member1.Key)
                {
                    return false;
                }

                if (!member.Value.Is(member1.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public void Set(Value value)
        {
            if (value is StructureValue structureValue)
            {
                Values = structureValue.Values;
                Structure = structureValue.Structure;
            }
            else if (value is ObjectValue objectValue)
            {
                Values = objectValue.Value.Values;
                Structure = objectValue.Value.Structure;
            }
        }
    }
}
