using System.Reflection;
using Microsoft.Extensions.Logging;
using LWMS.Application.Common.Interfaces;
using LWMS.Application.Common.Security;
using MediatR;

namespace LWMS.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

    public AuthorizationBehavior(ICurrentUserService currentUserService, ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated
            if (_currentUserService.UserId == null)
            {
                _logger.LogWarning("Authorization failed: UserId is null for request {RequestType}", typeof(TRequest).Name);
                throw new UnauthorizedAccessException("Unauthenticated");
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        if (_currentUserService.Role == role.Trim())
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                if (!authorized)
                {
                    _logger.LogWarning("Authorization failed: Role mismatch. Required: {RequiredRoles}, Actual: {ActualRole}", 
                        string.Join(", ", authorizeAttributesWithRoles.Select(a => a.Roles)), _currentUserService.Role);
                    throw new ForbiddenAccessException("Forbidden: You don't have permission to perform this action.");
                }
            }
        }

        // User is authorized / Authorization not required
        return await next();
    }
}

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message) : base(message) { }
}
