using LWMS.Application.Parcels.Commands.ScanInbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Authorize(Roles = "Admin,HubManager,Sorter")]
public class InboundController : ApiControllerBase
{
    [HttpPost("scan")]
    public async Task<ActionResult<bool>> Scan(ScanInboundCommand command)
    {
        return await Mediator.Send(command);
    }
}
