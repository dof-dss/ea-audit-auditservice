using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.CloudFoundry.Connector.Redis;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EA.Audit.Subscriber;
using EA.Audit.Common.Data;
using EA.Audit.Common.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace EA.Audit.SubscriberApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = (o) =>
                o.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(60), null);
            services.AddDbContext<AuditContext>(options => options.UseMySql(Configuration, mySqlOptionsAction),
                                                ServiceLifetime.Transient);

            services.AddRedisConnectionMultiplexer(Configuration);
            services.AddTransient<IAuditContextFactory, AuditContextFactory>();
            services.AddSingleton<IUriService, UriService>();
            services.TryAddSingleton(sp =>
            {
                var builder = new DbContextOptionsBuilder<AuditContext>();
                return builder.UseMySql(Configuration).Options;
            });
            var assembly = AppDomain.CurrentDomain.Load("EA.Audit.Common");
            services.AddAutoMapper(assembly);
            services.AddMediatR(assembly);
            services.AddHostedService<RedisSubscriberService>();
            services.AddControllers();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
