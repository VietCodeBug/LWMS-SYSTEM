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

             var (statusCode, message, errorCode) = exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message, "INVALID_ARGUMENT"),
                FluentValidation.ValidationException vex => (HttpStatusCode.BadRequest, string.Join("; ", vex.Errors.Select(e => e.ErrorMessage)), "VALIDATION_ERROR"),
                LWMS.Application.Common.Exceptions.BusinessException => (HttpStatusCode.BadRequest, exception.Message, "BUSINESS_ERROR"),
                LWMS.Application.Common.Behaviors.ForbiddenAccessException => (HttpStatusCode.Forbidden, exception.Message, "FORBIDDEN_ACCESS"),
                Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "Dữ liệu đã được cập nhật bởi một người dùng khác. Vui lòng tải lại trang.", "CONCURRENCY_ERROR"),
                KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message, "NOT_FOUND"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Không có quyền truy cập.", "UNAUTHORIZED"),
                _ => (HttpStatusCode.InternalServerError, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.", "INTERNAL_ERROR")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new LWMS.Application.Common.Models.ErrorResponse(
                (int)statusCode,
                message,
                errorCode,
                context.TraceIdentifier // Dùng TraceIdentifier của ASP.NET Core làm TraceId
            );

            var result = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return context.Response.WriteAsync(result);
        }
    }
}
