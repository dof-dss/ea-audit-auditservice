using AutoMapper;
using MediatR;
using System.Linq;
using Microsoft.Extensions.Logging;
using EA.Audit.Common.Data;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Application.Features.Shared;
using EA.Audit.Common.Infrastructure.Extensions;
using EA.Audit.Common.Infrastructure.Functional;

namespace EA.Audit.Common.Application.Queries
{
    public class GetAuditApplicationsQuery : IRequest<Result<PagedResponse<ApplicationDto>>>
    {
        public GetAuditApplicationsQuery()
        {
            PageNumber = 1;
            PageSize = 100;
        }
        public GetAuditApplicationsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAuditApplicationQueryHandler : RequestHandler<GetAuditApplicationsQuery, Result<PagedResponse<ApplicationDto>>>
    {
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly ILogger<GetAuditApplicationQueryHandler> _logger;

        public GetAuditApplicationQueryHandler(IAuditContextFactory dbContextFactory, IMapper mapper, IUriService uriService, ILogger<GetAuditApplicationQueryHandler> logger)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
            _uriService = uriService;
            _logger = logger;
        }

        protected override Result<PagedResponse<ApplicationDto>> Handle(GetAuditApplicationsQuery request)
        {  
            _logger.LogInformation(
                        "----- Handling query: {RequestName} - ({@Request})",
                        request.GetGenericTypeName(),
                        request);

            var pagination = _mapper.Map<PaginationDetails>(request).WithTotal(_dbContext.AuditApplications.Count())
                                                                    .WithApiRoute(ApiRoutes.AuditApplications.GetAll);

            var auditApps = _mapper.ProjectTo<ApplicationDto>(_dbContext.AuditApplications.OrderBy(a => a.Id))
                .Skip(pagination.PreviousPageNumber * pagination.PageSize)
                .Take(request.PageSize).ToList();

            var paginationResponse = PagedResponse<ApplicationDto>.CreatePaginatedResponse(_uriService, pagination, auditApps);

            return Result.Ok(paginationResponse);
        }
    }
}
