using EA.Audit.AuditService.Application.Features.Shared;
using System;

namespace EA.Audit.Infrastructure
{
    public interface IUriService
    {
        Uri GetAuditUri(string postId);
        Uri GetAllAuditsUri(PaginationQuery pagination = null);
    }
}