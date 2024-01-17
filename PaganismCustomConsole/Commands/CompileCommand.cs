using Paganism.Exceptions;
using Paganism.Interpreter;
using Paganism.Interpreter.Data;
using Paganism.Lexer;
using Paganism.PParser;
using PaganismCustomConsole.API.Features;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;
using System.IO;

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
            string path = Path.Combine(CustomConsole.CurrentDirectory, arguments["filename"]);

            if (!File.Exists(path))
            {
                response = "File is not exists";
                return false;
            }

            try
            {
                Lexer lexer = new(File.ReadAllLines(path));
                Token[] tokens = lexer.Run();

                Parser parser = new(tokens, path);
                Paganism.PParser.AST.BlockStatementExpression expressions = parser.Run();

                Interpreter compiler = new(expressions);
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
