using EA.Audit.Infrastructure;
using EA.Audit.Infrastructure.Model;
using System.Collections.Generic;
using System.Linq;

namespace EA.Audit.AuditService.Application.Features.Shared
{
    public class PaginationHelpers
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, PaginationFilter pagination, List<T> response, int total)    
        {
            var nextPage = pagination.PageNumber >= 1
                ? uriService.GetAllAuditsUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize)).ToString()
                : null;

            var previousPage = pagination.PageNumber - 1 >= 1
                ? uriService.GetAllAuditsUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize)).ToString()
                : null;

            return new PagedResponse<T>
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage,
                Total = total
            };
        }
    }
}