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
            if (!row.GetAll().Any() || !query.IsValidSql() || !row.GetColumnNames().IsValidSql())
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

            if (!query.IsValidSql() || !columns.IsValidSql())
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
            if (!row.GetAll().Any() || !query.IsValidSql() || !row.GetColumnNames().IsValidSql())
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
            if (!query.IsValidSql())
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

        public static string AsSql(this IEnumerable<string> columns, string prefix = "", string suffix = "") =>
            columns.Any() ? string.Join(",", columns.Select(x => $"{prefix}{x}{suffix}")) : "*";

        public static bool IsValidSql<T>(this Query<T> query) => 
            (query.Namespace.IsNullOrEmpty() || query.Namespace.IsValidSql()) && query.Table.IsValidSql() && query.Column.IsValidSql();

        public static bool IsValidSql(this IEnumerable<string> strings) =>
            strings.All(s => s.IsValidSql());

        public static bool IsValidSql(this string str) =>
            Regex.IsMatch(str, "^[a-zA-Z0-9_]+$");

        public static string BuildTableName<T>(this Query<T> query) =>
            string.IsNullOrEmpty(query.Namespace) ? query.Table : query.Namespace + "." + query.Table;

        public static IEnumerable<string> GetColumnNames(this Row row, string with = "") =>
            row.GetAll().Keys;

        public static bool IsNullOrEmpty(this string str) =>
            string.IsNullOrEmpty(str);
    } 
}
