namespace LWMS.Application.Common.Interfaces;

public record TokenResponse(string AccessToken, string RefreshToken);

public interface IJwtService
{
    string GenerateToken(Guid userId, string fullName, string role, Guid? merchantId);
    string GenerateRefreshToken();
}
