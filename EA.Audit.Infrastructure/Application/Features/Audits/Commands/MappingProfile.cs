using AutoMapper;
using EA.Audit.Common.Model;
using EA.Audit.Common.Application.Commands;
using EA.Audit.Common.Model.Admin;

namespace EA.Audit.Common.Application.Features.Audits.Commands
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