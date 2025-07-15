using MediatR;

namespace Api.Features.Client.Products.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest;
