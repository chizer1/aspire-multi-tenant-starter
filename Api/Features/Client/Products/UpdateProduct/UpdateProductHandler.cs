using Db;
using MediatR;

namespace Api.Features.Client.Products.UpdateProduct;

public class UpdateProductHandler(IClientDbContextFactory factory)
    : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        await using var dbContext = await factory.CreateDbContextAsync();

        var product =
            await dbContext.Products.FindAsync([request.Id], cancellationToken)
            ?? throw new InvalidOperationException($"Product with ID {request.Id} not found.");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
