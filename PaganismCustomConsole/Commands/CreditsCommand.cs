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

        public override string Command => "credits";

        public override string Description => "Show credits";

        public override string[] Aliases { get; } = new string[0];

        public override bool Execute(Dictionary<string, string> arguments, out string response)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("- ImIsaacTbh");
            stringBuilder.AppendLine("- FoxWorn");

            response = stringBuilder.ToString();
            return true;
        }
    }
}
