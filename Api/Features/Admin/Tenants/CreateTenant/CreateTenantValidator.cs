using FluentValidation;

namespace Api.Features.Admin.Tenants.CreateTenant;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.TenantName)
            .NotEmpty()
            .WithMessage("Tenant name is required.")
            .MaximumLength(50)
            .WithMessage("Tenant name must not exceed 50 characters.");

        RuleFor(x => x.TenantCode)
            .NotEmpty()
            .WithMessage("Tenant code is required.")
            .MaximumLength(10)
            .WithMessage("Tenant code must not exceed 100 characters.");
    }
}
