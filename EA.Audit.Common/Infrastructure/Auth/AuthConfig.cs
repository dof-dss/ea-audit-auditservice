using Microsoft.Extensions.Configuration;

namespace EA.Audit.Common.Infrastructure.Auth
{
    public class AuthConfig
    {
        public AuthConfig(string authDomain)
        {
            AuthDomain = authDomain;
        }
        public string AuthDomain { get; set; }

        public static AuthConfig FromConfiguration(IConfiguration configuration)
        {
            var authDomain = configuration["issuer"] ?? "defaultAuthDomain";


            return new AuthConfig(authDomain);
        }
    }

    
}
