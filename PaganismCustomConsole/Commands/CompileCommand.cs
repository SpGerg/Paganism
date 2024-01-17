using Paganism;
using Paganism.Exceptions;
using Paganism.Interpreter;
using Paganism.Interpreter.Data;
using Paganism.Lexer;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Interfaces;
using PaganismCustomConsole.API.Features;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            var path = Path.Combine(CustomConsole.CurrentDirectory, arguments["filename"]);

            if (!File.Exists(path))
            {
                response = "File is not exists";
                return false;
            }

            try
            {
                var lexer = new Lexer(File.ReadAllLines(path));
                var tokens = lexer.Run();

                var parser = new Parser(tokens, path);
                var expressions = parser.Run();

                var compiler = new Interpreter(expressions);
                compiler.Run();
            }
            catch (Exception ex)
            {
                Tasks.Clear();
                Variables.Instance.Value.Clear();
                Functions.Instance.Value.Clear();
                Structures.Instance.Value.Clear();

                if (ex is not PaganismException)
                {
                    response = $"Paganism FATAL {ex.GetType().Name}, says: {ex.Message}\n{ex.StackTrace}"; ;
                    return true;
                }

                response = ex.Message;
                return true;
            }

            while (Tasks.Count() != 0) { }

            Tasks.Clear();
            Variables.Instance.Value.Clear();
            Functions.Instance.Value.Clear();
            Structures.Instance.Value.Clear();

            response = "Script has been executed.";
            return true;
        }
    }
}
