using LWMS.Application.Hubs.Commands.Create;
using LWMS.Application.Hubs.Queries.GetHubList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/v1/admin/hubs")]
public class HubsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateHubCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet]
    public async Task<ActionResult<List<HubBriefDto>>> GetList()
    {
        return await Mediator.Send(new GetHubListQuery());
    }
}
