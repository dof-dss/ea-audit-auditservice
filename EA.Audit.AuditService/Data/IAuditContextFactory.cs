using Microsoft.EntityFrameworkCore;

namespace EA.Audit.AuditService.Data
{
    public interface IAuditContextFactory
    {
        AuditContext AuditContext { get; }
    }
}
