using Autofac.Builder;
using Seedwork.IOC.Autofac.Interfaces;

namespace Seedwork.IOC.Autofac
{
    public static class Extensions
    {
        internal static IRegistrationBuilder WithAdapter<T, U, V>(this IRegistrationBuilder<T, U, V> builder) =>
            new RegistrationBuilderAdapter<T, U, V>(builder);
    }
}
