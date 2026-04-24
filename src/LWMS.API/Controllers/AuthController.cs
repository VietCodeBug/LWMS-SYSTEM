using LWMS.Application.Auth.Commands.Login;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Route("api/v1/auth")]
[Microsoft.AspNetCore.Authorization.AllowAnonymous]
public class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LWMS.Application.Auth.Commands.Refresh.RefreshResponse>> Refresh(LWMS.Application.Auth.Commands.Refresh.RefreshCommand command)
    {
        return await Mediator.Send(command);
    }
}
