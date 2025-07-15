using MediatR;

namespace Api.Features.Admin.Tenants.CreateTenant;

public record CreateTenantCommand(string TenantName, string TenantCode) : IRequest<int>;
