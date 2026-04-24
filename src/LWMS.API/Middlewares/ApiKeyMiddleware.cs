using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using LWMS.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LWMS.API.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string APIKEYNAME = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            await _next(context);
            return;
        }

        var dbContext = context.RequestServices.GetRequiredService<IApplicationDbContext>();
        
        // Luu y: Trong thuc te nen dung cache hoac cache hash de performance tot hon
        // O day chung ta check truc tiep record co ApiKey khop (gia su dang luu plain hoac check hash)
        var merchant = await dbContext.Merchants
            .FirstOrDefaultAsync(m => m.ApiKey == extractedApiKey.ToString() && m.IsActive);

        if (merchant == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key invalid or inactive.");
            return;
        }

        // Gan merchant id vao context de cac handler duoi handle tiep neu can
        context.Items["MerchantId"] = merchant.Id;

        await _next(context);
    }
}
