using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Seedwork.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using System.Text.RegularExpressions;

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
            if (!row.GetAll().Any())
                return false;
            var columns = row.GetAll().ToArray();

            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO @table (PrimaryKey, @PrimaryKey) VALUES (@key, {columns.Select(x => $"@{x.Key}").AsSql()})"; // UNSAFE
                command.AddParameter("@table", query.Namespace + "." + query.Table);
                command.AddParameter("@key", query.Key);
                foreach (var column in columns)
                    command.AddParameter($"@{column.Key}", column.Value);

                connection.Open();
                var updated = command.ExecuteNonQuery();
                _logger.LogInformation($"{updated} Rows Created"); //TODO change
                connection.Close();

                return updated > 0;
            }
        }

        public bool Delete<TPrimary>(Query<TPrimary> query)
        {
            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM @table WHERE PrimaryKey = @key";
                command.AddParameter("@table", query.Namespace + "." + query.Table);
                command.AddParameter("@key", query.Key);

                connection.Open();
                var updated = command.ExecuteNonQuery();
                _logger.LogInformation($"{updated} Rows Deleted"); //TODO change
                connection.Close();

                return updated > 0;
            }
        }

        public Row Read<TPrimary>(Query<TPrimary> query, params string[] columns)
        {
            var row = new Row(new Dictionary<string, object>());

            if (!query.IsValidSql() || !columns.IsValidSql())
                return row;

            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                var table  = query.Namespace == null ? query.Table : query.Namespace + "." + query.Table;
                var column = query.Column ?? GetPrimaryKeyColumn();

                command.CommandText = $"SELECT {columns.AsSql()} FROM {table} WHERE {column} = @key";
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

        // TODO make work
        public IEnumerable<Row> Read(Query<Filter> query, params string[] columns)
        {
            using (var connection = _connectionFactory()) 
            using (var command = connection.CreateCommand())
            {
                //Build Command
                var sb = new StringBuilder();
                sb.Append("SELECT @columns FROM @table");
                if (query.Key != null)
                {
                    sb.Append(" WHERE ");
                    //sb.Append();
                }
                command.CommandText = sb.ToString();
                command.AddParameter("@columns", columns.AsSql());
                command.AddParameter("@table", query.Namespace + "." + query.Table);

                //Execute Query
                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                while (reader.Read())
                    yield return new Row(reader.ToDictionary());
                connection.Close();
            }
        }

        public bool Update<TPrimary>(Query<TPrimary> query, Row row)
        {
            if (!row.GetAll().Any()) 
                return false;

            using (var connection = _connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"UPDATE @table SET {row.GetAll().Select(c => $"{c.Key} = @{c.Key}").AsSql()} WHERE PrimaryKey = @key"; //UNSAFE
                command.AddParameter("@table", query.Namespace + "." + query.Table);
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

        public bool Update(Query<Filter> query, Row row)
        {
            throw new NotImplementedException();
        }

        private string GetPrimaryKeyColumn() 
        {
            throw new NotImplementedException();
        }
    }

    public enum Operation
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanEquals,
        LessThanEquals
    }


    public static class Extensions
    {
        public static void AddParameter(this IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;

            command.Parameters.Add(parameter);
        }

        public static IReadOnlyDictionary<string, object> ToDictionary(this IDataReader reader)
        {
            var dictionary = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++) 
                dictionary.Add(reader.GetName(i), reader.GetValue(i));
            return dictionary;
        }

        public static string AsSql(this IEnumerable<string> columns) =>
            columns.Any() ? string.Join(",", columns) : "*";

        public static bool IsValidSql<T>(this Query<T> query)
        {
            return query.Namespace.IsValidSql() && query.Table.IsValidSql() && query.Column.IsValidSql();
        }

        public static bool IsValidSql(this IEnumerable<string> strings) =>
            strings.All(s => s.IsValidSql());

        public static bool IsValidSql(this string str) =>
            Regex.IsMatch(str, "^[a-zA-Z0-9_]+$");
    } 
}
