using System.Data.Common;
using Db;
using Finbuckle.MultiTenant;

namespace Api;

public class TenantDbConnectionProvider(
    DbLocatorWrapper dbLocatorWrapper,
    IHttpContextAccessor httpContextAccessor
) : ITenantDbConnectionProvider
{
    public async Task<DbConnection> ClientDb()
    {
        var context =
            httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");

        var tenant = context.GetMultiTenantContext<TenantInfo>();
        var identifier =
            tenant?.TenantInfo?.Identifier
            ?? throw new InvalidOperationException("Tenant identifier is missing.");

        return (DbConnection)(
            await dbLocatorWrapper.ClientDb(identifier)
            ?? throw new InvalidOperationException("Database connection could not be established.")
        );
    }
}
