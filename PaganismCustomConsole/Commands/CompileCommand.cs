using Paganism.Lexer;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaganismCustomConsole.Commands
{
    public class CompileCommand : CommandBase
    {
        public override string Command => "compile";

        public override string Description => "Compile a paganism script";

        public override string[] Aliases => new string[] { "cmp" };

        public override bool Execute(Dictionary<string, string> arguments, out string response)
        {
            var lexer = new Lexer(new string[] { "print(\"Hello, world)" });
            Token[] tokens;

            try
            {
                tokens = lexer.Run();
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
                return true;
            }

            foreach (var token in tokens)
            {
                Console.WriteLine($"{token.Type}: {token.Value}");
            }

            response = "Script has been compilated";
            return true;
        }
    }
}
