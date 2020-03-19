using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MediatR;
using AutoMapper;
using System.Reflection;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authorization;
using EA.Audit.Infrastructure.Idempotency;
using EA.Audit.Infrastructure.Data;
using EA.Audit.Infrastructure.Auth;
using EA.Audit.Infrastructure;
using Steeltoe.CloudFoundry.Connector.Redis;

namespace EA.Audit.AuditService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var jwtSettings = JwtSettings.FromConfiguration(Configuration);
            services.AddSingleton(jwtSettings);
            services.AddHttpContextAccessor();

            var assembly = AppDomain.CurrentDomain.Load("EA.Audit.Infrastructure");
            services.AddAutoMapper(assembly);
            services.AddMediatR(assembly);
            services.AddControllers();            

            ConfigureAuthentication(services);
            ConfigureAuditContext(services);
            ConfigureSwagger(services);        

            services.AddMvc(opt =>
                {
                    opt.Filters.Add(typeof(ValidatorActionFilter));
                }).AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });
            
            services.AddSingleton<IUriService>(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new UriService(absoluteUri);
            });

            services.AddScoped<IRequestManager, RequestManager>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient<IAuditContextFactory, AuditContextFactory>();

            services.AddRedisConnectionMultiplexer(Configuration);

        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            string authdomain = "https://cognito-idp.eu-west-2.amazonaws.com/eu-west-2_QtgogH91v";

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                    {
                        options.Authority = authdomain;
                        //options.Audience = "3inpv3ubfmag4k97cu5iqsesg8";
                        options.TokenValidationParameters = new TokenValidationParameters() { ValidateAudience = false };
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("audit-api/read_audits", policy => policy.Requirements.Add(new HasScopeRequirement("audit-api/read_audits", authdomain)));
                options.AddPolicy("audit-api/create_audit", policy => policy.Requirements.Add(new HasScopeRequirement("audit-api/create_audit", authdomain)));
                options.AddPolicy("audit-api/audit_admin", policy => policy.Requirements.Add(new HasScopeRequirement("audit-api/audit_admin", authdomain)));
            });

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Audit Service", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });

        }

        private void ConfigureAuditContext(IServiceCollection services)
        {
            services.AddDbContext<AuditContext>(options => options.UseMySql(Configuration));
            services.TryAddSingleton(sp =>
            {
                var builder = new DbContextOptionsBuilder<AuditContext>();
                return builder.UseMySql(Configuration).Options;
            });
        }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {          
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
             
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AuditContext>();
                if (!context.Database.ProviderName.Contains("Microsoft.EntityFrameworkCore.InMemory"))
                {
                    context.Database.Migrate();
                }
                
                try{
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = serviceScope.ServiceProvider.GetService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            //Disable/refine in Production
            app.UseCors(builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Audit Service V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
