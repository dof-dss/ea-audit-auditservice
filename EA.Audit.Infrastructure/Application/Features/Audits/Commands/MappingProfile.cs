using AutoMapper;
using EA.Audit.Infrastructure.Model;
using EA.Audit.Infrastructure.Application.Commands;
using EA.Audit.Infrastructure.Model.Admin;

namespace EA.Audit.Infrastructure.Application.Features.Audits.Commands
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAuditCommand, AuditEntity>(MemberList.Source);
            CreateMap<PublishAuditCommand, CreateAuditCommand>(MemberList.Source);
            CreateMap<CreateAuditApplicationCommand, AuditApplication>(MemberList.Source);
        }
    }
}