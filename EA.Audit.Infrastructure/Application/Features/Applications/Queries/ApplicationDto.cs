using System;
using System.ComponentModel.DataAnnotations;

namespace EA.Audit.Infrastructure.Application.Queries
{
    public class ApplicationDto
    {
        [Display(Name = "ApplicationId")]
        public long Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
    }
}
