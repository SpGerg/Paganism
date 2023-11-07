using PaganismCustomConsole.API.Features.Commands;
using PaganismCustomConsole.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaganismCustomConsole.API.Features
{
    public class CustomConsole
    {
        public CommandBase[] Commands { get; }

        public bool IsOpen { get; set; }

        public CustomConsole()
        {
            Commands = new CommandBase[]
            {
                new CompileCommand()
            };
        }

        public void Run()
        {
            IsOpen = true;

            while (IsOpen)
            {
                Console.Write(">>> ");
                string line = Console.ReadLine();

                var arguments = line.Split(' ');

                var executeCommand = Commands.FirstOrDefault(command => command.Command == arguments[0]);

                if (executeCommand == default)
                {
                    Console.WriteLine($"Unknown command with name {line}");
                }
                else
                {
                    executeCommand.Execute(new ArraySegment<string>(arguments), out string response);
                    Console.WriteLine(response);
                }

                Console.WriteLine();
            }
        }
    }
}
