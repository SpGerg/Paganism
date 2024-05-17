namespace Paganism.Interpreter.Data.Instances
{
    public struct InstanceInfo
    {
        public InstanceInfo(bool isShow, bool isReadOnly, string filePath)
        {
            IsShow = isShow;
            IsReadOnly = isReadOnly;
            FilePath = filePath;
        }

        public static InstanceInfo Empty { get; } = new InstanceInfo();

        public bool IsShow { get; }

        public bool IsReadOnly { get; }

        public string FilePath { get; }
    }
}
