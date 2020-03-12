using EA.Audit.AuditService.Application.Commands;
using EA.Audit.AuditService.Model.Admin;
using EA.Audit.AuditService.Models;
using AutoMapper;

namespace EA.Audit.AuditService.Application.Features.Audits.Commands
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAuditCommand, AuditEntity>(MemberList.Source);
            CreateMap<CreateAuditApplicationCommand, AuditApplication>(MemberList.Source);
        }
    }
}