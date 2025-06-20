using Seedwork.IOC.Autofac;
using Seedwork.IOC.Interfaces;
using Seedwork.IOC.Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seedwork.Console.Sandboxes
{
    public class IocContainerBuilder
    {
        public static Task<IIocScope> SetupScope()
        {
            var container = new NinjectContainer();



            return Task.FromResult(container.Build());
        }
    }


}
