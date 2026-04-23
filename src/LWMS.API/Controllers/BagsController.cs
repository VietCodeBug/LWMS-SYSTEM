using LWMS.Application.Bags.Commands.AddParcel;
using LWMS.Application.Bags.Commands.Create;
using LWMS.Application.Bags.Commands.Receive;
using LWMS.Application.Bags.Commands.Seal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Authorize(Roles = "Admin,HubManager,Sorter")]
public class BagsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateBagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("add-parcel")]
    public async Task<ActionResult<bool>> AddParcel(AddParcelToBagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("seal")]
    public async Task<ActionResult<bool>> Seal(SealBagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("receive")]
    public async Task<ActionResult<bool>> Receive(ReceiveBagCommand command)
    {
        return await Mediator.Send(command);
    }
}
