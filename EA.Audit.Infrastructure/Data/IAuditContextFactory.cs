
namespace EA.Audit.Infrastructure.Data
{
    public interface IAuditContextFactory
    {
        AuditContext AuditContext { get; }
    }
}
