using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Auth.Commands.Login;

public record LoginResponse(string Token);

public class LoginCommand : IRequest<LoginResponse>
{
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IUnitOfWork uow, IJwtService jwtService)
    {
        _uow = uow;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.Query()
            .FirstOrDefaultAsync(u => u.Phone == request.Phone, cancellationToken);

        if (user == null || request.Password != "123456") // Bám theo logic cũ của bạn
        {
            throw new UnauthorizedAccessException("Số điện thoại hoặc mật khẩu không đúng.");
        }

        var token = _jwtService.GenerateToken(user.Id, user.FullName, user.Role.ToString(), null);
        
        return new LoginResponse(token);
    }
}
