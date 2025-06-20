using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.IOC.Interfaces
{
    public interface IIocScope : IDisposable
    {
        T Resolve<T>();
        T Resolve<T>(string name);
    }
}
