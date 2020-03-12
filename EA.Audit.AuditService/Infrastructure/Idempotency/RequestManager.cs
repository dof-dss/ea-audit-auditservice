using EA.Audit.AuditService.Data;
using System;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Infrastructure.Idempotency
{
    public class RequestManager : IRequestManager
    {
        private readonly AuditContext _context;

        public RequestManager(AuditContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.
                FindAsync<ClientRequest>(id);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exists = await ExistAsync(id).ConfigureAwait(false);

            var request = exists ?
                throw new Exception($"Request with {id} already exists") :
                new ClientRequest()
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            _context.Add(request);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
