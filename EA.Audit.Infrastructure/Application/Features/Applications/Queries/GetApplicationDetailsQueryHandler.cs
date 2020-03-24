using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Logging;
using EA.Audit.Common.Data;
using EA.Audit.Common.Infrastructure.Extensions;
using System.Threading.Tasks;
using EA.Audit.Common.Infrastructure.Functional;
using EA.Audit.Common.Infrastructure;
using System.Threading;

namespace EA.Audit.Common.Application.Queries
{

    public class GetAuditApplicationDetailsQuery : IRequest<Result<ApplicationDto>>
    {
        public long Id { get; set; }
    }

    public class GetAuditLevelDetailsQueryHandler : IRequestHandler<GetAuditApplicationDetailsQuery, Result<ApplicationDto>>
    {
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAuditLevelDetailsQueryHandler> _logger;

        public GetAuditLevelDetailsQueryHandler(IAuditContextFactory dbContextFactory, IMapper mapper, ILogger<GetAuditLevelDetailsQueryHandler> logger)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ApplicationDto>> Handle(GetAuditApplicationDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
            "----- Handling query: {RequestName} - ({@Request})",
            request.GetGenericTypeName(),
            request);

            var result = await _dbContext.AuditApplications.SingleOrDefaultAsync(a => a.Id == request.Id);

            return result.ToMaybe().ToResult(Constants.ErrorMessages.NoItemExists)
              .OnBoth(i => i.IsSuccess
                  ? Result.Ok(_mapper.Map<ApplicationDto>(i.Value))
                  : Result.Fail<ApplicationDto>(i.Error));
        }
    }
}
