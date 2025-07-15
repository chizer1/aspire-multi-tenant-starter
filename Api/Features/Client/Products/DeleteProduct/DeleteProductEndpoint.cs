using FluentValidation;
using MediatR;

namespace Api.Features.Client.Products.DeleteProduct;

public static class DeleteProductEndpoint
{
    public static IEndpointRouteBuilder MapDeleteProduct(this IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/products",
                async (int id, ISender mediator, IValidator<DeleteProductCommand> validator) =>
                {
                    var query = new DeleteProductCommand(id);

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
