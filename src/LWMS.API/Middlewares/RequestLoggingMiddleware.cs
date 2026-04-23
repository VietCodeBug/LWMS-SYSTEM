using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LWMS.API.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
            sw.Stop();

            var statusCode = context.Response.StatusCode;
            var level = statusCode >= 500 ? LogLevel.Error : (statusCode >= 400 ? LogLevel.Warning : LogLevel.Information);

            _logger.Log(level, "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms",
                context.Request.Method, context.Request.Path, statusCode, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "HTTP {Method} {Path} failed after {Elapsed}ms",
                context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
