using LWMS.Application.Common.Security;
using MediatR;

namespace LWMS.Application.Parcels.Commands.Create;

public record CreateParcelResponse(Guid Id, string TrackingCode);

[Authorize(Roles = "Admin")]
public class CreateParcelCommand : IRequest<CreateParcelResponse>
{
    public string SenderName { get; set; } = string.Empty;
    public string SenderPhone { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal CodAmount { get; set; }
    public string? TrackingCode { get; set; }
    public Guid OriginHubId { get; set; }
    public Guid DestHubId { get; set; }
    public Guid ServiceId { get; set; }
    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
}
