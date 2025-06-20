using Autofac;
using Seedwork.IOC.Autofac.Interfaces;
using Seedwork.IOC.Interfaces;
using Seedwork.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Seedwork.IOC.Autofac
{
    public class AutofacContainer : IIocContainer, IIocBuilder
    {
        private IContainer _container;
        private readonly ContainerBuilder _builder;
        Func<IRegistrationBuilder, Type, IRegistrationBuilder> _named;
        Action<IRegistrationBuilder> _scope;

        public AutofacContainer()
        {
            _builder = new ContainerBuilder();
            _builder.RegisterType<AutofacScope>().As<IIocScope>();
            _scope = s => { };
            _named = (n, t) => n.As(t);
        }

        public IIocBuilder AsDependency()
        {
            _scope = s => { };
            return this;
        }

        public IIocBuilder AsRequest()
        {
            _scope = s => s.InstancePerRequest();
            return this;
        }

        public IIocBuilder AsSingleton()
        {
            _scope = s => s.SingleInstance();
            return this;
        }
        
        public IIocBinder Named(string name)
        {
            _named = (n, t) => n.Named(t, name);
            return this;
        }

        public void Bind(Type from, params Type[] to) =>
            Bind(_builder.RegisterType(from).WithAdapter(), to);

        public void BindGeneric(Type from, params Type[] to) =>
            Bind(_builder.RegisterGeneric(from).WithAdapter(), to);

        public void BindMethod(Func<IIocScope, object> method, params Type[] to) =>
            Bind(_builder.Register(c => c.Resolve<IIocScope>().Then(method)).WithAdapter(), to);

        private void Bind(IRegistrationBuilder binding, params Type[] to)
        {
            foreach (var type in to)
                binding = binding.Then(type, _named);
            binding.Lastly(_scope);

            _scope = s => { };
            _named = (n, t) => n.As(t);
        }

        public IIocScope Build()
        {
            _container = _builder.Build();
            var scope = _container.BeginLifetimeScope();
            return scope.Resolve<IIocScope>();
        }

        public void Dispose() => 
            _container.Dispose();
    }
}
