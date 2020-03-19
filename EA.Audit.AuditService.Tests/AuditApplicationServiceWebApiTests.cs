using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Controllers;
using EA.Audit.Infrastructure.Application.Commands;
using EA.Audit.Infrastructure.Application.Queries;
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
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CreateAuditApplicationCommand, long>>(), default(CancellationToken)))
                .Returns(Task.FromResult(10L));

            //Act
            var auditController = new AuditApplicationController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.CreateAuditApplicationAsync(new CreateAuditApplicationCommand(), Guid.NewGuid().ToString()) as OkObjectResult;

            //Assert
            Assert.AreEqual(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);

        }

        [Test]
        public async Task Create_application_bad_request()
        {
            //Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CreateAuditApplicationCommand, long>>(), default(CancellationToken)))
                .Returns(Task.FromResult(10L));

            //Act
            var auditController = new AuditApplicationController(_loggerMock.Object, _mediatorMock.Object);
            var actionResult = await auditController.CreateAuditApplicationAsync(new CreateAuditApplicationCommand(), String.Empty) as BadRequestResult;

            //Assert
            Assert.AreEqual(actionResult.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Get_applications_success()
        {
            //Arrange
            var fakeDynamicResult = new PagedResponse<ApplicationDto>();

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
            var fakeDynamicResult = new ApplicationDto();

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
