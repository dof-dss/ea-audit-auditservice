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
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authorization;
using EA.Audit.Common.Idempotency;
using EA.Audit.Common.Data;
using Steeltoe.CloudFoundry.Connector.Redis;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Infrastructure.Behaviours;
using EA.Audit.Common.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

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
            //services.AddHttpContextAccessor();

            var assembly = AppDomain.CurrentDomain.Load("EA.Audit.Common");
            services.AddAutoMapper(assembly);
            services.AddMediatR(assembly);
            services.AddControllers();            

            ConfigureAuthentication(services);
            ConfigureAuditContext(services);
            ConfigureSwagger(services);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddMvc(opt =>
                {
                    opt.Filters.Add(typeof(ValidatorActionFilter));
                }).AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssembly(assembly); });

            services.AddSingleton<IUriService, UriService>();

            services.AddScoped<IRequestManager, RequestManager>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehaviour<,>));
            services.AddTransient<IAuditContextFactory, AuditContextFactory>();

            ConfigureRedis(services);

        }
        protected virtual void ConfigureRedis(IServiceCollection services)
        {
            services.AddRedisConnectionMultiplexer(Configuration);
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services)
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
                        //options.Audience = "3inpv3ubfmag4k97cu5iqsesg8";
                        options.TokenValidationParameters = new TokenValidationParameters() { ValidateAudience = false };
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Auth.ReadAudits, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Auth.ReadAudits, authDomain)));
                options.AddPolicy(Constants.Auth.CreateAudits, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Auth.CreateAudits, authDomain)));
                options.AddPolicy(Constants.Auth.Admin, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Auth.Admin, authDomain)));
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
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton(sp =>
            {
                var builder = new DbContextOptionsBuilder<AuditContext>();
                return builder.UseMySql(Configuration).Options;
            });
        }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
