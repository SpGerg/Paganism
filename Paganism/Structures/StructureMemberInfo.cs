using Paganism.PParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Structures
{
    public readonly struct StructureMemberInfo
    {
        public StructureMemberInfo(bool isDelegate, Argument[] arguments, bool isReadOnly, bool isShow, bool isCastable, bool isAsync)
        {
            IsDelegate = isDelegate;
            Arguments = arguments;
            IsReadOnly = isReadOnly;
            IsShow = isShow;
            IsCastable = isCastable;
            IsAsync = isAsync;
        }

        public bool IsDelegate { get; }

        public Argument[] Arguments { get; }

        public bool IsReadOnly { get; }

        public bool IsShow { get; }

        public bool IsCastable { get; }

        public bool IsAsync { get; }
    }
}
