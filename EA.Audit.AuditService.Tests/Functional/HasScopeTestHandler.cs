using System.Threading.Tasks;
using EA.Audit.Common.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;

namespace EA.Audit.AuditService.Tests.Functional
{
    public class HasScopeTestHandler: HasScopeHandler
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}