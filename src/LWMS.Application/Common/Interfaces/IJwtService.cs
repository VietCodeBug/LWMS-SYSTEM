namespace LWMS.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string fullName, string role, Guid? merchantId);
}
