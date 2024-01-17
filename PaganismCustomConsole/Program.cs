using PaganismCustomConsole.API.Features;

namespace PaganismCustomConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomConsole customConsole = new(false);

            customConsole.Run();
        }
    }
}
