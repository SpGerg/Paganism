using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaganismCustomConsole.API.Features.Commands
{
    public readonly struct CommandParameter
    {
        public CommandParameter(string name, string description, bool isRequired, Type type = null)
        {
            Name = name;
            Description = description;
            IsRequired = isRequired;
            Type = type;
        }

        public string Name { get; }

        public string Description { get; }

        public bool IsRequired { get; }

        public Type Type { get; }
    }
}
