using FluentValidation;
using MediatR;

namespace Api.Features.Client.Products.AddProduct;

public static class AddProductEndpoint
{
    public static IEndpointRouteBuilder MapAddProduct(this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/products",
                async (
                    string name,
                    string description,
                    decimal price,
                    ISender mediator,
                    IValidator<AddProductCommand> validator
                ) =>
                {
                    var query = new AddProductCommand(name, description, price);

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
