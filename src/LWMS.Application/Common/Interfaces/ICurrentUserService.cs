namespace LWMS.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? MerchantId { get; }
    string? Role { get; }
}
