using LWMS.Application.Users.Commands.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/v1/admin/users")]
public class UsersController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Register(RegisterUserCommand command)
    {
        return await Mediator.Send(command);
    }
}
