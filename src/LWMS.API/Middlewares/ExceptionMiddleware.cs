using System.Net;
using System.Text.Json;

namespace LWMS.API.Middlewares
{
    /// <summary>
    /// Middleware bắt toàn bộ unhandled exception — trả về JSON chuẩn thay vì HTML error page.
    /// Đăng ký trong Program.cs: app.UseMiddleware&lt;ExceptionMiddleware&gt;();
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                ArgumentNullException => (HttpStatusCode.BadRequest, exception.Message),
                KeyNotFoundException  => (HttpStatusCode.NotFound, exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Không có quyền truy cập."),
                _ => (HttpStatusCode.InternalServerError, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.")
            };

            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                statusCode = (int)statusCode,
                message
            });

            return context.Response.WriteAsync(result);
        }
    }
}
