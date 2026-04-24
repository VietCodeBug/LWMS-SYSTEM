using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LWMS.Application.Auth.Commands.Login;

public record LoginResponse(string Token, string RefreshToken);

public class LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; set; } = string.Empty; // Có thể là Phone hoặc EmployeeCode
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUnitOfWork uow, IJwtService jwtService, IPasswordService passwordService, ILogger<LoginCommandHandler> logger)
    {
        _uow = uow;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        var user = await _uow.Users.Query()
            .FirstOrDefaultAsync(u => u.Phone == request.Username || u.EmployeeCode == request.Username, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {Username}", request.Username);
            throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không đúng.");
        }

        bool isPasswordValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Invalid password for user: {Username}", request.Username);
            throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không đúng.");
        }

        var token = _jwtService.GenerateToken(user.Id, user.FullName, user.Role.ToString(), user.MerchantId);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };
        await _uow.RefreshTokens.AddAsync(refreshTokenEntity);
        await _uow.SaveChangesAsync();
        
        _logger.LogInformation("Login successful for user: {Username}", request.Username);
        return new LoginResponse(token, refreshToken);
    }
}
