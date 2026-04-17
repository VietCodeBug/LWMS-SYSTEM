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
            app.MapPost("/api/auth/login", Login)
               .AllowAnonymous()
               .WithTags("Auth")
               .WithSummary("Đăng nhập và lấy JWT token");

            return app;
        }

        private static async Task<IResult> Login(
            LoginRequest request,
            JwtService jwtService)
        {
            // TODO: Thay bằng UserService thật sau này
            await Task.CompletedTask;

            if (request.Username != "admin" || request.Password != "123456")
                return Results.Unauthorized();

            var token = jwtService.GenerateToken(request.Username);
            return Results.Ok(new { token });
        }
    }
}
