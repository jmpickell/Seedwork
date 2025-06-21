using Ninject;
using Seedwork.IOC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public T Resolve<T>(params IocParameter[] parameters) =>
            _kernel.Get<T>(parameters.Select(x => x.Convert()).ToArray());

        public T Resolve<T>(string name, params IocParameter[] parameters) =>
            _kernel.Get<T>(name, parameters.Select(x => x.Convert()).ToArray());

        public void Dispose() =>
            _kernel.Dispose();
    }
}
