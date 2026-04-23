using LWMS.Application.Cod.Commands.Submit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Authorize(Roles = "Shipper")]
public class CodSubmitController : ApiControllerBase
{
    [HttpPost("submit")]
    public async Task<ActionResult<bool>> Submit(SubmitCodCommand command)
    {
        return await Mediator.Send(command);
    }
}
