using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LWMS.API.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Items[CorrelationIdHeaderName] = correlationId;
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;

        await _next(context);
    }
}
