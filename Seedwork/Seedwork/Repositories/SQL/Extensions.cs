using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Seedwork.Repositories.SQL
{
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

        public static bool IsValidSqlField<T>(this Query<T> query) => 
            (query.Namespace.IsNullOrEmpty() || query.Namespace.IsValidSqlField()) && query.Table.IsValidSqlField() && query.Column.IsValidSqlField();

        public static bool IsValidSqlField(this IEnumerable<string> strings) =>
            strings.All(s => s.IsValidSqlField());

        public static bool IsValidSqlField(this string str) =>
            Regex.IsMatch(str, "^[a-zA-Z0-9_]+$");

        public static string BuildTableName<T>(this Query<T> query) =>
            string.IsNullOrEmpty(query.Namespace) ? query.Table : query.Namespace + "." + query.Table;

        public static IEnumerable<string> GetColumnNames(this Row row) =>
            row.GetAll().Keys;

        public static bool IsNullOrEmpty(this string str) =>
            string.IsNullOrEmpty(str);
    } 
}
