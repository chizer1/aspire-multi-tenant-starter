using FluentValidation;
using MediatR;

namespace Api.Features.Admin.Tenants.CreateTenant;

public static class CreateTenantEndpoint
{
    public static IEndpointRouteBuilder MapCreateTenant(this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/tenants",
                async (
                    string tenantName,
                    string tenantCode,
                    ISender mediator,
                    IValidator<CreateTenantCommand> validator
                ) =>
                {
                    var command = new CreateTenantCommand(tenantName, tenantCode);

                    var validation = await validator.ValidateAsync(command);
                    if (!validation.IsValid)
                        return Results.ValidationProblem(validation.ToDictionary());

                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithTags("Admin - Tenants")
            .RequireAuthorization("AdminOnly");

        return app;
    }
}
