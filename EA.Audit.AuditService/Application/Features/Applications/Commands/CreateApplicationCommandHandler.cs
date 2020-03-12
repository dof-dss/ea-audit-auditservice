using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Data;
using EA.Audit.AuditService.Infrastructure.Idempotency;
using EA.Audit.AuditService.Model.Admin;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Application.Commands
{

    public class CreateAuditApplicationCommand : IRequest<long>
    {
        public CreateAuditApplicationCommand()
        {

        }

        public CreateAuditApplicationCommand(string name, string description, string clientId)
        {
            Name = name;
            Description = description;
            ClientId = clientId;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }

    }

    public class CreateApplicationValidator : AbstractValidator<CreateAuditApplicationCommand>
    {
        public CreateApplicationValidator()
        {
            RuleFor(m => m.Name).NotNull().Length(0, 500);
            RuleFor(m => m.Description).NotNull().Length(0, 500);
            RuleFor(m => m.ClientId).NotNull().Length(0, 500);
        }
    }

    public class CreateApplicationCommandHandler : IRequestHandler<CreateAuditApplicationCommand, long>
    {
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;

        public CreateApplicationCommandHandler(IAuditContextFactory dbContextFactory, IMapper mapper)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
        }

        public async Task<long> Handle(CreateAuditApplicationCommand request, CancellationToken cancellationToken)
        {
            var app = _mapper.Map<CreateAuditApplicationCommand, AuditApplication>(request);
            //Overwrite ClientId in context for Application as it will be supplied, 
            //not derived from Token, as Token will be an Admin Token
            _dbContext.ClientId = request.ClientId;

            _dbContext.AuditApplications.Add(app);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return app.Id;
        }
    }


    public class CreateApplicationIdentifiedCommandHandler : IdentifiedCommandHandler<CreateAuditApplicationCommand, long>
    {
        public CreateApplicationIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateAuditApplicationCommand, long>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override long CreateResultForDuplicateRequest()
        {
            return -1;                // Ignore duplicate requests for processing create audit.
        }
    }
}
