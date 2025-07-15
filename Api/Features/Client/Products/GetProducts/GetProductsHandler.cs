using Api.Models;
using Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Client.Products.GetProducts;

public class GetProductsHandler(IClientDbContextFactory factory)
    : IRequestHandler<GetProductsQuery, List<Product>>
{
    public async Task<List<Product>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await factory.CreateDbContextAsync();

        var products = await dbContext.Products.ToListAsync(cancellationToken);

        return
        [
            .. products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            })
        ];
    }
}
