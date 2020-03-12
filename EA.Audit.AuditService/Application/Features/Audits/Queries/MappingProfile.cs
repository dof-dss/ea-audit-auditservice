using EA.Audit.AuditService.Models;
using AutoMapper;

namespace EA.Audit.AuditService.Application.Features.Audits.Queries
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