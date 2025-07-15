using FluentValidation;
using MediatR;

namespace Api.Features.Client.Products.GetProducts;

public static class GetProductsEndpoint
{
    public static IEndpointRouteBuilder MapGetProducts(this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/products",
                async (ISender mediator, IValidator<GetProductsQuery> validator) =>
                {
                    var query = new GetProductsQuery();

                    var validation = await validator.ValidateAsync(query);
                    if (!validation.IsValid)
                        return Results.ValidationProblem(validation.ToDictionary());

                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithTags("Client")
            .RequireAuthorization();

        return app;
    }
}
