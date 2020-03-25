using EA.Audit.AuditService.Application.Features.Shared;
using AutoMapper;
using MediatR;
using System.Linq;
using Microsoft.Extensions.Logging;
using EA.Audit.Common.Data;
using EA.Audit.Common.Infrastructure.Extensions;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Application.Features.Shared;
using EA.Audit.Common.Infrastructure.Functional;

namespace EA.Audit.Common.Application.Features.Audits.Queries
{
    public class SearchAuditsQuery : IRequest<Result<PagedResponse<AuditDto>>>
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

    public class SearchAuditsQueryHandler : RequestHandler<SearchAuditsQuery, Result<PagedResponse<AuditDto>>>
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

        protected override Result<PagedResponse<AuditDto>> Handle(SearchAuditsQuery request)
        {  
            _logger.LogInformation(
                       "----- Handling query: {RequestName} - ({@Request})",
                       request.GetGenericTypeName(),
                       request);

            var pagination = _mapper.Map<PaginationDetails>(request).WithTotal(_dbContext.Audits.Count());

            var skip = (request.PageNumber) * request.PageSize;

            var audits = _mapper.ProjectTo<AuditDto>(_dbContext.Audits)
                .Where(a => string.IsNullOrEmpty(request.DescriptionContains) || a.Description.Contains(request.DescriptionContains))
                .Where(a => string.IsNullOrEmpty(request.PropertiesContains) || a.Properties.Contains(request.PropertiesContains))
                .OrderBy(a => a.Id)
                .Skip(pagination.PreviousPageNumber * pagination.PageSize).ToList();

            var paginationResponse = PagedResponse<AuditDto>.CreatePaginatedResponse(_uriService, pagination, audits);

            return Result.Ok(paginationResponse);

        }
    }
}


