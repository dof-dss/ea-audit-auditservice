using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EA.Audit.Common.Idempotency;
using EA.Audit.Common.Model;
using EA.Audit.Common.Model.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EA.Audit.Common.Data
{

    public class AuditContext : DbContext
    {
        public string _clientId;
        private IDbContextTransaction _currentTransaction;
        public DbSet<AuditEntity> Audits { get; set; }
        public DbSet<AuditApplication> AuditApplications { get; set; }
        public DbSet<ClientRequest> ClientRequests { get; set; }

        public string ClientId {    
                                    get { 
                                            return IsAdmin ? "Admin" : _clientId; 
                                        }
                                    set { 
                                            _clientId = value; 
                                        } 
                                }

        public bool IsAdmin { get; }

        public AuditContext(DbContextOptions options, string clientId)
            : base(options)
        {
            _clientId = clientId;
        }

        public AuditContext(DbContextOptions options, bool IsAdmin)
           : base(options)
        {
            this.IsAdmin = IsAdmin;
        }

        public AuditContext(DbContextOptions<AuditContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEntity>().HasQueryFilter(b => IsAdmin || EF.Property<string>(b, nameof(ClientId)) == ClientId).HasOne(a => a.AuditApplication);

            modelBuilder.Entity<AuditApplication>().HasQueryFilter(b => IsAdmin || EF.Property<string>(b, nameof(ClientId)) == ClientId);
 
            base.OnModelCreating(modelBuilder);
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                                e.State == EntityState.Added
                                || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).DateModified = DateTime.Now;

                if (entityEntry.State != EntityState.Added) continue;

                ((BaseEntity)entityEntry.Entity).DateCreated = DateTime.Now;

                if (entityEntry.Metadata.GetProperties().Any(p => p.Name == nameof(ClientId)))
                {
                    entityEntry.CurrentValues[nameof(ClientId)] = ClientId;                    
                }
            }
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);

                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}