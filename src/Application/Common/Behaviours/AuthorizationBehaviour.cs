using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using ProjectAPI.Application.Common.Exceptions;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Security;

namespace ProjectAPI.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<TRequest> _logger;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService,
        ILogger<TRequest> logger,
        IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>(true);
        if (authorizeAttributes.Any())
        {
            try
            {
                // Must be authenticated user
                if (_currentUserService.UserId == null)
                    throw new UnauthorizedAccessException("Sin autorización vigente");
                if (!(await _identityService.ValidateUserToken(_currentUserService.UserId)))
                    throw new UnauthorizedAccessException("Token inválido");
                // Role-based authorization
                var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

                if (authorizeAttributesWithRoles.Any())
                {
                    var authorized = false;

                    foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                    {
                        foreach (var role in roles)
                        {
                            var isInRole = await _identityService.IsInRoleAsync(_currentUserService.UserId, role.Trim());
                            if (isInRole)
                            {
                                authorized = true;
                                break;
                            }
                        }
                    }

                    // Must be a member of at least one role in roles
                    if (!authorized)
                    {
                        throw new ForbiddenAccessException("Acceso restringido");
                    }
                }
                // Policy-based authorization
                var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
                if (authorizeAttributesWithPolicies.Any())
                {
                    var authorized = false;
                    foreach (var policies in authorizeAttributesWithPolicies.Select(a => a.Policy.Split(',')))
                    {
                        foreach (var policy in policies)
                        {
                            authorized = await _identityService.AuthorizeAsync(_currentUserService.UserId, policy);

                            if (authorized)
                            {
                                authorized = true;
                                break;
                            }
                        }
                    }
                    if (!authorized)
                    {
                        throw new UnauthorizedAccessException("User does not have the required permission.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Template Response: Handled Exception for Request {request}. Message:{ex.Message}");
                throw;
            }
        }

        //// User is authorized / authorization not required
        return await next();
    }
}