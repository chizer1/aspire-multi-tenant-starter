using Api.Models;
using MediatR;

namespace Api.Features.Client.Products.GetProducts;

public record GetProductsQuery() : IRequest<List<Product>>;
