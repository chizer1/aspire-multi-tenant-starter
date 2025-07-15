using Db;
using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Admin.Tenants.CreateTenant;

public class CreateTenantCommandHandler(DbLocatorWrapper dbLocatorWrapper, TenantStore tenantStore)
    : IRequestHandler<CreateTenantCommand, int>
{
    public async Task<int> Handle(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var tenantInfo = await CreateAndRetrieveTenantAsync(command);

        if (tenantInfo == null || string.IsNullOrEmpty(tenantInfo.Id))
            throw new InvalidOperationException("Tenant information or Tenant ID is null.");

        var tenantId = tenantInfo.Id;
        if (string.IsNullOrEmpty(tenantInfo.Name))
            throw new InvalidOperationException("Tenant name is null or empty.");

        if (string.IsNullOrEmpty(tenantInfo.Identifier))
            throw new InvalidOperationException("Tenant identifier is null or empty.");

        var databaseName = tenantInfo.Name.ToLower().Replace(" ", "_");
        var databaseId = await dbLocatorWrapper.AddDatabase(databaseName);

        await dbLocatorWrapper.AddConnection(int.Parse(tenantId), databaseId);

        var databaseUserId = await dbLocatorWrapper.AddDatabaseUser(
            databaseId,
            $"{tenantInfo.Identifier}_user",
            Guid.NewGuid().ToString()
        );

        await dbLocatorWrapper.AddDatabaseUserRoles(databaseUserId);

        if (string.IsNullOrEmpty(tenantInfo.Identifier))
            throw new InvalidOperationException("Tenant identifier is null or empty.");

        var builder = new SqlConnectionStringBuilder(dbLocatorWrapper.ConnectionString())
        {
            InitialCatalog = databaseName
        };
        var connectionString = builder.ToString();

        var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        await using var tenantDbContext = new ClientDbContext(optionsBuilder.Options);
        await tenantDbContext.Database.MigrateAsync(cancellationToken);

        return int.Parse(tenantInfo.Id);
    }

    private async Task<TenantInfo?> CreateAndRetrieveTenantAsync(CreateTenantCommand command)
    {
        var createTenantInfo = new TenantInfo
        {
            Name = command.TenantName,
            Identifier = command.TenantCode,
        };

        await tenantStore.TryAddAsync(createTenantInfo);
        return await tenantStore.TryGetAsync(command.TenantCode);
    }
}
