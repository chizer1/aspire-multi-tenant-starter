using FluentValidation;

namespace Api.Features.Client.Products.DeleteProduct;

public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Product ID must be greater than zero.");
    }
}
