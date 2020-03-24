using EA.Audit.Common.Model.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EA.Audit.Common.Model
{
    public class AuditEntity : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AuditID")]
        public new long Id { get; set; }

        [Required]
        public long AuditApplicationId { get; set; }
        [ForeignKey("AuditApplicationId")]
        public AuditApplication AuditApplication { get; set; } 
        public long SubjectId { get; set; }
        public string Subject { get; set; }
        public long ActorId { get; set; }
        public string Actor { get; set; }
        public string Description { get; set; }     
        public string Properties { get; set; }

    }
}
