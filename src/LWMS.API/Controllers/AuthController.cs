using LWMS.Application.Auth.Commands.Login;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Route("api/v1/auth")]
public class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginCommand command)
    {
        return await Mediator.Send(command);
    }
}
