using Microsoft.EntityFrameworkCore;

namespace Db;

public class ClientDbContextFactory(ITenantDbConnectionProvider tenantProvider)
    : IClientDbContextFactory
{
    public async Task<ClientDbContext> CreateDbContextAsync()
    {
        var connection = await tenantProvider.ClientDb();

        var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
        optionsBuilder.UseSqlServer(connection.ConnectionString);

        return new ClientDbContext(optionsBuilder.Options);
    }
}
