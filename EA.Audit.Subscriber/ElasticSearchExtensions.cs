using EA.Audit.Infrastructure.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace EA.Audit.Subscriber
{
   public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(
            this IServiceCollection services, IConfiguration configuration)
        {
            var uri = configuration["elasticsearch:credentials:uri"];
            var username = configuration["elasticsearch:credentials:username"];
            var password = configuration["elasticsearch:credentials:password"];
            var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(uri))
                .BasicAuthentication(username, password)
                .DefaultIndex(defaultIndex)
                .DefaultMappingFor<AuditEntity>(m => m
                    .Ignore(p => p.ClientId)
                    .PropertyName(p => p.Id, "id")
                );

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
