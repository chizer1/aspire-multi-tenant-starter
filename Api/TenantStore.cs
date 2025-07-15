using Db;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;

namespace Api;

public class TenantStore(DbLocatorWrapper dbLocatorWrapper) : IMultiTenantStore<TenantInfo>
{
    public async Task<TenantInfo?> TryGetAsync(string identifier)
    {
        var tenant = await dbLocatorWrapper.GetTenant(identifier);

        return tenant == null
            ? null
            : new TenantInfo
            {
                Id = tenant.Id.ToString(),
                Name = tenant.Name,
                Identifier = tenant.Code
            };
    }

    public async Task<IEnumerable<TenantInfo>> GetAllAsync()
    {
        var tenants = await dbLocatorWrapper.GetTenants();

        return tenants.Select(t => new TenantInfo
        {
            Id = t.Id.ToString(),
            Name = t.Name,
            Identifier = t.Code
        });
    }

    public async Task<bool> TryAddAsync(TenantInfo tenantInfo)
    {
        if (string.IsNullOrEmpty(tenantInfo.Name) || string.IsNullOrEmpty(tenantInfo.Identifier))
            throw new ArgumentException("Tenant name and identifier cannot be null or empty.");

        await dbLocatorWrapper.AddTenant(tenantInfo.Name, tenantInfo.Identifier);

        return true;
    }

    public async Task<bool> TryUpdateAsync(TenantInfo tenantInfo)
    {
        if (
            string.IsNullOrEmpty(tenantInfo.Id)
            || string.IsNullOrEmpty(tenantInfo.Name)
            || string.IsNullOrEmpty(tenantInfo.Identifier)
        )
            throw new ArgumentException("Tenant ID, name, and identifier cannot be null or empty.");

        var tenantId = int.Parse(tenantInfo.Id);
        await dbLocatorWrapper.UpdateTenant(tenantId, tenantInfo.Name, tenantInfo.Identifier);

        return true;
    }

    public async Task<bool> TryRemoveAsync(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            throw new ArgumentException("Tenant identifier cannot be null or empty.");

        await dbLocatorWrapper.DeleteTenant(int.Parse(identifier));

        return true;
    }

    public async Task<TenantInfo?> TryGetByIdentifierAsync(string identifier)
    {
        var tenant = await dbLocatorWrapper.GetTenant(identifier);

        return tenant == null
            ? null
            : new TenantInfo
            {
                Id = tenant.Id.ToString(),
                Name = tenant.Name,
                Identifier = tenant.Code
            };
    }

    public async Task<IEnumerable<TenantInfo>> GetAllAsync(int take, int skip)
    {
        var tenants = await dbLocatorWrapper.GetTenants();

        return tenants
            .Skip(skip)
            .Take(take)
            .Select(t => new TenantInfo
            {
                Id = t.Id.ToString(),
                Name = t.Name,
                Identifier = t.Code
            });
    }
}
