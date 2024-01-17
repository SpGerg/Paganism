using PaganismCustomConsole.API.Features;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;

namespace PaganismCustomConsole.Commands
{
    public class ChangeDirectoryCommand : CommandBase
    {
        public ChangeDirectoryCommand(CustomConsole customConsole) : base(customConsole)
        {
        }

        public override string Command => "change_directory";

        public override string Description => "Changing the current directory";

        public override string[] Aliases => new string[] { "cd" };

        public override bool Execute(Dictionary<string, string> arguments, out string response)
        {
            throw new NotImplementedException();
        }
    }
}
