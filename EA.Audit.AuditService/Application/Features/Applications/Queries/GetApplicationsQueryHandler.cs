using EA.Audit.AuditService.Application.Extensions;
using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Data;
using EA.Audit.AuditService.Infrastructure;
using EA.Audit.AuditService.Models;
using AutoMapper;
using MediatR;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EA.Audit.AuditService.Application.Features.Application.Queries
{
    public class GetAuditApplicationsQuery : IRequest<PagedResponse<ApplicationDto>>
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

    public class GetAuditApplicationQueryHandler : RequestHandler<GetAuditApplicationsQuery, PagedResponse<ApplicationDto>>
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

        protected override PagedResponse<ApplicationDto> Handle(GetAuditApplicationsQuery request)
        {
            int total = 0;
            if (request == null)
            {
                var response = _mapper.ProjectTo<ApplicationDto>(_dbContext.AuditApplications).OrderBy(a => a.Id).ToList();
                total = _dbContext.AuditApplications.Count();
                return new PagedResponse<ApplicationDto>(response, total);
            }

            _logger.LogInformation(
                        "----- Handling query: {RequestName} - ({@Request})",
                        request.GetGenericTypeName(),
                        request);

            var pagination = _mapper.Map<PaginationFilter>(request);

            var skip = (request.PageNumber) * request.PageSize;
            var query = _mapper.ProjectTo<ApplicationDto>(_dbContext.AuditApplications).OrderBy(a => a.Id)
                .Skip(skip).Take(request.PageSize).ToSql();

            var audits = _mapper.ProjectTo<ApplicationDto>(_dbContext.AuditApplications).OrderBy(a => a.Id)
                .Skip(skip).Take(request.PageSize).ToList();

            total = _dbContext.AuditApplications.Count();
            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, audits, total);

            return paginationResponse;

        }
    }
}
