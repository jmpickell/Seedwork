using Seedwork.IOC.Interfaces;
using Seedwork.Repositories;
using Seedwork.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Seedwork.Console.Sandboxes
{
    public class SqlDbSandbox
    {
        public static Task Run(IIocScope scope, string db)
        {
            var repository = scope.Resolve<IRepository>(db);

            var query = new Query<string> { Key = "USA", Column = "code", Table = "country" };

            var row = repository.Read(query, "IndepYear");

            return Task.CompletedTask;
        }
    }
}
