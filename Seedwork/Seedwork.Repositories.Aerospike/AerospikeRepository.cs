using Aerospike.Client;
using Seedwork.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using client = Aerospike.Client;

namespace Seedwork.Repositories.Aerospike
{
    public class AerospikeFilter
    {

        public void apple()
        {
            Exp.GT(Exp.MapBin(""), Exp.Val(1));
        }
    }



    public class AerospikeRepository : IRepository
    {
        private readonly IAerospikeClient _client;

        public AerospikeRepository(IAerospikeClient client) =>
            _client = client;

        public bool Create<TPrimary>(Query<TPrimary> query, Row row) =>
            Update(query, row);

        public bool Delete<TPrimary>(Query<TPrimary> query)
        {
            var wp = _client.WritePolicyDefault;
            wp.durableDelete = true;
            var key = new Key(query.Namespace, query.Table, (dynamic)query.Key);
            return _client.Delete(wp, key);
        }

        public Row Read<TPrimary>(Query<TPrimary> query, params string[] columns)
        {
            var key = new Key(query.Namespace, query.Table, (dynamic)query.Key);
            var result = _client.Get(_client.ReadPolicyDefault, key, columns);
            return new Row(result?.bins ?? new Dictionary<string, object>());
        }

        public bool Update<TPrimary>(Query<TPrimary> query, Row row)
        {
            var key = new Key(query.Namespace, query.Table, (dynamic)query.Key);
            _client.Put(_client.WritePolicyDefault, key,
                row.GetAll().Select(o => new Bin(o.Key, o.Value)).ToArray());
            return true;
        }
    }
}
