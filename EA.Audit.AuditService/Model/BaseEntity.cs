using System;

namespace EA.Audit.AuditService.Models
{
    public abstract class BaseEntity
    {
        public string ClientId { get; set; }
        public long Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}