using MediatR;
using Microsoft.Extensions.Logging;
using ProjectAPI.Application.Common.Exceptions;
using ProjectAPI.Application.Common.Interfaces;

namespace ProjectAPI.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;
    private readonly List<Type> _exceptionHandlers;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
        _exceptionHandlers = new List<Type>{
            typeof(ValidationException),
            typeof(ConflictException),
            typeof(NotFoundException),
            typeof(UnauthorizedAccessException),
            typeof(ForbiddenAccessException),
        };
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            Type type = ex.GetType();
            if (!_exceptionHandlers.Contains(type))
            {
                string mensaje = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                _logger.LogError(ex, $"ProjectAPI Response: Unhandled Exception for Request {requestName} {request}. Message:{mensaje}");
            }
            //_logger.LogError(ex, $"ProjectAPI Response: Handled Exception {type.Name} for Request {requestName} {request}. Message:{ex.Message}");
            // else
            throw;
        }
    }
}