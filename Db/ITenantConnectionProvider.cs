using System.Data.Common;

namespace Db;

public interface ITenantDbConnectionProvider
{
    Task<DbConnection> ClientDb();
}
