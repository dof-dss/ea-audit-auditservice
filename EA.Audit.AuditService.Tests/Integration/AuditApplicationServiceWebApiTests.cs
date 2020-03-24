using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Controllers;
using EA.Audit.Common.Application.Commands;
using EA.Audit.Common.Application.Features.Shared;
using EA.Audit.Common.Application.Queries;
using EA.Audit.Common.Infrastructure.Functional;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Tests
{

    [TestFixture]
    public class AuditApplicationServiceWebApiTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<AuditApplicationController>> _loggerMock;

        public AuditApplicationServiceWebApiTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<AuditApplicationController>>();
        }

        [Test]
        public async Task Create_application_with_requestId_success()
        {
            //Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CreateAuditApplicationCommand, Result<long>>>(), default(CancellationToken)))
                .Returns(Task.FromResult(Result.Ok(10L)));

            //Act
            var auditController = new AuditApplicationController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.CreateAuditApplicationAsync(new CreateAuditApplicationCommand(), Guid.NewGuid().ToString()) as StatusCodeResult;

            //Assert
            Assert.AreEqual(actionResult.StatusCode, (int)System.Net.HttpStatusCode.Created);

        }

        [Test]
        public async Task Create_application_bad_request()
        {
            //Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CreateAuditApplicationCommand, Result<long>>>(), default(CancellationToken)))
                .Returns(Task.FromResult(Result.Ok(10L)));

            //Act
            var auditController = new AuditApplicationController(_loggerMock.Object, _mediatorMock.Object);
            //No X-RequestID
            var actionResult = await auditController.CreateAuditApplicationAsync(new CreateAuditApplicationCommand(), String.Empty);
            var res = actionResult as BadRequestObjectResult;

            //Assert
            Assert.AreEqual(res.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Get_applications_success()
        {
            //Arrange
            var fakeDynamicResult = Result.Ok(new PagedResponse<ApplicationDto>());

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAuditApplicationsQuery>(), default(CancellationToken)))
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var auditController = new AuditApplicationController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.GetAuditApplicationsAsync(new GetAuditApplicationsQuery()) as OkObjectResult;

            //Assert
            Assert.AreEqual((actionResult as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Test]
        public async Task Get_application_success()
        {
            //Arrange
            var fakeDynamicResult = Result.Ok(new ApplicationDto());

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAuditApplicationDetailsQuery>(), default(CancellationToken)))
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var auditController = new AuditApplicationController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.GetAuditApplicationAsync(10) as OkObjectResult;

            //Assert
            Assert.AreEqual((actionResult as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

    }
}
