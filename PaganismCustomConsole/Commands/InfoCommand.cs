using PaganismCustomConsole.API.Features;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaganismCustomConsole.Commands
{
    public class InfoCommand : CommandBase
    {
        public InfoCommand(CustomConsole customConsole) : base(customConsole)
        {
        }

        public override string Command => "info";

        public override string Description => "Show info";

        public override string[] Aliases { get; } = new string[0];

        public override bool Execute(Dictionary<string, string> arguments, out string response)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Paganism - A dynamically typed scripting programming language created by the Uncomplicated Custom Servers team.");
            stringBuilder.AppendLine("https://discord.gg/VEXKQ8G8");
            stringBuilder.AppendLine("https://github.com/UncomplicatedCustomServer");

            response = stringBuilder.ToString();
            return false;
        }
    }
}
