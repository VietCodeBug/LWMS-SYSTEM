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

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    
    // Giả định chúng ta lưu MerchantId vào Claim "MerchantId" khi login
    public string? MerchantId => _httpContextAccessor.HttpContext?.User?.FindFirstValue("MerchantId");
    
    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}
