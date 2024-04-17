namespace Paganism.Lexer
{
    public static class Utilities
    {
        public static string ReplaceEscapeCode(string value)
        {
            return value switch
            {
                "\\n" => "\n",
                "\\0" => "\0",
                "\\t" => "\t",
                _ => value,
            };
        }

        public static string ReplaceEscapeCodes(string value)
        {
            var result = string.Empty;

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (c == '\\')
                {
                    result += ReplaceEscapeCode(c.ToString() + value[i + 1]);
                    i++; //Skip identifier
                    continue;
                }

                result += c;
            }

            return result;
        }
    }
}
