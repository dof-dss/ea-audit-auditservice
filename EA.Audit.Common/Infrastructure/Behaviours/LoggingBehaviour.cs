using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EA.Audit.Common.Infrastructure.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation($"Handling {typeof(TRequest).Name} at {DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.fff")}");
            var response = await next().ConfigureAwait(false);
            _logger.LogInformation($"Handled {typeof(TResponse).Name} of {typeof(TRequest).Name} at {DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.fff")}");

            return response;
        }
    }
}
