using FluentValidation;
using LWMS.Application.Bags.Commands.AssignShipper;

namespace LWMS.Application.Bags.Commands.AssignShipper;

public class AssignShipperValidator : AbstractValidator<AssignShipperCommand>
{
    public AssignShipperValidator()
    {
        // 🔥 TrackingCode
        RuleFor(x => x.TrackingCode)
            .NotEmpty().WithMessage("TrackingCode không được để trống")
            .MaximumLength(50).WithMessage("TrackingCode tối đa 50 ký tự");

        // 🔥 ShipperId
        RuleFor(x => x.ShipperId)
            .NotEmpty().WithMessage("ShipperId không hợp lệ");

        // 🔥 extra rule (optional nhưng nên có)
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.TrackingCode))
            .WithMessage("TrackingCode không hợp lệ");
    }
}