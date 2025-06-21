using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.IOC.Interfaces
{
    public interface IIocScope : IDisposable
    {
        T Resolve<T>(params IocParameter[] parameters);
        T Resolve<T>(string name, params IocParameter[] parameters);
    }
}
