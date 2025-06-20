using Seedwork.IOC.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.IOC
{
    public interface IServiceLocator
    {
        void RegisterScope(IIocScope scope);
    }

    //To be used in only very specific cases
    public class ServiceLocator : IServiceLocator
    {
        static Lazy<ServiceLocator> _instance = new Lazy<ServiceLocator>();
        private IIocScope _scope;

        public static ServiceLocator Instance => _instance.Value;

        public void RegisterScope(IIocScope scope) =>
            _scope = scope;

        public IIocScope GetScope() => _scope;
    }
}
