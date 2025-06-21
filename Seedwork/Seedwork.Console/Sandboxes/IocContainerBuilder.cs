using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MySql.Data.MySqlClient;
using Seedwork.IOC;
using Seedwork.IOC.Autofac;
using Seedwork.IOC.Interfaces;
using Seedwork.IOC.Ninject;
using Seedwork.Repositories.Interfaces;
using Seedwork.Repositories.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seedwork.Console.Sandboxes
{
    public class IocContainerBuilder
    {
        public static Task<IIocScope> SetupScope()
        {
            IIocContainer container = new NinjectContainer();

            container.AsSingleton().BindGeneric(typeof(NullLogger<>), typeof(ILogger<>));
            container.AsDependency().Bind<SqlRepository, SqlRepository>();
            
            container.AsSingleton().Named("MySQL").BindMethod(s => new MySqlConnection("server=localhost;port=3306;user id=root; password=T=j9!:u5GR3C; database=world"), typeof(IDbConnection));
            container.AsDependency().Named("MySQL").BindMethod(s => s.Resolve<SqlRepository>(("connection", s.Resolve<IDbConnection>("MySQL"))), typeof(IRepository));

            return Task.FromResult(container.Build());
        }
    }


}
