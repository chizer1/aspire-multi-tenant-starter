using Db;
using Db.Entities;
using MediatR;

namespace Api.Features.Client.Products.AddProduct;

public class AddProductHandler(IClientDbContextFactory factory)
    : IRequestHandler<AddProductCommand, int>
{
    public async Task<int> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        await using var dbContext = await factory.CreateDbContextAsync();

        var product = new ProductEntity
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
