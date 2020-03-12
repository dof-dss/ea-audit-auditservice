using EA.Audit.AuditService.Application.Extensions;
using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Data;
using EA.Audit.AuditService.Infrastructure;
using EA.Audit.AuditService.Models;
using AutoMapper;
using MediatR;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EA.Audit.AuditService.Application.Features.Audits.Queries
{
    public class SearchAuditsQuery : IRequest<PagedResponse<AuditDto>>
    {
        public SearchAuditsQuery()
        {
            PageNumber = 1;
            PageSize = 100;
        }
        public SearchAuditsQuery(string descriptionContains, string propertiesContains, int pageNumber, int pageSize)
        {
            DescriptionContains = descriptionContains;
            PropertiesContains = propertiesContains;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public string DescriptionContains { get; set; }
        public string PropertiesContains { get; set; }
    }

    public class SearchAuditsQueryHandler : RequestHandler<SearchAuditsQuery, PagedResponse<AuditDto>>
    {
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly ILogger<SearchAuditsQueryHandler> _logger;

        public SearchAuditsQueryHandler(IAuditContextFactory dbContextFactory, IMapper mapper, IUriService uriService, ILogger<SearchAuditsQueryHandler> logger)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
            _uriService = uriService;
            _logger = logger;
        }

        protected override PagedResponse<AuditDto> Handle(SearchAuditsQuery request)
        {
            int total = 0;
            if (request == null)
            {
                var response = _mapper.ProjectTo<AuditDto>(_dbContext.Audits).OrderBy(a => a.Id).ToList();
                total = _dbContext.Audits.Count();
                return new PagedResponse<AuditDto>(response, total);
            }

            _logger.LogInformation(
                       "----- Handling query: {RequestName} - ({@Request})",
                       request.GetGenericTypeName(),
                       request);

            var pagination = _mapper.Map<PaginationFilter>(request);

            var skip = (request.PageNumber) * request.PageSize;

            var audits = _mapper.ProjectTo<AuditDto>(_dbContext.Audits)
                .Where(a => string.IsNullOrEmpty(request.DescriptionContains) || a.Description.Contains(request.DescriptionContains))
                .Where(a => string.IsNullOrEmpty(request.PropertiesContains) || a.Properties.Contains(request.PropertiesContains))
                .OrderBy(a => a.Id)
                .Skip(skip).Take(request.PageSize).ToList();

            total = _dbContext.Audits.Count();

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, audits, total);

            return paginationResponse;

        }
    }
}


