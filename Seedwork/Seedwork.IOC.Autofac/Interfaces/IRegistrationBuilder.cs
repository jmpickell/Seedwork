using Autofac;
using Autofac.Builder;
using System;

namespace Seedwork.IOC.Autofac.Interfaces
{
    internal interface IRegistrationBuilder
    {
        IRegistrationBuilder As(Type type);
        IRegistrationBuilder Named(Type type, string name);
        IRegistrationBuilder SingleInstance();
        IRegistrationBuilder InstancePerRequest();
    }

    internal class RegistrationBuilderAdapter<T, U, V> : IRegistrationBuilder
    {
        private IRegistrationBuilder<T, U, V> _builder;
        public RegistrationBuilderAdapter(IRegistrationBuilder<T, U, V> builder)
        {
            _builder = builder;
        }

        public IRegistrationBuilder As(Type type)
        {
            _builder = _builder.As(type);
            return this;
        }

        public IRegistrationBuilder Named(Type type, string name)
        {
            _builder = _builder.Named(name, type);
            return this;
        }

        public IRegistrationBuilder SingleInstance()
        {
            _builder = _builder.SingleInstance();
            return this;
        }

        public IRegistrationBuilder InstancePerRequest()
        {
            _builder = _builder.InstancePerRequest();
            return this;
        }
    }

}
