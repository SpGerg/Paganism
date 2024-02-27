using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0659
namespace Paganism.PParser.Values
{
    public class StructureValue : Value
    {
        public StructureValue(Dictionary<string, StructureMemberExpression> members)
        {
            Values = new Dictionary<string, Value>();

            foreach (var member in members)
            {
                if (member.Value.Info.IsDelegate)
                {
                    Values.Add(member.Key, new FunctionValue(null));
                }
                else
                {
                    Values.Add(member.Key, Value.NoneValue);
                }
            }
        }

        public StructureValue(BlockStatementExpression expression, StructureInstance structureInstance) : this(structureInstance.Members)
        {
            Structure = structureInstance;
            BlockStatement = expression;
        }

        public StructureValue(BlockStatementExpression expression, string name) : this(Interpreter.Data.Structures.Instance.Value.Get(expression, name).Members)
        {
            Structure = Interpreter.Data.Structures.Instance.Value.Get(expression, name);
            BlockStatement = expression;
        }

        public override string Name => "Structure";

        public override TypesType Type => TypesType.Structure;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String
        };

        public Dictionary<string, Value> Values { get; }

        public StructureInstance Structure { get; }

        public BlockStatementExpression BlockStatement { get; }

        public void Set(string key, Value value, string filePath)
        {
            if (!Values.ContainsKey(key))
            {
                throw new InterpreterException($"Unknown member with '{key}' name.");
            }

            var member = Structure.Members[key];

            if (member.Info.IsReadOnly && filePath != member.Filepath)
            {
                throw new InterpreterException($"You cant access to structure member '{key}' in '{Structure.Name}' structure");
            }

            if (member.Type.Value != TypesType.Any && member.Type.Value != value.Type && (value is TypeValue typeValue && typeValue.Value is not TypesType.None))
            {
                throw new InterpreterException($"Except {member.Type} type");
            }

            if (member.Structure is not null && member.Structure != string.Empty)
            {
                if (value is StructureValue structureValue1 && structureValue1.Structure.Name != member.Type.TypeName)
                {
                    throw new InterpreterException($"Except structure '{member.Type}' type");
                }

                if (value is EnumValue enumValue && enumValue.Member.Enum != member.Type.TypeName)
                {
                    throw new InterpreterException($"Except enum '{member.Type}' type");
                }
            }

            if (member.Info.IsDelegate)
            {
                if (value is not FunctionValue functionValue)
                {
                    throw new InterpreterException($"Except function", member.Line, member.Position);
                }

                if (!functionValue.Is(member.Type.Value, member.Type.TypeName))
                {
                    throw new InterpreterException($"Except member {member.Name}, {member.Type}", member.Line, member.Position);
                }
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
                try
                {
                    result += $"{item.Key} = {Values[item.Key].AsString()}, ";
                }
                catch
                {
                    result += $"{item.Key} = {Values[item.Key]}, ";
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
