using Paganism.PParser;

namespace Paganism.Structures
{
    public readonly struct StructureMemberInfo
    {
        public StructureMemberInfo(bool isReadOnly, bool isShow, bool isCastable)
        {
            IsReadOnly = isReadOnly;
            IsShow = isShow;
            IsCastable = isCastable;
        }

        public bool IsReadOnly { get; }

        public bool IsShow { get; }

        public bool IsCastable { get; }
    }
}
