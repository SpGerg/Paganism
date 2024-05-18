using PaganismCustomConsole.API.Features;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;
using System.IO;

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

        public override CommandParameter[] Parameters { get; } = new CommandParameter[]
        {
            new CommandParameter("path", "path", true)
        };

        public override bool Execute(Dictionary<string, string> arguments, out string response)
        {
            var path = arguments["path"];

            if (!Directory.Exists(path))
            {
                response = $"Directory {path} doesnt exists";
                return false;
            }

            CustomConsole.CurrentDirectory = arguments["path"];

            response = string.Empty;
            return true;
        }
    }
}
