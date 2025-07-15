using Db;
using DbLocator.Domain;
using MediatR;

namespace Api.Features.Admin.Tenants.GetTenants;

public class GetTenantsQueryHandler(DbLocatorWrapper dbLocatorWrapper)
    : IRequestHandler<GetTenantsQuery, List<Tenant>>
{
    public async Task<List<Tenant>> Handle(
        GetTenantsQuery query,
        CancellationToken cancellationToken
    )
    {
        return await dbLocatorWrapper.GetTenants();
    }
}
