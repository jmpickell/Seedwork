using Npgsql;
using Seedwork.Repositories;
using Seedwork.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.Respositories.PostgreSql
{
    internal class PostgreRepository : IRepository
    {
        public void Run()
        {
            var aaa = new NpgsqlConnection();
            aaa.Open();
            aaa.CreateCommand().ExecuteNonQuery();
        }

        public bool Create<TPrimary>(Query<TPrimary> query, Row row)
        {
            throw new NotImplementedException();
        }

        public bool Delete<TPrimary>(Query<TPrimary> query, Row row)
        {
            throw new NotImplementedException();
        }
        public Row Read<TPrimary>(Query<TPrimary> query, params string[] columns)
        {
            throw new NotImplementedException();
        }

        public bool Update<TPrimary>(Query<TPrimary> query, Row row)
        {
            throw new NotImplementedException();
        }
    }
}
