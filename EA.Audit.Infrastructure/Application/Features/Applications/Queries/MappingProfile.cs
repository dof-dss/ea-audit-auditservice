using AutoMapper;
using EA.Audit.Common.Model.Admin;
using EA.Audit.Common.Application.Features.Shared;

namespace EA.Audit.Common.Application.Queries
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuditApplication, ApplicationDto>();
            CreateMap<GetAuditApplicationsQuery, PaginationDetails>();
        }
    }
}
