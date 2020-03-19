using MediatR;
using System;

namespace EA.Audit.AuditService.Application.Features.Shared
{
    public class IdentifiedCommand<T, R> : IRequest<R>
        where T : IRequest<R>
    {
        public T Command { get; }
        public Guid Id { get; }
        public IdentifiedCommand(T command, Guid id)
        {
            Command = command;
            Id = id;
        }
    }
}
