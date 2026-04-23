using LWMS.Application.Bags.Commands.AssignShipper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Authorize(Roles = "Admin,HubManager")]
public class AssignmentController : ApiControllerBase
{
    [HttpPost("shipper")]
    public async Task<ActionResult<bool>> AssignShipper(AssignShipperCommand command)
    {
        return await Mediator.Send(command);
    }
}
