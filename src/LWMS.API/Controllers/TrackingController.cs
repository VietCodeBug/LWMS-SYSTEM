using LWMS.Application.Parcels.Queries.GetParcelByTracking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LWMS.API.Controllers;

[Route("api/v1/tracking")]
[ApiController]
[EnableRateLimiting("Public")]
public class TrackingController : ApiControllerBase
{
    [HttpGet("{code}")]
    public async Task<ActionResult<ParcelDto>> Get(string code)
    {
        var result = await Mediator.Send(new GetParcelByTrackingQuery { TrackingCode = code });
        
        if (result == null) return NotFound();

        // Ẩn thông tin nhạy cảm trước khi trả về cho Public
        result.ReceiverPhone = MaskPhone(result.ReceiverPhone);
        
        return result;
    }

    private string MaskPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 4) return "****";
        return phone.Substring(0, phone.Length - 4) + "****";
    }
}
