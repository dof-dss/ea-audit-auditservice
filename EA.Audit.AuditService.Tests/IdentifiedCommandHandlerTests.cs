using EA.Audit.AuditService.Application.Features.Audits.Commands;
using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EA.Audit.AuditServiceTests.Application
{
    [TestFixture]
    public class IdentifiedCommandHandlerTest
    {
        private readonly Mock<IRequestManager> _requestManager;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<ILogger<IdentifiedCommandHandler<CreateAuditCommand, long>>> _loggerMock;

        public IdentifiedCommandHandlerTest()
        {
            _requestManager = new Mock<IRequestManager>();
            _mediator = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<IdentifiedCommandHandler<CreateAuditCommand, long>>>();
        }

        [Test]
        public async Task Handler_sends_command_when_audit_not_exist()
        {
            // Arrange
            var fakeGuid = Guid.NewGuid();
            var fakeAuditCmd = new IdentifiedCommand<CreateAuditCommand, long>(FakeAuditRequest(), fakeGuid);

            _requestManager.Setup(x => x.ExistAsync(It.IsAny<Guid>()))
               .Returns(Task.FromResult(false));

            _mediator.Setup(x => x.Send(It.IsAny<IRequest<long>>(), default(CancellationToken)))
               .Returns(Task.FromResult(10L));

            //Act
            var handler = new IdentifiedCommandHandler<CreateAuditCommand, long>(_mediator.Object, _requestManager.Object, _loggerMock.Object);
            var cltToken = new CancellationToken();
            var result = await handler.Handle(fakeAuditCmd, cltToken);

            //Assert
            Assert.True(result == 10);
            _mediator.Verify(x => x.Send(It.IsAny<IRequest<long>>(), default(CancellationToken)), Times.Once());
        }

        [Test]
        public async Task Handler_sends_no_command_when_audit_already_exists()
        {
            // Arrange
            var fakeGuid = Guid.NewGuid();
            var fakeAuditCmd = new IdentifiedCommand<CreateAuditCommand, long>(FakeAuditRequest(), fakeGuid);

            _requestManager.Setup(x => x.ExistAsync(It.IsAny<Guid>()))
               .Returns(Task.FromResult(true));

            _mediator.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), default(CancellationToken)))
               .Returns(Task.FromResult(true));

            //Act
            var handler = new IdentifiedCommandHandler<CreateAuditCommand, long>(_mediator.Object, _requestManager.Object, _loggerMock.Object);
            var cltToken = new CancellationToken();
            var result = await handler.Handle(fakeAuditCmd, cltToken);

            //Assert
            Assert.False(result == -1);
            _mediator.Verify(x => x.Send(It.IsAny<IRequest<bool>>(), default(CancellationToken)), Times.Never());
        }

        private CreateAuditCommand FakeAuditRequest(Dictionary<string, object> args = null)
        {
            return new CreateAuditCommand(
                subjectId: args != null && args.ContainsKey("subjectId") ? (int)args["subjectId"] : 0,
                subject: args != null && args.ContainsKey("subject") ? (string)args["subject"] : null,
                actorId: args != null && args.ContainsKey("actorId") ? (int)args["actorId"] : 0,
                actor: args != null && args.ContainsKey("actor") ? (string)args["actor"] : null,
                description: args != null && args.ContainsKey("description") ? (string)args["description"] : null,
                properties: args != null && args.ContainsKey("properties") ? (string)args["properties"] : null);

        }
    }
}
