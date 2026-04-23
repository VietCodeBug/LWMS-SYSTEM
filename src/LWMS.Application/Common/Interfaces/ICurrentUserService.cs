namespace LWMS.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? MerchantId { get; }
    string? Role { get; }
}
