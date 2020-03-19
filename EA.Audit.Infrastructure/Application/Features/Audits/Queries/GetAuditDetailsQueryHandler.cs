using AutoMapper;
using System.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EA.Audit.Infrastructure.Data;
using EA.Audit.Infrastructure.Application.Extensions;

namespace EA.Audit.Infrastructure.Application.Features.Audits.Queries
{
    public class GetAuditDetailsQuery : IRequest<AuditDto>
    { 
        public long Id { get; set; }
    }
    
    public class GetAuditDetailsQueryHandler : RequestHandler<GetAuditDetailsQuery, AuditDto>
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

        protected override AuditDto Handle(GetAuditDetailsQuery request)
        {
            _logger.LogInformation(
            "----- Handling query: {RequestName} - ({@Request})",
            request.GetGenericTypeName(),
            request);

            return _mapper.Map<AuditDto>(_dbContext.Audits.Include(a => a.AuditApplication).FirstOrDefault(a => a.Id == request.Id));
        }
    }
}