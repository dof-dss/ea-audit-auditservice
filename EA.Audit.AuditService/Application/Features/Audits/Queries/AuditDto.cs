using System;
using System.ComponentModel.DataAnnotations;
using EA.Audit.AuditService.Model.Admin;
using EA.Audit.AuditService.Models;

namespace EA.Audit.AuditService.Application.Features.Audits.Queries
{
    public class AuditDto
    {
        [Display(Name = "AuditId")]
        public long Id { get; set; }
        public AuditApplication AuditApplication { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DateCreated { get; set; }
        public long SubjectId { get; set; }
        public string Subject { get; set; }
        public long ActorId { get; set; }
        public string Actor { get; set; }
        public string Description { get; set; }
        public string Properties { get; set; }

    }
}