using FluentValidation;
using LWMS.Application.Bags.Commands.DeliveryFailed;

namespace LWMS.Application.Bags.Commands.DeliveryFailed;

public class DeliveryFailedValidator : AbstractValidator<DeliveryFailedCommand>
{
    public DeliveryFailedValidator()
    {
        // 🔥 TrackingCode
        RuleFor(x => x.TrackingCode)
            .NotEmpty().WithMessage("TrackingCode không được để trống")
            .MaximumLength(50);

        // 🔥 Reason
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Lý do giao thất bại không được để trống")
            .MaximumLength(255);

        // 🔥 extra guard
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("Reason không hợp lệ");
    }
}