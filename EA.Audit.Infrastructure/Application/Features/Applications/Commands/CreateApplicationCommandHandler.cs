using EA.Audit.AuditService.Application.Features.Shared;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using EA.Audit.Common.Model.Admin;
using EA.Audit.Common.Data;
using EA.Audit.Common.Idempotency;
using EA.Audit.Common.Infrastructure.Functional;
using EA.Audit.Common.Infrastructure;

namespace EA.Audit.Common.Application.Commands
{

    public class CreateAuditApplicationCommand : IRequest<Result<long>>
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

    public class CreateApplicationCommandHandler : IRequestHandler<CreateAuditApplicationCommand, Result<long>>
    {
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;

        public CreateApplicationCommandHandler(IAuditContextFactory dbContextFactory, IMapper mapper)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
        }

        public async Task<Result<long>> Handle(CreateAuditApplicationCommand command, CancellationToken cancellationToken)
        {
            var app = _mapper.Map<CreateAuditApplicationCommand, AuditApplication>(command);
            //Overwrite ClientId in context for Application as it will be supplied, 
            //not derived from Token, as Token will be an Admin Token
            _dbContext.ClientId = command.ClientId;

            _dbContext.AuditApplications.Add(app);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return Result.Ok(app.Id);
        }
    }


    public class CreateApplicationIdentifiedCommandHandler : IdentifiedCommandHandler<CreateAuditApplicationCommand, Result<long>>
    {
        public CreateApplicationIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateAuditApplicationCommand, Result<long>>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override Result<long> CreateResultForDuplicateRequest()
        {
            return Result.Fail<long>(Constants.ErrorMessages.DuplicateRequestForAuditApplication);                // Ignore duplicate requests for processing create audit.
        }
    }
}
