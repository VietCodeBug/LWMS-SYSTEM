using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LWMS.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUser;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId?.ToString() ?? "anonymous";
        var correlationId = Guid.NewGuid().ToString().Substring(0, 8);

        var stopwatch = Stopwatch.StartNew();

        // 🔥 LOG START (KHÔNG log full object)
        _logger.LogInformation(
            "[START] {RequestName} | User={UserId} | Trace={CorrelationId}",
            requestName,
            userId,
            correlationId
        );

        try
        {
            var response = await next();

            stopwatch.Stop();

            // 🔥 LOG SUCCESS
            _logger.LogInformation(
                "[SUCCESS] {RequestName} | User={UserId} | Trace={CorrelationId} | Time={Elapsed}ms",
                requestName,
                userId,
                correlationId,
                stopwatch.ElapsedMilliseconds
            );

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // 🔥 LOG ERROR
            _logger.LogError(
                ex,
                "[ERROR] {RequestName} | User={UserId} | Trace={CorrelationId} | Time={Elapsed}ms",
                requestName,
                userId,
                correlationId,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }
}