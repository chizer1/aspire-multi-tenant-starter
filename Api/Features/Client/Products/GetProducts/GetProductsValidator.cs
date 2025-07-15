using FluentValidation;

namespace Api.Features.Client.Products.GetProducts;

public class GetProductsValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsValidator() { }
}
