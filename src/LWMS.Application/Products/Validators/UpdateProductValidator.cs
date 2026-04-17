using FluentValidation;
using LWMS.Application.Products.Commands.Update;

namespace LWMS.Application.Products.Validators
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        }
    }
}