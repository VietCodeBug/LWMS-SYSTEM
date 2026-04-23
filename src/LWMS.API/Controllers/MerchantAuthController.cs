using Microsoft.AspNetCore.Mvc;
using LWMS.API.Middlewares;

namespace LWMS.API.Controllers;

[Route("api/v1/merchant/auth")]
[ApiController]
public class MerchantAuthController : ControllerBase
{
    [HttpGet("verify")]
    [ApiKey]
    public IActionResult Verify()
    {
        return Ok(new { Message = "API Key is valid. Merchant is authorized.", Timestamp = DateTime.UtcNow });
    }
}
