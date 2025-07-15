using MediatR;

namespace Api.Features.Client.Products.UpdateProduct;

public record UpdateProductCommand(int Id, string Name, string Description, decimal Price)
    : IRequest;
