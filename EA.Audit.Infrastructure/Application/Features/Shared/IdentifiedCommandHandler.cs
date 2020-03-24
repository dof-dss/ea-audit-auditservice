using EA.Audit.Common.Infrastructure.Extensions;
using EA.Audit.Common.Application.Features.Audits.Commands;
using EA.Audit.Common.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Application.Features.Shared
{
    public class IdentifiedCommandHandler<T, R> : IRequestHandler<IdentifiedCommand<T, R>, R>
        where T : IRequest<R>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;
        private readonly ILogger<IdentifiedCommandHandler<T, R>> _logger;

        public IdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<T, R>> logger)
        {
            _mediator = mediator;
            _requestManager = requestManager;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates the result value to return if a previous request was found
        /// </summary>
        /// <returns></returns>
        protected virtual R CreateResultForDuplicateRequest()
        {
            return default(R);
        }

        /// <summary>
        /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
        /// just enqueues the original inner command.
        /// </summary>
        /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
        /// <returns>Return value of inner command or default value if request same ID was found</returns>
        public async Task<R> Handle(IdentifiedCommand<T, R> message, CancellationToken cancellationToken)
        {
            var alreadyExists = await _requestManager.ExistAsync(message.Id).ConfigureAwait(false);
            if (alreadyExists)
            {
                var result = CreateResultForDuplicateRequest();
                return result;
            }
            else
            {
                await _requestManager.CreateRequestForCommandAsync<T>(message.Id).ConfigureAwait(false);
                try
                {
                    var command = message.Command;
                    var commandName = command.GetGenericTypeName();

                    switch (command)
                    {
                        case PublishAuditCommand publishAuditCommand:                         
                            break;                       

                        default:
                            break;
                    }

                    _logger.LogInformation(
                        "----- Sending command: {CommandName} - ({@Command})",
                        commandName,
                        command);

                    var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

                    _logger.LogInformation(
                        "----- Command result: {@Result} - {CommandName} - ({@Command})",
                        result,
                        commandName,
                        command);

                    return result;
                }
                catch
                {
                    return default(R);
                }
            }
        }
    }
}
