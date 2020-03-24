using EA.Audit.Common.Application.Features.Shared;
using System;

namespace EA.Audit.Common.Infrastructure
{
    public interface IUriService
    {
        Uri CreateNextPageUri(PaginationDetails paginationDetails);
        Uri CreatePreviousPageUri(PaginationDetails paginationDetails);
    }
}