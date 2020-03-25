using AutoMapper;
using EA.Audit.Common.Data;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Infrastructure.Behaviours;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Reflection;

namespace EA.Audit.AuditService.Tests.Integration
{
    [TestFixture]
    public class BaseIntegration
    {
        protected readonly AuditContext DbContext;
        protected readonly IMediator Mediator;

        protected IAuditContextFactory AuditContextFactory { get; }

        public BaseIntegration()
        {
            var services = new ServiceCollection();

            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(x => x.HttpContext.Request.Headers[Constants.XRequest.XRequestIdHeaderName]).Returns(Guid.NewGuid().ToString());
            mockAccessor.Setup(x => x.HttpContext.Request.Headers["Authorization"]).Returns(Guid.NewGuid().ToString());
            
            mockAccessor.Setup(x => x.HttpContext.Request.Scheme).Returns("http");
            mockAccessor.Setup(x => x.HttpContext.Request.Host).Returns(new HostString("test"));
            services.AddSingleton<IHttpContextAccessor>(x => mockAccessor.Object);

            var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            mockConnectionMultiplexer.Setup(x => x.GetSubscriber(null)).Returns(new FakeSubscriber());
            services.AddSingleton(mockConnectionMultiplexer.Object);

            services.AddLogging();

            services.AddDbContext<AuditContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));
            services.AddSingleton(sp =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AuditContext>();
                return optionsBuilder.UseInMemoryDatabase("InMemoryDbForTesting").Options;
            });

            services.AddAutoMapper(typeof(AuditContext).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(AuditContext).GetTypeInfo().Assembly);

            services.AddSingleton<IUriService, UriService>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient<IAuditContextFactory, AuditContextFactory>();

            var servicesProvider = services.BuildServiceProvider();

            Mediator = servicesProvider.GetRequiredService<IMediator>();

            AuditContextFactory = servicesProvider.GetRequiredService<IAuditContextFactory>();
            DbContext = AuditContextFactory.AuditContext;        
        }

        [TearDown]
        public void CleanUp()
        {
            DbContext.PurgeTable(DbContext.AuditApplications);
            DbContext.PurgeTable(DbContext.Audits);
        }
    }
}
