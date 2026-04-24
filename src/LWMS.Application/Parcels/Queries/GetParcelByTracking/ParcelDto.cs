using LWMS.Domain.Enums;

namespace LWMS.Application.Parcels.Queries.GetParcelByTracking;

public class ParcelDto
{
    public Guid Id { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public ParcelStatus Status { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    private string _receiverPhone = string.Empty;
    public string ReceiverPhone 
    { 
        get => MaskPhone(_receiverPhone);
        set => _receiverPhone = value; 
    }

    private string MaskPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 4) return phone;
        return phone.Substring(0, 3) + "****" + phone.Substring(phone.Length - 3);
    }
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
