using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using System;
using System.Collections.Generic;
using System.IO;

namespace EA.Audit.Common.Data
{
    public class AuditContextDesignTimeFactory : IDesignTimeDbContextFactory<AuditContext>
    {
        public AuditContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "/../EA.Audit.AuditService/appsettings.json").Build();

            var optionsBuilder = new DbContextOptionsBuilder<AuditContext>();
            optionsBuilder.UseMySql(configuration);

            return new AuditContext(optionsBuilder.Options);
        }
    }
}
