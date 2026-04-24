using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Auth.Commands.Refresh;

public record RefreshResponse(string Token, string RefreshToken);

public class RefreshCommand : IRequest<RefreshResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;

    public RefreshCommandHandler(IUnitOfWork uow, IJwtService jwtService)
    {
        _uow = uow;
        _jwtService = jwtService;
    }

    public async Task<RefreshResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _uow.RefreshTokens.Query()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.Revoked == null, cancellationToken);

        if (storedToken == null || storedToken.IsExpired)
        {
            throw new UnauthorizedAccessException("Refresh token không hợp lệ hoặc đã hết hạn.");
        }

        // Rotate token
        storedToken.Revoked = DateTime.UtcNow;
        
        var user = storedToken.User;
        var newToken = _jwtService.GenerateToken(user.Id, user.FullName, user.Role.ToString(), user.MerchantId);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        var newRefreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = newRefreshToken,
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        await _uow.RefreshTokens.AddAsync(newRefreshTokenEntity);
        await _uow.SaveChangesAsync();

        return new RefreshResponse(newToken, newRefreshToken);
    }
}
