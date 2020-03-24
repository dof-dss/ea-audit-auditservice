using EA.Audit.Common.Application.Features.Audits.Commands;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Model.Admin;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Tests.Integration
{

    [TestFixture]
    class AddAuditCommandShouldBeAbleTo : BaseIntegration
    {

        [Test]
        public async Task HandleAddAuditWithNoApplicationFoundForClientId()
        {
            // Arrange
            var command = new CreateAuditCommand {
                Actor = "Actor", ActorId = 1, 
                ClientId = "1", Description  = "Description", 
                Subject="Subject", SubjectId = 1,
                Properties = "properties"
            };

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoApplicationFound, result.Error);
        }

        //[Test]
        //public async Task HandleAddAuditWithSuccess()
        //{
        //    // Arrange
        //    var app = new AuditApplication
        //    {
        //        Id = 123456,
        //        ClientId = "123456",
        //        DateCreated = DateTime.Now,
        //        DateModified = DateTime.Now,
        //        Description = "App Desc",
        //        Name = "Name"
        //    };
        //    DbContext.AuditApplications.Add(app);
        //    DbContext.SaveChanges();

        //    var command = new CreateAuditCommand
        //    {
        //        Actor = "Actor",
        //        ActorId = 1,
        //        ClientId = "123456",
        //        Description = "Description",
        //        Subject = "Subject",
        //        SubjectId = 1,
        //        Properties = "properties"
        //    };

        //    // Act
        //    var result = await Mediator.Send(command);

        //    //Assert
        //    Assert.True(result.IsFailure);
        //    Assert.AreEqual(Constants.ErrorMessages.NoApplicationFound, result.Error);
        //}
    }
}
