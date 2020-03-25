using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EA.Audit.Common.Infrastructure.Behaviours
{
    public class ExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public ExceptionBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var response = await next();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception at {typeof(TResponse).Name} of {typeof(TRequest).Name} at {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss.fff}");
                throw;
            }
        }
    }
}
