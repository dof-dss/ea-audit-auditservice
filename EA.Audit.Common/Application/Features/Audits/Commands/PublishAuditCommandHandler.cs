using EA.Audit.AuditService.Application.Features.Shared;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using EA.Audit.Common.Data;
using EA.Audit.Common.Idempotency;
using StackExchange.Redis;
using Newtonsoft.Json;
using EA.Audit.Common.Infrastructure.Functional;
using EA.Audit.Common.Infrastructure;

namespace EA.Audit.Common.Application.Features.Audits.Commands
{
    public class PublishAuditCommand : IRequest<Result<string>>
    {
        public PublishAuditCommand()
        {

        }

        public PublishAuditCommand(long subjectId, string subject, long actorId, string actor, string description, string properties)
        {
            SubjectId = subjectId;
            Subject = subject;
            ActorId = actorId;
            Actor = actor;
            Description = description;
            Properties = properties;
        }
        public long SubjectId { get; set; }
        public string Subject { get; set; }
        public long ActorId { get; set; }
        public string Actor { get; set; }
        public string Description { get; set; }    
        public string Properties { get; set; }
    }

     public class PublishAuditValidator : AbstractValidator<PublishAuditCommand>
        {
            public PublishAuditValidator()
            {
                RuleFor(m => m.Subject).NotNull().Length(0, 500);
                RuleFor(m => m.Actor).NotNull().Length(0, 500);
                RuleFor(m => m.Description).NotNull().Length(0, 1000);
                RuleFor(m => m.Properties).NotNull().Length(0, 5000);
            }
        }
    
    public class PublishAuditCommandHandler : IRequestHandler<PublishAuditCommand, Result<string>>
    {
        private readonly HttpContext _httpContext;
        private readonly IMapper _mapper;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<PublishAuditCommandHandler> _logger;

        public PublishAuditCommandHandler(IMapper mapper, IHttpContextAccessor httpContentAccessor,
            IConnectionMultiplexer connectionMultiplexer, ILogger<PublishAuditCommandHandler> logger)
        {
            _mapper = mapper;
            _httpContext = httpContentAccessor.HttpContext;
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(PublishAuditCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Entering Handle()");
            var createCommand = _mapper.Map<CreateAuditCommand>(command); 

            //Get Client Id from Token
            var clientId = _httpContext.User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;

            if (clientId == null)
            {
                return Result.Fail<string>(Constants.ErrorMessages.NoClientIdFound);
            }

            createCommand.ClientId = clientId;
            var publisher = _connectionMultiplexer.GetSubscriber();
            var result = await publisher.PublishAsync(Constants.Redis.AuditChannel, JsonConvert.SerializeObject(createCommand), CommandFlags.FireAndForget);

            _logger.LogDebug("Exiting Handle()");

            return Result.Ok(Constants.SuccessMessages.AuditPublishSuccess);
        }
    }


    public class PublishAuditIdentifiedCommandHandler : IdentifiedCommandHandler<PublishAuditCommand, Result<string>>
    {
        public PublishAuditIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<PublishAuditCommand, Result<string>>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override Result<string> CreateResultForDuplicateRequest()
        {
            return Result.Fail<string>(Constants.ErrorMessages.DuplicateRequestForAudit);
        }
    }
}