using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LWMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    private ICurrentUserService? _currentUserService;
    protected ICurrentUserService CurrentUserService => _currentUserService ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
}
