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
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Command paramaters: ");

                foreach (var parameter in Parameters)
                {
                    stringBuilder.AppendLine($"{parameter.Name}: {parameter.Description} " + (parameter.Type is null ? string.Empty : parameter.Type.Name));
                }

                response = stringBuilder.ToString();
                return false;
            }

            var result = new Dictionary<string, string>();

            for (int i = 0; i < Parameters.Length; i++)
            {
                var index = (arguments.Array.Length - 1) - i;

                if (index > arguments.Count && Parameters[i].IsRequired)
                {
                    throw new ArgumentException($"{(arguments.Array.Length - 1) - i} argument must exist");
                }

                var value = arguments.Array[index];

                if (Parameters[i].Type != null)
                {
                    var type = Parameters[i].Type;

                    if (!TryParse(type, value))
                    {
                        throw new ArgumentException($"{(arguments.Array.Length - 1) - i} argument must be {type.Name}");
                    }
                }

                result.Add(Parameters[i].Name, value);
            }

            return Execute(result, out response);
        }

        private bool TryParse(Type type, string value)
        {
            if (type == typeof(int))
            {
                return int.TryParse(value, out _);
            }

            if (type == typeof(bool))
            {
                return bool.TryParse(value, out _);
            }

            if (type == typeof(char))
            {
                return char.TryParse(value, out _);
            }

            return false;
        }
    }
}
