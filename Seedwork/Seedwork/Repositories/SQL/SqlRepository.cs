using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using Seedwork.Repositories.Interfaces;
using Seedwork.Utilities.Specification;
using Seedwork.Utilities.Specification.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Seedwork.Repositories.SQL
{
    public class SqlRepository : IRepository
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private readonly ILogger<SqlRepository> _logger;

        public SqlRepository(Func<IDbConnection> connection, ILogger<SqlRepository> logger)
        {
            _connectionFactory = connection;
            _logger = logger;
        }

        public bool Create<TPrimary>(Query<TPrimary> query, Row row)
        {
            if (!row.GetAll().Any() || !query.IsValidSqlField() || !row.GetColumnNames().IsValidSqlField())
                return false;

            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                var pkColumn = query.Column ?? GetPrimaryKeyColumn();

                command.CommandText = $"INSERT INTO {query.BuildTableName()} ({pkColumn}, {row.GetColumnNames().AsSql()}) VALUES (@key, {row.GetColumnNames().AsSql(prefix: "@")})";
                command.AddParameter("@key", query.Key);
                foreach (var column in row.GetAll())
                    command.AddParameter($"@{column.Key}", column.Value);

                connection.Open();
                var updated = command.ExecuteNonQuery();
                _logger.LogInformation($"{updated} Rows Created"); //TODO change
                connection.Close();

                return updated > 0;
            }
        }

        public Row Read<TPrimary>(Query<TPrimary> query, params string[] columns)
        {
            var row = new Row(new Dictionary<string, object>());

            if (!query.IsValidSqlField() || !columns.IsValidSqlField())
                return row;

            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                var column = query.Column ?? GetPrimaryKeyColumn();

                command.CommandText = $"SELECT {columns.AsSql()} FROM {query.BuildTableName()} WHERE {column} = @key";
                command.AddParameter("@key", query.Key);

                //Execute Query
                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                    if (reader.Read())
                        row = new Row(reader.ToDictionary());
                connection.Close();
            }
            return row;
        }

        public bool Update<TPrimary>(Query<TPrimary> query, Row row)
        {
            if (!row.GetAll().Any() || !query.IsValidSqlField() || !row.GetColumnNames().IsValidSqlField())
                return false;

            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"UPDATE {query.BuildTableName()} SET {row.GetColumnNames().Select(c => $"{c} = @{c}").AsSql()} WHERE {query.Column} = @key"; //UNSAFE
                command.AddParameter("@key", query.Key);
                foreach (var column in row.GetAll())
                    command.AddParameter($"@{column.Key}", column.Value);

                connection.Open();
                var updated = command.ExecuteNonQuery();
                _logger.LogInformation($"{updated} Rows Updated"); //TODO change
                connection.Close();

                return updated > 0;
            }
        }

        public bool Delete<TPrimary>(Query<TPrimary> query)
        {
            if (!query.IsValidSqlField())
                return false;

            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                var column = query.Column ?? GetPrimaryKeyColumn();

                command.CommandText = $"DELETE FROM {query.BuildTableName()} WHERE {column} = @key";
                command.AddParameter("@key", query.Key);

                connection.Open();
                var updated = command.ExecuteNonQuery();
                _logger.LogInformation($"{updated} Rows Deleted");
                connection.Close();

                return updated > 0;
            }
        }

        public IEnumerable<Row> Read(Query<ISpecification<Row>> query, params string[] columns)
        {
            if (!query.IsValidSqlField() || !columns.IsValidSqlField())
                yield break;

            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                IList<Filter> filters = new List<Filter>();
                command.CommandText = $"SELECT {columns.AsSql()} FROM {query.BuildTableName()} WHERE {BuildFilterQuery(query.Key, ref filters)}";
                for(int i = 0; i < filters.Count; i++)
                    command.AddParameter($"@{filters[i].Column}{i}", filters[i].Value);

                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        yield return new Row(reader.ToDictionary());
                connection.Close();
            }
        }

        //// TODO make work

        //public bool Update(Query<Filter> query, Row row)
        //{
        //    throw new NotImplementedException();
        //}

        private string GetPrimaryKeyColumn() 
        {
            throw new NotImplementedException();
        }

        private string BuildFilterQuery(ISpecification<Row> query, ref IList<Filter> filters)
        {
            var sb = new StringBuilder();
            if (query is ICompositeSpecification<Row> composite)
            {
                sb.Append('(');
                sb.Append(BuildFilterQuery(composite.Left, ref filters));
                sb.Append(
                    composite is AndSpecification<Row> ? " AND " :
                    composite is OrSpecification<Row> ? " OR " :
                    throw new ArgumentException("Unknown Specification!"));
                sb.Append(BuildFilterQuery(composite.Right, ref filters));
                sb.Append(')');
                return sb.ToString();
            }
            else if (query is NotSpecification<Row> unitary)
            {
                sb.Append(" NOT  (");
                sb.Append(BuildFilterQuery(unitary.Base, ref filters));
                sb.Append(')');
                return sb.ToString();
            }
            else if (query is Filter filter)
            {
                filters.Add(filter);
                return $"{filter.Column} {filter.Operation.GetDescription()} @{filter.Column}{filters.Count-1}";
            }
            throw new ArgumentException("Unknown Specification!");
        }
    }
}
