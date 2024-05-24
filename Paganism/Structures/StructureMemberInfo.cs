using Paganism.PParser;

namespace Paganism.Structures
{
    public readonly struct StructureMemberInfo
    {
        public StructureMemberInfo(bool isReadOnly, bool isShow, bool isCastable, bool isAsync)
        {
            IsReadOnly = isReadOnly;
            IsShow = isShow;
            IsCastable = isCastable;
            IsAsync = isAsync;
        }

        public bool IsReadOnly { get; }

        public bool IsShow { get; }

        public bool IsCastable { get; }

        public bool IsAsync { get; }
    }
}
