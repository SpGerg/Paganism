using Paganism.API.Attributes;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using Paganism.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Paganism.API
{
    public static class PaganismFromCSharp
    {
        public static object Create(Type type, ref List<object> importedTypes)
        {
            var value = Value.Create(type);

            importedTypes ??= new List<object>();

            if (value is not NoneValue)
            {
                return value;
            }

            if (type.IsEnum)
            {
                var enumType = CreateEnum(type);

                importedTypes.Add(enumType);

                return enumType;
            }

            if (IsStructure(type) || type.IsClass)
            {
                return CreateStructureOrClass(type, importedTypes);
            }

            return null;
        }

        public static StructureValue CreateStructureOrClass(Type type, List<object> importedTypes)
        {
            IEnumerable<FieldInfo> fields;
            IEnumerable<MethodInfo> methods;

            fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase).
                Where(field => field.GetCustomAttribute<PaganismSerializable>() is not null);

            methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase).
                Where(method => method.GetCustomAttribute<PaganismSerializable>() is not null &&
                method.GetParameters()[0].ParameterType.IsArray && method.GetParameters()[0].ParameterType == typeof(Argument[])
                && method.ReturnType == typeof(Value));

            var members = new Dictionary<string, StructureMemberExpression>(fields.Count());

            foreach (var field in fields)
            {
                var csharpType = Create(field.FieldType, ref importedTypes);

                var typeValue = csharpType is Value value ? value.GetTypeValue() : new TypeValue(ExpressionInfo.EmptyInfo, TypesType.Enum, (csharpType as EnumInstance).Name);

                if (!importedTypes.Contains(csharpType) && csharpType is not Value)
                {
                    importedTypes.Add(csharpType);
                }

                members.Add(field.Name, new StructureMemberExpression(ExpressionInfo.EmptyInfo,
                    type.Name, typeValue, field.Name,
                    new StructureMemberInfo(false, true, false)));
            }

            foreach (var method in methods)
            {
                var array = method.GetParameters();

                var arguments = new Argument[array.Length];

                for (int j = 0; j < array.Length; j++)
                {
                    var paramater = array[j];

                    var createdType = Create(paramater.ParameterType, ref importedTypes);

                    var argumentType = createdType is Value value ? value.GetTypeValue() : new TypeValue(ExpressionInfo.EmptyInfo, TypesType.Enum, (createdType as EnumInstance).Name);

                    arguments[j] = new Argument(paramater.Name, argumentType, null, true, paramater.ParameterType.IsArray);
                }

                var createdReturnType = Create(method.ReturnType, ref importedTypes);
                var returnType = createdReturnType is Value value2 ? value2.GetTypeValue() : new TypeValue(ExpressionInfo.EmptyInfo, TypesType.Enum, (createdReturnType as EnumInstance).Name);

                members.Add(method.Name, new StructureMemberExpression(ExpressionInfo.EmptyInfo,
                    type.Name, returnType, method.Name,
                    new StructureMemberInfo(true, true, false)));
            }

            var structure = new StructureValue(ExpressionInfo.EmptyInfo, type.Name, members, new InstanceInfo(false, false, string.Empty));

            if (methods.Count() != 0)
            {
                var instance = Activator.CreateInstance(type);

                foreach (var method in methods)
                {
                    var member = structure.Structure.Members[method.Name];

                    var action = (Func<Argument[], Value>)Delegate.CreateDelegate(typeof(Func<Argument[], Value>), instance, method);

                    structure.Values[method.Name] = new FunctionValue(ExpressionInfo.EmptyInfo, method.Name, (member.Type as FunctionTypeValue).Arguments, member.Type, action);
                }
            }

            return structure;
        }

        public static EnumInstance CreateEnum(Type type)
        {
            if (!type.IsEnum)
            {
                return null;
            }

            var values = type.GetEnumValues();

            var members = new EnumMemberExpression[values.Length];

            System.Collections.IList list = values;
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                var name = type.GetEnumName(value);

                var memberValue = new NumberValue(ExpressionInfo.EmptyInfo, (int)value);

                members[i] = new EnumMemberExpression(ExpressionInfo.EmptyInfo, name, memberValue, type.Name);
            }

            var instanceInfo = new InstanceInfo(true, false, string.Empty);

            return new EnumInstance(instanceInfo, new EnumDeclarateExpression(ExpressionInfo.EmptyInfo, type.Name, members, instanceInfo));
        }

        public static bool IsStructure(Type source)
        {
            return source.IsValueType && !source.IsPrimitive && !source.IsEnum;
        }

        public static object ValueToObject(Value value)
        {
            switch (value)
            {
                case StringValue stringValue:
                    return stringValue.Value;
                case NumberValue numberValue:
                    return numberValue.Value;
                case BooleanValue booleanValue:
                    return booleanValue.Value;
                case ArrayValue arrayValue:
                    var array = new object[arrayValue.Elements.Length];

                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = ValueToObject(value);
                    }

                    return array;
                case EnumValue enumValue:
                    return enumValue.Member.Enum;
            }

            return null;
        }
    }
}
