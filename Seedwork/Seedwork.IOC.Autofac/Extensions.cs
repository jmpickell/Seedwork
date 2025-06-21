using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Seedwork.IOC.Autofac.Interfaces;

namespace Seedwork.IOC.Autofac
{
    public static class Extensions
    {
        internal static IRegistrationBuilder WithAdapter<T, U, V>(this IRegistrationBuilder<T, U, V> builder) =>
            new RegistrationBuilderAdapter<T, U, V>(builder);

        public static Parameter Convert(this IocParameter parameter) =>
            new NamedParameter(parameter.Name, parameter.Value);
    }
}
