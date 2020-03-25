using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EA.Audit.Common.Data;
using System.Threading.Tasks;
using EA.Audit.Common.Infrastructure.Functional;
using EA.Audit.Common.Infrastructure;
using System.Threading;
using EA.Audit.Common.Infrastructure.Extensions;
using FluentValidation;

namespace EA.Audit.Common.Application.Features.Audits.Queries
{
    public class GetAuditDetailsQuery : IRequest<Result<AuditDto>>
    { 
        public long Id { get; set; }
    }

    public class GetAuditDetailsQueryValidator : AbstractValidator<GetAuditDetailsQuery>
    {
        public GetAuditDetailsQueryValidator()
        {
            RuleFor(m => m.Id).GreaterThan(0);
        }
    }

    public class GetAuditDetailsQueryHandler : IRequestHandler<GetAuditDetailsQuery, Result<AuditDto>>
    {
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAuditDetailsQueryHandler> _logger;

        public GetAuditDetailsQueryHandler(IAuditContextFactory dbContextFactory, IMapper mapper, ILogger<GetAuditDetailsQueryHandler> logger)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<Result<AuditDto>> Handle(GetAuditDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
            "----- Handling query: {RequestName} - ({@Request})",
            request.GetGenericTypeName(),
            request);

            var result = await _dbContext.Audits.Include(a => a.AuditApplication).SingleOrDefaultAsync(a => a.Id == request.Id);

            return result.ToMaybe().ToResult(Constants.ErrorMessages.NoItemExists)
              .OnBoth(i => i.IsSuccess
                  ? Result.Ok(_mapper.Map<AuditDto>(i.Value))
                  : Result.Fail<AuditDto>(i.Error));
        }
    }
}