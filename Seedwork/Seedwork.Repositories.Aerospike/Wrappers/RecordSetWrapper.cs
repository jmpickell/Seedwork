using Aerospike.Client;
using System.Diagnostics.CodeAnalysis;

namespace Seedwork.Repositories.Aerospike.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class RecordSetWrapper : IRecordSet 
    {
        private readonly RecordSet _rs;
        public RecordSetWrapper(RecordSet rs) => _rs = rs;

        public bool Next() => _rs.Next();
        public Record Record => _rs.Record;
    }

    public interface IRecordSet
    {
        Record Record { get; }
        bool Next();
    }
}
