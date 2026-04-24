using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Auth.Commands.Login;

public record LoginResponse(string Token, string RefreshToken);

public class LoginCommand : IRequest<LoginResponse>
{
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public LoginCommandHandler(IUnitOfWork uow, IJwtService jwtService, IPasswordService passwordService)
    {
        _uow = uow;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.Query()
            .FirstOrDefaultAsync(u => u.Phone == request.Phone, cancellationToken);

        // NOTE: Trong demo cũ dùng "123456". Trong thực tế production sẽ verify hash.
        bool isPasswordValid = user != null && (request.Password == "123456" || _passwordService.VerifyPassword(request.Password, user.PasswordHash));

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Số điện thoại hoặc mật khẩu không đúng.");
        }

        var token = _jwtService.GenerateToken(user.Id, user.FullName, user.Role.ToString(), user.MerchantId);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(7), // Refresh token expires in 7 days
            UserId = user.Id
        };
        await _uow.RefreshTokens.AddAsync(refreshTokenEntity);
        await _uow.SaveChangesAsync();
        
        return new LoginResponse(token, refreshToken);
    }
}
