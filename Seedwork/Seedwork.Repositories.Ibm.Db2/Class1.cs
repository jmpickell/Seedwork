using Seedwork.Repositories.Interfaces;
using Seedwork.Repositories.SQL;
using System;
using System.Data;
using System.Data.Common;
using IBM;
//using IBM.Data.DB2;

namespace Seedwork.Repositories.Ibm.Db2
{
    public class Db2ConnectionAdapter : ISqlConnection
    {
        private IDbConnection _connection;

        public Db2ConnectionAdapter(IDbConnection connection) =>
            _connection = connection;

        public void Close() =>
            _connection.Close();

        public void Execute(string sql) =>
            _connection.BeginTransaction();

        public void Open() => 
            _connection.Open();
    }

    public class Repository 
    {
        public void Run()
        {
            //IDbConnection connection;
            //var command = connection.CreateCommand();
            //command.CommandText = "BS";
            //command.Execute
        }
    }
}
