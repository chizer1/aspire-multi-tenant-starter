using FluentValidation;
using MediatR;

namespace Api.Features.Admin.Tenants.GetTenants;

public static class GetTenantsEndpoint
{
    public static IEndpointRouteBuilder MapGetTenants(this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/tenants",
                async (ISender mediator, IValidator<GetTenantsQuery> validator) =>
                {
                    var query = new GetTenantsQuery();

                    var validation = await validator.ValidateAsync(query);
                    if (!validation.IsValid)
                        return Results.ValidationProblem(validation.ToDictionary());

                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithTags("Admin - Tenants")
            .RequireAuthorization("AdminOnly");
        return app;
    }
}
