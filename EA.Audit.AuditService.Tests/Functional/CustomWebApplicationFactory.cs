using EA.Audit.Common.Data;
using EA.Audit.Common.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore;

namespace EA.Audit.AuditService.Tests.Functional
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly ServiceProvider _serviceProvider;
        private string _clientId;

        public CustomWebApplicationFactory()
        {
            _serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                .UseStartup<TStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                MockHttpContext(services);
                AddInMemoryDbOptions(services);

                var sp = services.BuildServiceProvider();

                SeedTestData(sp);
            });
        }
        private void MockHttpContext(IServiceCollection services)
        {
            var mockAccessor = new Mock<IHttpContextAccessor>();
            _clientId = Guid.NewGuid().ToString();
            mockAccessor.Setup(x => x.HttpContext.User.Claims).Returns(new List<Claim>() { new Claim("client_id", _clientId) });
            mockAccessor.Setup(x => x.HttpContext.Request.Scheme).Returns("http");
            mockAccessor.Setup(x => x.HttpContext.Request.Host).Returns(new HostString("test"));
            mockAccessor.Setup(x => x.HttpContext.Request.Headers[Constants.XRequest.XRequestIdHeaderName]).Returns("b0ed668d-7ef2-4a23-a333-94ad278f45d7");
            services.AddSingleton<IHttpContextAccessor>(x => mockAccessor.Object);
        }

        private void AddInMemoryDbOptions(IServiceCollection services)
        {
            services.AddSingleton(x =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AuditContext>();
                optionsBuilder.UseInMemoryDatabase("InMemoryDbForTesting");
                optionsBuilder.UseInternalServiceProvider(_serviceProvider);

                return optionsBuilder.Options;
            });
        }

        private void SeedTestData(ServiceProvider sp)
        {
            // Create a scope to obtain a reference to the database
            // context (AuditContext).
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                var options = scopedServices.GetRequiredService<DbContextOptions<AuditContext>>();
                var httpContextAccessor = scopedServices.GetRequiredService<IHttpContextAccessor>();

                var db = new AuditContextFactory(httpContextAccessor, options);

                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                db.AuditContext.Database.EnsureCreated();

                try
                {
                    DbInitializer.Initialize(db.AuditContext);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred initializing the " +
                                        $"database with test messages. Error: {ex.Message}");
                }
            }
        }
    }
}
