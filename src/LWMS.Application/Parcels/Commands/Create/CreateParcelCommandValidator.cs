using FluentValidation;

namespace LWMS.Application.Parcels.Commands.Create;
public class CreateParcelCommandValidator :AbstractValidator<CreateParcelCommand>
{
    public CreateParcelCommandValidator()
    {
        RuleFor(x=>x.SenderName).NotEmpty();
        RuleFor(x=>x.SenderPhone).NotEmpty().Matches(@"^\d{9,11}$");
        RuleFor(x=>x.ReceiverName).NotEmpty();
        RuleFor(x=>x.ReceiverPhone).NotEmpty().Matches(@"^\d{9,11}$");
        RuleFor(x=>x.Province).NotEmpty();
        RuleFor(x=>x.Weight).GreaterThan(0);
        RuleFor(x=>x.ServiceId).NotEmpty();
    }
}