using EA.Audit.AuditService.Model.Admin;
using EA.Audit.AuditService.Models;
using AutoMapper;

namespace EA.Audit.AuditService.Application.Features.Application.Queries
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
