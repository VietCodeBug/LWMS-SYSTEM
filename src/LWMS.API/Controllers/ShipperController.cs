using LWMS.Application.Bags.Commands.DeliveryFailed;
using LWMS.Application.Bags.Commands.DeliverySuccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Authorize(Roles = "Shipper")]
public class ShipperController : ApiControllerBase
{
    [HttpPost("deliver-success")]
    public async Task<ActionResult<bool>> Success(DeliverySuccessCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("deliver-fail")]
    public async Task<ActionResult<bool>> Fail(DeliveryFailedCommand command)
    {
        return await Mediator.Send(command);
    }
}
