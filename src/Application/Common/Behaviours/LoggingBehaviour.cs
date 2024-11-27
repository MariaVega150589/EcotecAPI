using MediatR;
using Microsoft.Extensions.Logging;
using ProjectAPI.Application.Common.Interfaces;

namespace ProjectAPI.Application.Common.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var response = await next();
        return response;
        /*
        var requestName = typeof(TRequest).Name;

        try
        {
            var response = await next();
            //if (response != null)
            //{
            //    _logger.LogInformation($"ProjectAPI Response: {JsonConvert.SerializeObject(response)}");
            //}

            return response;
        }
        catch (Exception)
        {
            _logger.LogError($"ProjectAPI Request: {requestName} Body: {JsonConvert.SerializeObject(request)}");
            throw;
        }
        */
    }
}