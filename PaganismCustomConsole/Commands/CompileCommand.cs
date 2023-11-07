using Paganism.Lexer;
using PaganismCustomConsole.API.Features;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaganismCustomConsole.Commands
{
    public class CompileCommand : CommandBase
    {
        public CompileCommand(CustomConsole customConsole) : base(customConsole)
        {
        }

        public override string Command => "compile";

        public override string Description => "Compile a paganism script";

        public override string[] Aliases { get; } = new string[] { "cmp" };

        public override CommandParameter[] Parameters { get; } = new CommandParameter[]
        {
            new CommandParameter("filename", "filename to compile", true)
        };

        public override bool Execute(Dictionary<string, string> arguments, out string response)
        {
            Lexer lexer;

            if (CustomConsole.IsDebug)
            {
                lexer = new Lexer(new string[] { "print(\"Hello, world\")", "print(12345689.25)" });
            }
            else
            {
                var path = Path.Combine(CustomConsole.CurrentDirectory, arguments["filename"]);

                if (!File.Exists(path))
                {
                    response = "File is not exists";
                    return false;
                }

                lexer = new Lexer(File.ReadAllLines(path));
            }
            
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
