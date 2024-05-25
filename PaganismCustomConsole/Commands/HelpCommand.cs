using PaganismCustomConsole.API.Features;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaganismCustomConsole.Commands
{
    public class HelpCommand : CommandBase
    {
        public HelpCommand(CustomConsole customConsole) : base(customConsole)
        {
        }

        public override string Command => "help";

        public override string Description => "Show list of commands or command description";

        public override string[] Aliases { get; } = new string[0];

        public override CommandParameter[] Parameters { get; } = new CommandParameter[]
        {
            new CommandParameter("name", "Command name", false, typeof(string))
        };

        public override bool Execute(Dictionary<string, string> arguments, out string response)
        {
            if (arguments.Count == 0)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("Commands list: ");

                foreach (var command in CustomConsole.Commands)
                {
                    stringBuilder.AppendLine($"{command.Command}: {command.Description}");
                }

                response = stringBuilder.ToString();
                return true;
            }
            
            var name = arguments["name"];

            var command1 = CustomConsole.Commands.FirstOrDefault(command => command.Command == name);

            if (command1 is null)
            {
                response = $"Unknown command with {name} name";
                return false;
            }

            response = $"{command1.Command}: {command1.Description}";
            return true;
        }
    }
}
