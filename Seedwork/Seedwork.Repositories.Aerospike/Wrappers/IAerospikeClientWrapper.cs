using Aerospike.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Seedwork.Repositories.Aerospike.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class AerospikeClientWrapper : IAerospikeClientWrapper
    {
        private readonly IAerospikeClient _client;
        public AerospikeClientWrapper(IAerospikeClient client) => _client = client;

        public WritePolicy WritePolicyDefault => _client.WritePolicyDefault;
        public Policy ReadPolicyDefault => _client.ReadPolicyDefault;
        public QueryPolicy QueryPolicyDefault => _client.QueryPolicyDefault;

        public bool Delete(WritePolicy policy, Key key) =>
            _client.Delete(policy, key);
        public Record Get(Policy policy, Key key, params string[] binNames) =>
            _client.Get(policy, key, binNames);

        public void Put(WritePolicy policy, Key key, params Bin[] binNames) =>
            _client.Put(policy, key, binNames);

        IRecordSet IAerospikeClientWrapper.Query(QueryPolicy policy, Statement statement) =>
            new RecordSetWrapper(_client.Query(policy, statement));
    }

    public interface IAerospikeClientWrapper
    {
        WritePolicy WritePolicyDefault { get; }
        Policy ReadPolicyDefault { get; }
        QueryPolicy QueryPolicyDefault { get; }

        void Put(WritePolicy policy, Key key, params Bin[] binNames);
        Record Get(Policy policy, Key key, params string[] binNames);
        bool Delete(WritePolicy policy, Key key);
        IRecordSet Query(QueryPolicy policy, Statement statement);
    }
}
