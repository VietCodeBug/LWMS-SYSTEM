using MediatR;
using LWMS.Application.Common.Models;
using LWMS.Infrastructure.Services;

namespace LWMS.API.Endpoints
{
    /// <summary>
    /// Tất cả endpoints liên quan đến Authentication.
    /// API layer chỉ gọi service / trả response — KHÔNG chứa logic.
    /// </summary>
    public static class AuthEndpoints
    {
        public static WebApplication MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/auth/login", async (LWMS.Application.Common.Models.LoginRequest request, JwtService jwtService, LWMS.Infrastructure.Data.AppDbContext dbContext) =>
            {
                var user = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(dbContext.Users, u => u.Phone == request.Phone).Result;
                if (user == null || request.Password != "123456") // Bỏ qua hash password cho test
                    return Results.Unauthorized();

                var token = jwtService.GenerateToken(user.Id, user.FullName, user.Role.ToString(), null);
                return Results.Ok(new { token });
            })
            .AllowAnonymous()
            .WithTags("Auth")
            .WithSummary("Đăng nhập và lấy JWT token");

            app.MapGet("/me", (LWMS.Application.Common.Interfaces.ICurrentUserService currentUserService) =>
            {
                return Results.Ok(new
                {
                    UserId = currentUserService.UserId,
                    MerchantId = currentUserService.MerchantId,
                    Role = currentUserService.Role
                });
            })
            .RequireAuthorization()
            .WithTags("Auth")
            .WithSummary("Lấy thông tin user hiện tại từ Token");

            return app;
        }
    }
}
