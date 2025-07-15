using Db;
using Db.Entities;
using MediatR;

namespace Api.Features.Client.Products.DeleteProduct;

public class DeleteProductHandler(IClientDbContextFactory factory)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await using var dbContext = await factory.CreateDbContextAsync();

        var product = new ProductEntity { Id = request.Id, };

        dbContext.Products.Remove(product);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
