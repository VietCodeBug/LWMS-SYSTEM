using System.Data;
using FluentValidation;

namespace LWMS.Application.Parcels.Commands.Create;

public class CreateParcelValidator : AbstractValidator<CreateParcelCommand>
{
    public CreateParcelValidator()
    {
        // 🔥 TRACKING (nếu client gửi)
        RuleFor(x => x.TrackingCode)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.TrackingCode));
        // 🔥 SENDER
        RuleFor(x => x.SenderName)
            .NotEmpty().WithMessage("Tên người gửi không được để trống")
            .MaximumLength(100);
        // 🔥 PHONE
        RuleFor(x => x.SenderPhone)
            .NotEmpty().WithMessage("Số điện thoại người gửi không được để trống")
            .Matches(@"^0\d{9}$").WithMessage("Số điện thoại không hợp lệ");
        // 🔥 RECEIVER
        RuleFor(x => x.ReceiverName)
            .NotEmpty().WithMessage("Tên người nhận không được để trống");
        RuleFor(x => x.ReceiverPhone)
            .NotEmpty().WithMessage("Số điện thoại người nhận không được để trống")
            .Matches(@"^0\d{9}$").WithMessage("Số điện thoại người nhận không hợp lệ");
        // 🔥 WEIGHT
        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Khối lượng phải lớn hơn 0");
        // 🔥 ADDRESS
        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("Địa chỉ không được để trống");
        //COD
        RuleFor(x => x.CodAmount)
            .GreaterThanOrEqualTo(0).WithMessage("COD không được âm");
        //HUB
        RuleFor(x => x.OriginHubId)
            .NotEmpty().WithMessage("Hub gửi không được để trống");
        RuleFor(x => x.DestHubId)
            .NotEmpty().WithMessage("DestHubId không được để trống");
        RuleFor(x => x)
       .Must(x => x.OriginHubId != x.DestHubId)
       .WithMessage("Origin và Destination không được trùng");

        // 🔥 DIMENSION (optional)
        RuleFor(x => x.Length)
            .GreaterThan(0)
            .When(x => x.Length.HasValue);

        RuleFor(x => x.Width)
            .GreaterThan(0)
            .When(x => x.Width.HasValue);

        RuleFor(x => x.Height)
            .GreaterThan(0)
            .When(x => x.Height.HasValue);
    }
}
