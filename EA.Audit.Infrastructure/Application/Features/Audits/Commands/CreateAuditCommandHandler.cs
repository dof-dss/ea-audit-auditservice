using AutoMapper;
using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.Common.Data;
using EA.Audit.Common.Idempotency;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Infrastructure.Functional;
using EA.Audit.Common.Model;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EA.Audit.Common.Application.Features.Audits.Commands
{
    public class CreateAuditCommand : IRequest<Result<long>>
    {
        public CreateAuditCommand()
        {

        }

        public CreateAuditCommand(long subjectId, string subject, long actorId, string actor, string description, string properties, string clientId)
        {
            SubjectId = subjectId;
            Subject = subject;
            ActorId = actorId;
            Actor = actor;
            Description = description;
            Properties = properties;
            ClientId = clientId;
        }
        public long SubjectId { get; set; }
        public string Subject { get; set; }
        public long ActorId { get; set; }
        public string Actor { get; set; }
        public string Description { get; set; }
        public string Properties { get; set; }
        public string ClientId { get; set; }
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

    public class CreateAuditCommandHandler : IRequestHandler<CreateAuditCommand, Result<long>>
    {
        
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateAuditCommandHandler> _logger;

        public CreateAuditCommandHandler(AuditContext auditContext, IMapper mapper, ILogger<CreateAuditCommandHandler> logger)
        {
            _dbContext = auditContext;            
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<long>> Handle(CreateAuditCommand command, CancellationToken cancellationToken)
        {
            var audit = _mapper.Map<AuditEntity>(command);

            _dbContext.ClientId = command.ClientId;

            var auditApplication = _dbContext.AuditApplications.FirstOrDefault(a => a.ClientId == command.ClientId);
            if (auditApplication == null)
            {
                return Result.Fail<long>(Constants.ErrorMessages.NoApplicationFound);
            }
            
            audit.AuditApplicationId = auditApplication.Id;
            _dbContext.Audits.Add(audit);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            _logger.LogInformation("Audit persisted to database with Id -{0}", audit.Id);

            return Result.Ok(audit.Id);
        }
    }


    public class CreateAuditIdentifiedCommandHandler : IdentifiedCommandHandler<CreateAuditCommand, Result<long>>
    {
        public CreateAuditIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateAuditCommand, Result<long>>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override Result<long> CreateResultForDuplicateRequest()
        {
            return Result.Fail<long>(Constants.ErrorMessages.DuplicateRequestForAudit);                // Ignore duplicate requests for processing create audit.
        }
    }
}
