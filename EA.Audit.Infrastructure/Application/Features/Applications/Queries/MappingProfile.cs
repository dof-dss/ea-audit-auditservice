using AutoMapper;
using EA.Audit.Infrastructure.Model.Admin;
using EA.Audit.Infrastructure.Model;

namespace EA.Audit.Infrastructure.Application.Queries
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuditApplication, ApplicationDto>();
            CreateMap<GetAuditApplicationsQuery, PaginationFilter>();
        }
    }
}
