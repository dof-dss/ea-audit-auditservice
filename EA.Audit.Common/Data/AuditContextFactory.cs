using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EA.Audit.Common.Data
{
    public class AuditContextFactory : IAuditContextFactory
    {
        private readonly HttpContext _httpContext;
        private DbContextOptions _options;

        public AuditContextFactory(IHttpContextAccessor httpContentAccessor,
            DbContextOptions<AuditContext> options)
        {
            _httpContext = httpContentAccessor.HttpContext;
            _options = options;
        }

        public AuditContext AuditContext
        {
            get
            {
                ValidateHttpContext();

                if (IsAdmin(_httpContext))
                {
                    //Admin Context
                    return new AuditContext(_options, true);
                }

                var clientId = GetClientId(_httpContext);
                return new AuditContext(_options, clientId);
            }
        }

        private string GetClientId(HttpContext httpContext)
        {
            var clientId = _httpContext.User?.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
            return clientId;
        }

        private bool IsAdmin(HttpContext httpContext)
        {
            var scopes = _httpContext.User?.FindFirst(c => c.Type == "scope");
            if (scopes != null)
            {
                if (scopes.Value.Split(' ').Any(s => s == "audit-api/audit_admin"))
                {
                    return true;
                }
            }

            return false;
        }

        private void ValidateHttpContext()
        {
            if (this._httpContext == null)
            {
                throw new ArgumentNullException(nameof(_httpContext));
            }
        }
       
    }
}
