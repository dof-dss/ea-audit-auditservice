using EA.Audit.AuditService.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EA.Audit.AuditService.Model.Admin
{
    public class AuditApplication : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ApplicationID")]
        public new long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
