using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaganismCustomConsole.API.Features.Commands
{
    public readonly struct CommandParameter
    {
        public string Name { get; }

        public string Description { get; }

        public bool IsRequired { get; }

        public Type Type { get; }
    }
}
