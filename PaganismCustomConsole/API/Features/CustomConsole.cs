using PaganismCustomConsole.API.Features.Commands;
using PaganismCustomConsole.Commands;
using System;
using System.Linq;

namespace PaganismCustomConsole.API.Features
{
    public class CustomConsole
    {
        public CommandBase[] Commands { get; }

        public bool IsOpen { get; set; }

        public string CurrentDirectory { get; set; }

        public bool IsDebug { get; }

        public CustomConsole(bool isDebug)
        {
            Commands = new CommandBase[]
            {
                new CompileCommand(this),
                new ChangeDirectoryCommand(this)
            };

            IsDebug = isDebug;
            CurrentDirectory = Environment.CurrentDirectory;
        }

        public void Run()
        {
            IsOpen = true;

            while (IsOpen)
            {
                Console.Write($"{CurrentDirectory}> ");
                string line = Console.ReadLine();

                string[] arguments = line.Split(' ');

                CommandBase executeCommand = Commands.FirstOrDefault(command => command.Command == arguments[0] || command.Aliases.Contains(arguments[0]));

                if (executeCommand == default)
                {
                    Console.WriteLine($"Unknown command with name {line}");
                }
                else
                {
                    _ = executeCommand.Execute(new ArraySegment<string>(arguments), out string response);
                    Console.WriteLine(response);
                }

                Console.WriteLine();
            }
        }
    }
}
