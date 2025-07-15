using FluentValidation;

namespace Api.Features.Admin.Tenants.GetTenants;

public class GetTenantsQueryValidator : AbstractValidator<GetTenantsQuery>
{
    public GetTenantsQueryValidator() { }
}
