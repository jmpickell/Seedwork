using System;
using System.Text;

namespace Seedwork.Repositories.Interfaces
{
    public interface IRepository
    {
        bool Create<TPrimary>(Query<TPrimary> query, Row row);
        Row Read<TPrimary>(Query<TPrimary> query, params string[] columns);
        bool Update<TPrimary>(Query<TPrimary> query, Row row);
        bool Delete<TPrimary>(Query<TPrimary> query);
    }
}
