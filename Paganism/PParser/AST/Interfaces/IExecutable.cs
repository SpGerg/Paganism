using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST.Interfaces
{
    public interface IExecutable
    {
        void Execute(params Argument[] arguments);
    }
}
