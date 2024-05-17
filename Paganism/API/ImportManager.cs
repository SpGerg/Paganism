using System.Collections.Generic;

#nullable enable
namespace Paganism.API
{
    public class ImportManager
    {
        public static string? SpecificDirectory { get; internal set; } = null;

        public static Dictionary<string, string[]> PreLoadedFiles { get; internal set; } = new();

        public static void AddPreLoadedFile(string Name, string[] Content)
        {
            if (PreLoadedFiles.ContainsKey(Name))
            {
                return;
            }

            PreLoadedFiles.Add(Name, Content);
        }

        public static void SetSpecificDirectory(string Path)
        {
            SpecificDirectory = Path;
        }
    }
}
