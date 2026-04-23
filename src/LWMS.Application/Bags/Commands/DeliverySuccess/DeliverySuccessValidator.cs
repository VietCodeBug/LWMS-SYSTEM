using FluentValidation;
using LWMS.Application.Bags.Commands.DeliverySuccess;

namespace LWMS.Application.Bags.Commands.DeliverySuccess;

public class DeliverySuccessValidator : AbstractValidator<DeliverySuccessCommand>
{
    public DeliverySuccessValidator()
    {
        // 🔥 TrackingCode
        RuleFor(x => x.TrackingCode)
            .NotEmpty().WithMessage("TrackingCode không được để trống")
            .MaximumLength(50);

        // 🔥 COD
        RuleFor(x => x.CodAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("COD không được âm");

        // 🔥 sanity check
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.TrackingCode))
            .WithMessage("TrackingCode không hợp lệ");
    }
}