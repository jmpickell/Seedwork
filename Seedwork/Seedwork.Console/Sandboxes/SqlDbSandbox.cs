using Seedwork.IOC.Interfaces;
using Seedwork.Repositories;
using Seedwork.Repositories.Interfaces;
using Seedwork.Repositories.SQL;
using Seedwork.Utilities.Specification;
using Seedwork.Utilities.Specification.Interfaces;
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
        public async static Task Run(IIocScope scope, string db)
        {
            await RunFilter(scope, db);
        }


        public static Task RunFilter(IIocScope scope, string db)
        {
            var apple = (Filter.Check("Banana").Equals(1) & Filter.Check("Apple").Equals(2)) | Filter.Check("Orange").Equals(3);

            var row = new Row(new Dictionary<string, object>
            {
                { "Banana", 2 },
                { "Apple", 2 },
                { "Orange", 3 }
            });

            apple.IsSatisfied(row);

            return Task.CompletedTask;
        }




        public static Task RunPrimary(IIocScope scope, string db)
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
