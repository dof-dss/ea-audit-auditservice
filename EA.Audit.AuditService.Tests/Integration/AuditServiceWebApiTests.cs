using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Controllers;
using EA.Audit.Common.Application.Features.Audits.Commands;
using EA.Audit.Common.Application.Features.Audits.Queries;
using EA.Audit.Common.Application.Features.Shared;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Infrastructure.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Tests
{
    [TestFixture]
    public class AuditServiceWebApiTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<AuditController>> _loggerMock;

        public AuditServiceWebApiTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<AuditController>>();
        }

        [Test]
        public async Task Create_audit_with_requestId_success()
        {
            //Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<PublishAuditCommand, Result<string>>>(), default(CancellationToken)))
                .Returns(Task.FromResult(Result.Ok(Constants.SuccessMessages.AuditPublishSuccess)));

            //Act
            var auditController = new AuditController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.CreateAuditAsync(new PublishAuditCommand(), Guid.NewGuid().ToString()) as StatusCodeResult;

            //Assert
            Assert.AreEqual(actionResult.StatusCode, (int)System.Net.HttpStatusCode.Created);

        }

        [Test]
        public async Task Create_audit_bad_request()
        {
            //Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CreateAuditCommand, Result<long>>>(), default(CancellationToken)))
                .Returns(Task.FromResult(Result.Ok(10L)));

            //Act
            var auditController = new AuditController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.CreateAuditAsync(new PublishAuditCommand(), String.Empty) as BadRequestObjectResult;

            //Assert
            Assert.AreEqual(actionResult.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }       

        [Test]
        public async Task Get_audits_success()
        {
            //Arrange
            var fakeDynamicResult = Result.Ok(new PagedResponse<AuditDto>());

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAuditsQuery>(), default))
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var auditController = new AuditController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.GetAuditsAsync( new GetAuditsQuery() ) as OkObjectResult;

            //Assert
            Assert.AreEqual((actionResult as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Test]
        public async Task Get_audit_success()
        {
            //Arrange
            var fakeDynamicResult = Result.Ok(new AuditDto());

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAuditDetailsQuery>(), default(CancellationToken)))
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var auditController = new AuditController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.GetAuditAsync(10) as OkObjectResult;

            //Assert
            Assert.AreEqual((actionResult as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
        }
       
    }
}
