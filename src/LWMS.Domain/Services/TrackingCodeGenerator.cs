namespace LWMS.Domain.Services;
public class TrackingCodeGenerator
{
    public static string GenerateTrackingCode(string hubcode,int sequence)
    {
       var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var seq = sequence.ToString().PadLeft(6,'0');
         return $"{hubcode}-{date}-{seq}";
    }
}