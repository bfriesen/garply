using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace garply
{
    public interface IExecutionContext
    {
        void Push(IFirstClassType value);
        IFirstClassType Pop();
    }
}
