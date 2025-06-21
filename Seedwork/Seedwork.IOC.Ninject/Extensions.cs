using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.IOC.Ninject
{
    public static class Extensions
    {
        public static IParameter Convert(this IocParameter parameter) =>
            new Parameter(parameter.Name, parameter.Value, true);
    }
}
