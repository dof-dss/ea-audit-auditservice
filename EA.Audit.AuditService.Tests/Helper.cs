using EA.Audit.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace EA.Audit.AuditService.Tests
{
    public static class Helper
    {
        public static void PurgeTable<T>(this AuditContext auditContext, DbSet<T> table) where T : class
        {
            foreach (var row in table)
            {
                auditContext.Set<T>().Remove(row);
            }
            auditContext.SaveChanges();
        }

        public static DbContextOptions<AuditContext> CreateNewContextOptionsUsingInMemoryDatabase()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<AuditContext>();
            builder.UseInMemoryDatabase("EA.Audit")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                     | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

    }
}
