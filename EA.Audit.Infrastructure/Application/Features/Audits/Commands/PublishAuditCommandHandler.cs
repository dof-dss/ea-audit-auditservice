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
using EA.Audit.Infrastructure.Data;
using EA.Audit.Infrastructure.Idempotency;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace EA.Audit.Infrastructure.Application.Features.Audits.Commands
{
    public class PublishAuditCommand : IRequest<long>
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
                RuleFor(m => m.Properties).NotNull().Length(0, 1000);
            }
        }
    
    public class PublishAuditCommandHandler : IRequestHandler<PublishAuditCommand, long>
    {
        private const string NoClientIdFound = "No ClientId on HttpContext";
        private const string NoApplicationFound = "No Application found for ClientId";
        private readonly HttpContext _httpContext;
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<PublishAuditCommandHandler> _logger;

        public PublishAuditCommandHandler(IAuditContextFactory dbContextFactory, IMapper mapper, IHttpContextAccessor httpContentAccessor,
            IConnectionMultiplexer connectionMultiplexer, ILogger<PublishAuditCommandHandler> logger)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
            _httpContext = httpContentAccessor.HttpContext;
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        public async Task<long> Handle(PublishAuditCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Entering Handle()");
            var createCommand = _mapper.Map<CreateAuditCommand>(command);            

            //Temp for Testing without Auth
            /***************
             * TODO Remove when SP is ready to use Auth
             * ************/
            var clientId = "3inpv3ubfmag4k97cu5iqsesg8";
            //Get Client Id from Token and lookup Application
            //var clientId = _httpContext.User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;

            if (clientId == null)
            {
                throw new Exception(NoClientIdFound);
            }


            createCommand.ClientId = clientId;
            var publisher = _connectionMultiplexer.GetSubscriber();
            var result = await publisher.PublishAsync("AuditCommand", JsonConvert.SerializeObject(createCommand), CommandFlags.FireAndForget);
            _logger.LogInformation("Publish Result {0}", result.ToString());
            _logger.LogInformation("Publishing {0}", JsonConvert.SerializeObject(createCommand).ToString());
            _logger.LogInformation("Exiting Handle()");
            return 1;
        }
    }


    public class PublishAuditIdentifiedCommandHandler : IdentifiedCommandHandler<PublishAuditCommand, long>
    {
        public PublishAuditIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<PublishAuditCommand, long>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override long CreateResultForDuplicateRequest()
        {
            return -1;                // Ignore duplicate requests for processing create audit.
        }
    }
}