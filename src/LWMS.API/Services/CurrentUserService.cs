using System.Security.Claims;
using LWMS.Application.Common.Interfaces;

namespace LWMS.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId =>
        Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;

    // Giả định chúng ta lưu MerchantId vào Claim "MerchantId" khi login
    public Guid? MerchantId =>
        Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue("MerchantId"), out var merchantId) ? merchantId : null;

    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}
