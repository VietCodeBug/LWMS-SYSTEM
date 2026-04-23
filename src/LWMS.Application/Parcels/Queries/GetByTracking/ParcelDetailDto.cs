namespace LWMS.Application.Parcels.Queries.GetByTracking;
public class ParcelDetailDto
{
    public string TrackingCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderPhone { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public DateTime? SlaDate { get; set; }
    public List<TrackingLogDto> TrackingLogs { get; set; } = new();
}