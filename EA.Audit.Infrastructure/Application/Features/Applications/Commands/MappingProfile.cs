using AutoMapper;
using EA.Audit.Common.Application.Commands;
using EA.Audit.Common.Model.Admin;

namespace EA.Audit.Common.Application.Features.Applications.Commands
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAuditApplicationCommand, AuditApplication>(MemberList.Source);
        }
    }
}
