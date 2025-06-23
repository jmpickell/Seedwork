using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Seedwork.Repositories
{
    [ExcludeFromCodeCoverage]
    public class Row
    {
        readonly IReadOnlyDictionary<string, object> _data;

        public Row(IReadOnlyDictionary<string, object> data)
        {
            _data = data;
        }

        public object Get(string column) => 
            _data.TryGetValue(column, out var v) ? v : default;

        public IReadOnlyDictionary<string, object> GetAll() => _data;
    }
}
