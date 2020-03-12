using System.Collections.Generic;

namespace EA.Audit.AuditService.Application.Features.Shared
{
    public class PagedResponse<T>
    {
        public PagedResponse(){}

        public PagedResponse(IEnumerable<T> data, int total)
        {
            Data = data;
            Total = total;
        }

        public IEnumerable<T> Data { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
        public int? Total { get; set; }
    }
}