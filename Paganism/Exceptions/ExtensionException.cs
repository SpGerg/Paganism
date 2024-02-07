namespace Paganism.Exceptions
{
    internal class ExtensionException : PaganismException
    {
        public ExtensionException()
        {
        }
        public ExtensionException(string message)
            : base(message, "Extension")
        {
        }


        public ExtensionException(string message, int line, int position)
            : base(message, line, position, "Extension")
        {
        }

        public override string Name => "Extension";
    }
}
