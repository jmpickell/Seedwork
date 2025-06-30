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
    public class DbSandbox
    {
        public async static Task Run(IIocScope scope, string db)
        {
            await RunFilter(scope, db);
        }


        public static Task RunFilter(IIocScope scope, string db)
        {
            var repository = scope.Resolve<IRepository>(db);

            var key =
                Filter.Check("LifeExpectancy").GreaterThan(70) & 
                (Filter.Check("GNP").GreaterThan(10000) & Filter.Check("GNP").LessThan(30000)) &
                Filter.Check("Continent").Equals("Europe");

            var query = new Query<ISpecification<Row>> { Table = "country", Key = key  };

            var result = repository.Read(query).ToArray();

            for(int i = 0; i < result.Length; i++)
            {
                var row = result[i];
                System.Console.Write($"({row.Get("Name")}, {row.Get("Continent")}):");
                System.Console.SetCursorPosition(35, i);
                System.Console.Write($"GNP:${ row.Get("GNP")}");
                System.Console.SetCursorPosition(55, i);
                System.Console.WriteLine($"Life Expectancy:{row.Get("LifeExpectancy")}");
            }
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
