using FluentValidation;
using MediatR;

namespace Api.Features.Client.Products.UpdateProduct;

public static class UpdateProductEndpoint
{
    public static IEndpointRouteBuilder MapUpdateProduct(this IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/products",
                async (
                    int id,
                    string name,
                    string description,
                    decimal price,
                    ISender mediator,
                    IValidator<UpdateProductCommand> validator
                ) =>
                {
                    var query = new UpdateProductCommand(id, name, description, price);

                    var validation = await validator.ValidateAsync(query);
                    if (!validation.IsValid)
                        return Results.ValidationProblem(validation.ToDictionary());

                    await mediator.Send(query);
                    return Results.Ok();
                }
            )
            .WithTags("Client")
            .RequireAuthorization();

        return app;
    }
}
