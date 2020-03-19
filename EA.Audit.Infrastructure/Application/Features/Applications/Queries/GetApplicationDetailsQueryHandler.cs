using AutoMapper;
using MediatR;
using System.Linq;
using Microsoft.Extensions.Logging;
using EA.Audit.Infrastructure.Data;
using EA.Audit.Infrastructure.Application.Extensions;

namespace EA.Audit.Infrastructure.Application.Queries
{

    public class GetAuditApplicationDetailsQuery : IRequest<ApplicationDto>
    {
        public long Id { get; set; }
    }

    public class GetAuditLevelDetailsQueryHandler : RequestHandler<GetAuditApplicationDetailsQuery, ApplicationDto>
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

        protected override ApplicationDto Handle(GetAuditApplicationDetailsQuery request)
        {
            _logger.LogInformation(
            "----- Handling query: {RequestName} - ({@Request})",
            request.GetGenericTypeName(),
            request);

            return _mapper.Map<ApplicationDto>(_dbContext.AuditApplications.FirstOrDefault(a => a.Id == request.Id));
        }
    }
}
