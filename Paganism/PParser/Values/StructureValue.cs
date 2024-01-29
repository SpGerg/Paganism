using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Paganism.PParser.Values
{
    public class StructureValue : Value
    {
        public StructureValue(Dictionary<string, StructureMemberExpression> members)
        {
            Values = new Dictionary<string, Value>();

            foreach (var member in members)
            {
                if (member.Value.IsDelegate)
                {
                    Values.Add(member.Key, new FunctionValue(null));
                }
                else
                {
                    Values.Add(member.Key, new NoneValue());
                }
            }
        }

        public StructureValue(BlockStatementExpression expression, StructureInstance structureInstance) : this(structureInstance.Members)
        {
            Structure = structureInstance;
            BlockStatement = expression;
        }

        public StructureValue(BlockStatementExpression expression, string name) : this(Structures.Instance.Value.Get(expression, name).Members)
        {
            Structure = Structures.Instance.Value.Get(expression, name);
            BlockStatement = expression;
        }

        public override string Name => "Structure";

        public override TypesType Type => TypesType.Structure;

        public Dictionary<string, Value> Values { get; }

        public StructureInstance Structure { get; }

        public BlockStatementExpression BlockStatement { get; }

        public void Set(string key, Value value)
        {
            if (!Values.ContainsKey(key))
            {
                throw new InterpreterException($"Unknown member with '{key}' name.");
            }

            var member = Structure.Members[key];

            if (member.Type != value.Type && (value is TypeValue typeValue && typeValue.Value is not TypesType.None))
            {
                throw new InterpreterException($"Except {member.GetRequiredType()} type");
            }

            if (member.Structure is not null && member.Structure != string.Empty)
            {
                if (value is StructureValue structureValue1 && structureValue1.Structure.Name != member.TypeName)
                {
                    throw new InterpreterException($"Except structure '{member.GetRequiredType()}' type");
                }

                if (value is EnumValue enumValue && enumValue.Member.Enum != member.TypeName)
                {
                    throw new InterpreterException($"Except enum '{member.GetRequiredType()}' type");
                }
            }

            if (member.IsDelegate)
            {
                if (value is not FunctionValue functionValue)
                {
                    throw new InterpreterException($"Except function", member.Line, member.Position);
                }

                if (functionValue.Value.ReturnType is null)
                {
                    if (member.Type is not TypesType.None)
                    {
                        throw new InterpreterException($"Except {(member.Type is TypesType.None ? "void" : member.GetRequiredType())} return type", member.Line, member.Position);
                    }
                }
                else
                {
                    if (member.Type != functionValue.Value.ReturnType.Value || member.TypeName != functionValue.Value.ReturnType.TypeName)
                    {
                        throw new InterpreterException($"Except {member.GetRequiredType()} type", member.Line, member.Position);
                    }

                    if (functionValue.Value.ReturnType.Type is TypesType.Structure or TypesType.Enum && functionValue.Value.ReturnType.TypeName != functionValue.Value.ReturnType.TypeName)
                    {
                        throw new InterpreterException($"Except {member.GetRequiredType()} return type", member.Line, member.Position);
                    }

                    for (int i = 0; i < member.Arguments.Length; i++)
                    {
                        if (i > functionValue.Value.RequiredArguments.Length)
                        {
                            throw new InterpreterException($"Except {member.Arguments[i].Type} type in argument with {member.Arguments[i].Name} name", member.Line, member.Position);
                        }

                        if (member.Arguments[i].Type != functionValue.Value.RequiredArguments[i].Type)
                        {
                            throw new InterpreterException($"Except {member.Arguments[i].Type} type in argument with {member.Arguments[i].Name} name", member.Line, member.Position);
                        }

                        if (value is StructureValue structureValue && member.Arguments[i].TypeName != structureValue.Structure.Name)
                        {
                            throw new InterpreterException($"Except {member.Arguments[i].Type} structure type in argument with {member.Arguments[i].Name} name", member.Line, member.Position);
                        }
                    }
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
    }
}
