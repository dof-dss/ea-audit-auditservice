using EA.Audit.AuditService.Tests.Integration;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using StackExchange.Redis;

namespace EA.Audit.AuditService.Tests.Functional
{
    public class TestStartup: Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureAuthentication(IServiceCollection services)
        {
            var authConfig = AuthConfig.FromConfiguration(Configuration);
            var authDomain = authConfig.AuthDomain;

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = authDomain;
                options.TokenValidationParameters = new TokenValidationParameters() { ValidateAudience = false };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Auth.ReadAudits, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Auth.ReadAudits, authDomain)));
                options.AddPolicy(Constants.Auth.CreateAudits, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Auth.CreateAudits, authDomain)));
                options.AddPolicy(Constants.Auth.Admin, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Auth.Admin, authDomain)));
            });

            services.AddSingleton<IAuthorizationHandler, HasScopeTestHandler>();

        }

        protected override void ConfigureRedis(IServiceCollection services)
        {
            var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            mockConnectionMultiplexer.Setup(x => x.GetSubscriber(null)).Returns(new FakeSubscriber());
            services.AddSingleton(mockConnectionMultiplexer.Object);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
