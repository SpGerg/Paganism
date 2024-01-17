using System;
using System.Collections.Generic;
using System.Text;

namespace PaganismCustomConsole.API.Features.Commands
{
    public abstract class CommandBase
    {
        public abstract string Command { get; }

        public abstract string Description { get; }

        public abstract string[] Aliases { get; }

        public virtual CommandParameter[] Parameters { get; } = new CommandParameter[0];

        public CustomConsole CustomConsole { get; }

        protected CommandBase(CustomConsole customConsole)
        {
            CustomConsole = customConsole;
        }

        public abstract bool Execute(Dictionary<string, string> arguments, out string response);

        public bool Execute(ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count - 1 < Parameters.Length)
            {
                StringBuilder stringBuilder = new();
                _ = stringBuilder.AppendLine("Command paramaters: ");

                foreach (CommandParameter parameter in Parameters)
                {
                    _ = stringBuilder.AppendLine($"{parameter.Name}: {parameter.Description} " + (parameter.Type is null ? string.Empty : parameter.Type.Name));
                }

                response = stringBuilder.ToString();
                return false;
            }

            Dictionary<string, string> result = new();

            for (int i = 0; i < Parameters.Length; i++)
            {
                int index = arguments.Array.Length - 1 - i;

                if (index > arguments.Count && Parameters[i].IsRequired)
                {
                    throw new ArgumentException($"{arguments.Array.Length - 1 - i} argument must be exists");
                }

                string value = arguments.Array[index];

                if (Parameters[i].Type != null)
                {
                    Type type = Parameters[i].Type;

                    if (!TryParse(type, value))
                    {
                        throw new ArgumentException($"{arguments.Array.Length - 1 - i} argument must be {type.Name}");
                    }
                }

                result.Add(Parameters[i].Name, value);
            }

            return Execute(result, out response);
        }

        private bool TryParse(Type type, string value)
        {
            return type == typeof(int)
                ? int.TryParse(value, out _)
                : type == typeof(bool) ? bool.TryParse(value, out _) : type == typeof(char) && char.TryParse(value, out _);
        }
    }
}
