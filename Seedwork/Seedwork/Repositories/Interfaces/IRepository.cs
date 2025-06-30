using Seedwork.Utilities.Specification.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.Repositories.Interfaces
{
    public interface IRepository
    {
        bool Create<TPrimary>(Query<TPrimary> query, Row row);
        Row Read<TPrimary>(Query<TPrimary> query, params string[] columns);
        bool Update<TPrimary>(Query<TPrimary> query, Row row);
        bool Delete<TPrimary>(Query<TPrimary> query);


        IEnumerable<Row> Read(Query<ISpecification<Row>> query, params string[] columns);
    }
}
