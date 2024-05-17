﻿using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data.Instances
{
    public class VariableInstance : Instance
    {
        public VariableInstance(InstanceInfo instanceInfo, Value value, TypeValue typeValue = null) : base(instanceInfo)
        {
            Value = value;
            Type = typeValue;

            if (Value is not null)
            {
                Type = Value.GetTypeValue();
            }
        }

        public override string InstanceName => "Variable";

        public Value Value { get; }

        public TypeValue Type { get; }
    }
}
