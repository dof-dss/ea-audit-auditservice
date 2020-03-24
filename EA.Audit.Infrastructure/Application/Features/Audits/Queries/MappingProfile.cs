using AutoMapper;
using EA.Audit.Common.Application.Features.Shared;
using EA.Audit.Common.Model;

namespace EA.Audit.Common.Application.Features.Audits.Queries
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuditEntity, AuditDto>();
            CreateMap<GetAuditsQuery, PaginationDetails>();
            CreateMap<SearchAuditsQuery, PaginationDetails>();
        }
    }
}