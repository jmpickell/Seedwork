using Autofac;
using Seedwork.IOC.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Seedwork.IOC.Autofac
{
    [ExcludeFromCodeCoverage]
    public class AutofacScope : IIocScope
    {
        private readonly ILifetimeScope _scope;

        public AutofacScope(ILifetimeScope scope) => 
            _scope = scope;

        public void Dispose() =>
            _scope.Dispose();

        public T Resolve<T>(params IocParameter[] parameters) =>
            _scope.Resolve<T>(parameters.Select(x => x.Convert()).ToArray());

        public T Resolve<T>(string name, params IocParameter[] parameters) =>
            _scope.ResolveNamed<T>(name, parameters.Select(x => x.Convert()).ToArray());
    }
}
