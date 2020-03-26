using System;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AutoMapper;
using EA.Audit.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EA.Audit.Common.Infrastructure;
using MediatR;
using Steeltoe.CloudFoundry.Connector.Redis;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;

namespace EA.Audit.Subscriber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<RedisSubscriberService>();
                    var assembly = AppDomain.CurrentDomain.Load("EA.Audit.Infrastructure");
                    services.AddAutoMapper(assembly);
                    services.AddMediatR(assembly);

                    services.AddHttpContextAccessor();

                    Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = (o) =>
                        o.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), null);
                    services.AddDbContext<AuditContext>(options => options.UseMySql(hostContext.Configuration, mySqlOptionsAction),
                                                ServiceLifetime.Transient);

                    services.AddElasticsearch(hostContext.Configuration);

                    services.AddRedisConnectionMultiplexer(hostContext.Configuration);
                    services.AddTransient<IAuditContextFactory,AuditContextFactory>();
                    services.AddSingleton<IUriService, UriService>();
                    services.TryAddSingleton(sp =>
                    {
                        var builder = new DbContextOptionsBuilder<AuditContext>();
                        return builder.UseMySql(hostContext.Configuration).Options;
                    });
                })
                .UseWindowsService()
                .AddCloudFoundry();
    }
}