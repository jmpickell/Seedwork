using Aerospike.Client;
using Seedwork.Repositories.Aerospike.Wrappers;
using Seedwork.Repositories.Interfaces;
using Seedwork.Repositories.SQL;
using Seedwork.Utilities.Specification;
using Seedwork.Utilities.Specification.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using client = Aerospike.Client;

namespace Seedwork.Repositories.Aerospike
{
    public class AerospikeRepository : IRepository
    {
        private readonly IAerospikeClientWrapper _client;

        public AerospikeRepository(IAerospikeClientWrapper client) =>
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

        public IEnumerable<Row> Read(Query<ISpecification<Row>> query, params string[] columns)
        {
            var policy = _client.QueryPolicyDefault;
            policy.filterExp = BuildFilterQuery(query.Key).AsExpression();

            var statement = new Statement();
            statement.SetNamespace(query.Namespace);
            statement.SetSetName(query.Table);
            //statement.SetFilter();

            var rs = _client.Query(_client.QueryPolicyDefault, statement);
            while (rs.Next())
                yield return new Row(rs.Record?.bins ?? new Dictionary<string, object>());
        }

        public bool Update<TPrimary>(Query<TPrimary> query, Row row)
        {
            var key = new Key(query.Namespace, query.Table, (dynamic)query.Key);
            _client.Put(_client.WritePolicyDefault, key,
                row.GetAll().Select(o => new Bin(o.Key, o.Value)).ToArray());
            return true;
        }

        private Exp BuildFilterQuery(ISpecification<Row> query)
        {
            var sb = new StringBuilder();
            if (query is ICompositeSpecification<Row> composite)
            {
                var left  = BuildFilterQuery(composite.Left);
                var right = BuildFilterQuery(composite.Right);
                return 
                    query is AndSpecification<Row> ? Exp.And(left, right) :
                    query is OrSpecification<Row>  ? Exp.Or(left, right)  :
                    throw new ArgumentException("Unknown Specification!");
            }

            if (query is NotSpecification<Row> unitary)
                return Exp.Not(BuildFilterQuery(unitary.Base));
            
            if (query is Filter filter)
            {
                var bin = Exp.Bin(filter.Column, filter.Value.GetBinType());
                var val = Exp.Val((dynamic)filter.Value);
                switch (filter.Operation)
                {
                    case Operation.Equals           : return Exp.EQ(bin, val);
                    case Operation.NotEquals        : return Exp.NE(bin, val);
                    case Operation.GreaterThan      : return Exp.GT(bin, val);
                    case Operation.GreaterThanEquals: return Exp.GE(bin, val);
                    case Operation.LessThan         : return Exp.LT(bin, val);
                    case Operation.LessThanEquals   : return Exp.LE(bin, val);
                    default: throw new ArgumentException("Unknown Specification!");
                }
            }
            throw new ArgumentException("Unknown Specification!");
        }
    }

    public static class Extensions
    {
        private static Dictionary<Type, Exp.Type> binType =
            new Dictionary<Type, Exp.Type>
            {
                { typeof(int), Exp.Type.INT },
                { typeof(long), Exp.Type.INT },
                { typeof(bool), Exp.Type.BOOL },
                { typeof(float), Exp.Type.FLOAT },
                { typeof(double), Exp.Type.FLOAT },
                { typeof(char), Exp.Type.STRING },
                { typeof(string), Exp.Type.STRING },
                { typeof(byte[]), Exp.Type.BLOB },
            };

        public static Expression AsExpression(this Exp exp)
        {
            var packer = new Packer();
            exp.Pack(packer);
            return Expression.FromBytes(packer.ToByteArray());
        }

        public static Exp.Type GetBinType(this object bin) =>
            binType.TryGetValue(bin.GetType(), out var value) ? value : throw new KeyNotFoundException();
    }
}
