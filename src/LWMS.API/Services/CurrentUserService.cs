using System.Security.Claims;
using LWMS.Application.Common.Interfaces;

using Microsoft.Extensions.Logging;

namespace LWMS.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Guid? UserId
    {
        get
        {
            var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (val == null)
            {
                var claims = _httpContextAccessor.HttpContext?.User?.Claims.Select(c => c.Type) ?? Enumerable.Empty<string>();
                _logger.LogWarning("CurrentUserService: UserId (NameIdentifier) not found. Available claims: {Claims}", string.Join(", ", claims));
            }
            return Guid.TryParse(val, out var userId) ? userId : null;
        }
    }

    // Giả định chúng ta lưu MerchantId vào Claim "MerchantId" khi login
    public Guid? MerchantId =>
        Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue("MerchantId"), out var merchantId) ? merchantId : null;

    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}
