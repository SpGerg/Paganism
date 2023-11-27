using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser
{
    public class Argument
    {
        public Argument(string name, TypesType requiredValue, bool isRequired, IEvaluable value = null, bool isArray = false, Value defaultValue = null, string structureName = null)
        {
            Name = name;
            Type = requiredValue;
            IsRequired = isRequired;
            Value = value;
            IsArray = isArray;
            DefaultValue = defaultValue;
            StructureName = structureName;
        }

        public string Name { get; set; }

        public IEvaluable Value { get; set; }

        public TypesType Type { get; }

        public bool IsRequired { get; }

        public Value DefaultValue { get; }

        public bool IsArray { get; }

        public string StructureName { get; }
    }
}
