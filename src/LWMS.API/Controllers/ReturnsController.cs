using LWMS.Application.Parcels.Commands.ScanInbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Authorize(Roles = "Admin,HubManager,Sorter")]
[Route("api/v1/returns")]
public class ReturnsController : ApiControllerBase
{
    [HttpPost("scan")]
    public async Task<ActionResult<bool>> ScanReturn(ScanInboundCommand command)
    {
        // Sử dụng lại logic ScanInbound nhưng với ghi chú là Hàng hoàn
        command.Note = "SCAN NHẬN HÀNG HOÀN";
        return await Mediator.Send(command);
    }
}
