using LWMS.Domain.Enums;

namespace LWMS.Application.Parcels.Queries.GetParcelByTracking;

public class ParcelDto
{
    public Guid Id { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public ParcelStatus Status { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty; // Masked in real prod
    public decimal Weight { get; set; }
    public decimal CodAmount { get; set; }
    
    public List<TrackingLogDto> TrackingHistory { get; set; } = new();
}

public class TrackingLogDto
{
    public ParcelStatus ToStatus { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
