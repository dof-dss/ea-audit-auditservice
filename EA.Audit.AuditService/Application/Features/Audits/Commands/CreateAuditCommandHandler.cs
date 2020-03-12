using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Data;
using EA.Audit.AuditService.Infrastructure.Idempotency;
using EA.Audit.AuditService.Models;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace EA.Audit.AuditService.Application.Features.Audits.Commands
{
    /********************************************************************************
     * CONSIDER PULLING INTO SEPARATE API FOR SCALING INDEPENDENT OF READ (True CQRS)
     * SHOULD BE STATELESS i.e. only writing to the queue
     * A SEPARATE BACKGROUND PROCESS SHOULD READ FROM THE QUEUE AND
     * PERSIST TO THE MYSQL DB (Don't yet have Queues in PaaS...or the expected loads)
     * ******************************************************************************/
    public class CreateAuditCommand : IRequest<long>
    {
        public CreateAuditCommand()
        {

        }

        public CreateAuditCommand(long subjectId, string subject, long actorId, string actor, string description, string properties)
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

     public class CreateAuditValidator : AbstractValidator<CreateAuditCommand>
        {
            public CreateAuditValidator()
            {
                RuleFor(m => m.Subject).NotNull().Length(0, 500);
                RuleFor(m => m.Actor).NotNull().Length(0, 500);
                RuleFor(m => m.Description).NotNull().Length(0, 1000);
                RuleFor(m => m.Properties).NotNull().Length(0, 1000);
            }
        }
    
    public class CreateAuditCommandHandler : IRequestHandler<CreateAuditCommand, long>
    {
        private const string NoClientIdFound = "No ClientId on HttpContext";
        private const string NoApplicationFound = "No Application found for ClientId";
        private readonly HttpContext _httpContext;
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;

        public CreateAuditCommandHandler(IAuditContextFactory dbContextFactory, IMapper mapper, IHttpContextAccessor httpContentAccessor)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
            _httpContext = httpContentAccessor.HttpContext;
        }

        public async Task<long> Handle(CreateAuditCommand request, CancellationToken cancellationToken)
        {
            var audit = _mapper.Map<CreateAuditCommand, AuditEntity>(request);

            //Get Client Id from Token and lookup Application
            var clientId = _httpContext.User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;

            if(clientId == null)
            {
                throw new Exception(NoClientIdFound);
            }

            var auditApplication = _dbContext.AuditApplications.FirstOrDefault(a => a.ClientId == clientId);
            if(auditApplication == null)
            {
                throw new Exception(NoApplicationFound);
            }

            audit.AuditApplication = auditApplication;

            _dbContext.Audits.Add(audit);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return audit.Id;
        }
    }


    public class CreateAuditIdentifiedCommandHandler : IdentifiedCommandHandler<CreateAuditCommand, long>
    {
        public CreateAuditIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateAuditCommand, long>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override long CreateResultForDuplicateRequest()
        {
            return -1;                // Ignore duplicate requests for processing create audit.
        }
    }
}