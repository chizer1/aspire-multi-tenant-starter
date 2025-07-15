using System.Data;
using DbLocator;
using DbLocator.Domain;

namespace Db;

public class DbLocatorWrapper(string connectionString)
{
    private readonly Locator locator = new Locator(connectionString);

    public string ConnectionString()
    {
        return locator.SqlConnection.ConnectionString;
    }

    public async Task<IDbConnection> ClientDb(string tenantCode)
    {
        return await locator.GetConnection(
            tenantCode,
            (byte)DatabaseType.Client,
            [DatabaseRole.DataReader, DatabaseRole.DataWriter]
        );
    }

    public async Task<int> AddTenant(string tenantName, string tenantCode)
    {
        return await locator.CreateTenant(tenantName, tenantCode, Status.Active);
    }

    public async Task<List<Database>> GetDatabases()
    {
        return await locator.GetDatabases();
    }

    public async Task<int> AddDatabase(string databaseName)
    {
        return await locator.CreateDatabase(
            databaseName,
            (byte)DatabaseServer.Master,
            (byte)DatabaseType.Client,
            Status.Active
        );
    }

    public async Task<int> AddDatabaseUser(
        int databaseId,
        string databaseUserName,
        string databasePassword
    )
    {
        return await locator.CreateDatabaseUser(
            [databaseId],
            databaseUserName,
            databasePassword,
            true
        );
    }

    public async Task AddDatabaseUserRoles(int databaseUserId)
    {
        await locator.CreateDatabaseUserRole(databaseUserId, DatabaseRole.DataReader, true);
        await locator.CreateDatabaseUserRole(databaseUserId, DatabaseRole.DataWriter, true);
    }

    public async Task<int> AddConnection(int tenantId, int databaseId)
    {
        return await locator.CreateConnection(tenantId, databaseId);
    }

    public async Task<List<Tenant>> GetTenants()
    {
        return await locator.GetTenants();
    }

    public async Task<Tenant> GetTenant(string tenantCode)
    {
        return await locator.GetTenant(tenantCode);
    }

    public async Task DeleteTenant(int tenantId)
    {
        await locator.DeleteTenant(tenantId);
    }

    public async Task UpdateTenant(int tenantId, string tenantName, string tenantCode)
    {
        await locator.UpdateTenant(tenantId, tenantName, tenantCode);
    }

    private enum DatabaseType : byte
    {
        Client = 1
    }

    private enum DatabaseServer : int
    {
        Master = 1
    }
}
