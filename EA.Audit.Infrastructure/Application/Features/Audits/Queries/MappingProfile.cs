using AutoMapper;
using EA.Audit.Infrastructure.Model;

namespace EA.Audit.Infrastructure.Application.Features.Audits.Queries
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuditEntity, AuditDto>();
            CreateMap<GetAuditsQuery,PaginationFilter>();
            CreateMap<SearchAuditsQuery, PaginationFilter>();
        }
    }
}