using DbLocator.Domain;
using MediatR;

namespace Api.Features.Admin.Tenants.GetTenants;

public record GetTenantsQuery() : IRequest<List<Tenant>>;
