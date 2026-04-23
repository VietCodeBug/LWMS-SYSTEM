namespace LWMS.Application.Common.Models;

public record ErrorResponse(
    int StatusCode,
    string Message,
    string? ErrorCode = null,
    string? TraceId = null
);
