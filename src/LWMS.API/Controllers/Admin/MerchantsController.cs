using LWMS.Application.Merchants.Commands.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/v1/admin/merchants")]
public class MerchantsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateMerchantCommand command)
    {
        return await Mediator.Send(command);
    }
}
