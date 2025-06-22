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

            var query = new Query<string> { Key = "SQP", Column = "code", Table = "country" };

            var columns = new Dictionary<string, object> 
            {
                { "IndepYear", -753 }
            };
            var row = new Row(columns);
            
            var result = repository.Create(query, row);
            columns["Continent"] = "Europe";

            result = repository.Update(query, row);

            var result2 = repository.Read(query, "Continent");

            result = repository.Delete(query);

            return Task.CompletedTask;
        }
    }
}
