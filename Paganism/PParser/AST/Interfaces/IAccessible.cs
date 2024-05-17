using Paganism.Interpreter.Data.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST.Interfaces
{
    public interface IAccessible
    {
        InstanceInfo Info { get; }
    }
}
