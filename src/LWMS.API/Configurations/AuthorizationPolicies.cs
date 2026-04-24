using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using LWMS.Domain.Enums;

namespace LWMS.API.Configurations;

public static class AuthorizationPolicies
{
    public const string CanScanInbound = "CanScanInbound";
    public const string CanDeliverParcel = "CanDeliverParcel";
    public const string CanManageMerchant = "CanManageMerchant";

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(CanScanInbound, policy => 
                policy.RequireRole("ADMIN", "SORTER"));
            
            options.AddPolicy(CanDeliverParcel, policy => 
                policy.RequireRole("ADMIN", "SHIPPER"));
                
            options.AddPolicy(CanManageMerchant, policy => 
                policy.RequireRole("ADMIN", "MERCHANT_ADMIN"));
        });

        return services;
    }
}
