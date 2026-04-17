using FluentValidation;
using LWMS.Application.Products.Commands.Create;

namespace LWMS.Application.Products.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Tên sản phẩm không được để trống.");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Số lượng sản phẩm không được để trống.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Giá sản phẩm phải lớn hơn 0.");
        }
    }
}