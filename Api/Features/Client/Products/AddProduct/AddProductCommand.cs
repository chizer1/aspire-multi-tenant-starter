using MediatR;

namespace Api.Features.Client.Products.AddProduct;

public record AddProductCommand(string Name, string Description, decimal Price) : IRequest<int>;
