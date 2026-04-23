namespace LWMS.Application.Parcels.Queries.GetByTracking;
public class TrackingLogDto
{
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}