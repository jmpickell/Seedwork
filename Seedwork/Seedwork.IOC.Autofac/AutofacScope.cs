using Autofac;
using Seedwork.IOC.Interfaces;

namespace Seedwork.IOC.Autofac
{
    public class AutofacScope : IIocScope
    {
        private readonly ILifetimeScope _scope;

        public AutofacScope(ILifetimeScope scope) => 
            _scope = scope;

        public void Dispose() =>
            _scope.Dispose();

        public T Resolve<T>() =>
            _scope.Resolve<T>();

        public T Resolve<T>(string name) =>
            _scope.ResolveNamed<T>(name);
    }
}
