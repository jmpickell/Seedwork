using Ninject;
using Seedwork.IOC.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.IOC.Ninject
{
    public class NinjectScope : IIocScope
    {
        private readonly IKernel _kernel;

        public NinjectScope(IKernel kernel)
        {
            _kernel = kernel;
        }

        public T Resolve<T>() =>
            _kernel.Get<T>();

        public T Resolve<T>(string name) =>
            _kernel.Get<T>(name);

        public void Dispose() =>
            _kernel.Dispose();
    }
}
