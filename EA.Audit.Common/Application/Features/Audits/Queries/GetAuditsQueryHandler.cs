using System.Linq;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using EA.Audit.Common.Data;
using EA.Audit.Common.Infrastructure.Extensions;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Infrastructure.Functional;
using EA.Audit.Common.Application.Features.Shared;
using FluentValidation;

namespace EA.Audit.Common.Application.Features.Audits.Queries
{
    public class GetAuditsQuery : IRequest<Result<PagedResponse<AuditDto>>>
    { 
        public GetAuditsQuery()
        {
            PageNumber = 1;
            PageSize = 100;
        }
        public GetAuditsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAuditsQueryValidator : AbstractValidator<GetAuditsQuery>
    {
        public GetAuditsQueryValidator()
        {
            RuleFor(m => m.PageNumber).GreaterThan(0);
        }
    }

    public class GetAuditsQueryHandler : RequestHandler<GetAuditsQuery, Result<PagedResponse<AuditDto>>>
    {
        private readonly AuditContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly ILogger<GetAuditsQueryHandler> _logger;

        public GetAuditsQueryHandler(IAuditContextFactory dbContextFactory, IMapper mapper, IUriService uriService, ILogger<GetAuditsQueryHandler> logger)
        {
            _dbContext = dbContextFactory.AuditContext;
            _mapper = mapper;
            _uriService = uriService;
            _logger = logger;
        }       

        protected override Result<PagedResponse<AuditDto>> Handle(GetAuditsQuery request)
        {
            _logger.LogInformation(
                        "----- Handling query: {RequestName} - ({@Request}) with ClientId - {ClientId}",
                        request.GetGenericTypeName(),
                        request,
                        _dbContext.ClientId);

            var pagination = _mapper.Map<PaginationDetails>(request).WithTotal(_dbContext.Audits.Count())
                                                                    .WithApiRoute(ApiRoutes.Audits.GetAll);

            var audits = _mapper.ProjectTo<AuditDto>(_dbContext.Audits.Include(a => a.AuditApplication)).OrderBy(a => a.Id)
                .Skip(pagination.PreviousPageNumber * pagination.PageSize)
                .Take(request.PageSize).ToList();

            var paginationResponse = PagedResponse<AuditDto>.CreatePaginatedResponse(_uriService, pagination, audits);

            return Result.Ok(paginationResponse);
    
        }
    }
}