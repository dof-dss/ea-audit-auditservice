using EA.Audit.Common.Application.Commands;
using EA.Audit.Common.Application.Features.Shared;
using EA.Audit.Common.Application.Queries;
using EA.Audit.Common.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Tests.Functional
{
    [TestFixture]
    public class AuditApplicationControllerShouldBeAbleTo
    {
        private readonly HttpClient _client;

        public AuditApplicationControllerShouldBeAbleTo()
        {
            _client = new CustomWebApplicationFactory<Startup>().CreateClient();
            _client.DefaultRequestHeaders.Add(Constants.XRequest.XRequestIdHeaderName, "b0ed668d-7ef2-4a23-a333-94ad278f45d7");
        }

        [Test]
        public async Task GetAllForApplication()
        {
            //Act
            var response = await _client.GetAsync("/api/v1/applications?PageNumber=0&PageSize=1");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<PagedResponse<ApplicationDto>>(stringResponse);

            //Assert
            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.Data.Count());
        }

        [Test]
        public async Task GetById()
        {
            //Act
            var response = await _client.GetAsync("/api/v1/applications/123456");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApplicationDto>(stringResponse);

            //Assert
            Assert.AreEqual(123456, result.Id);
            Assert.AreEqual("TestAuditApplication", result.Name);
            Assert.AreEqual("Test Audit Application", result.Description);
        }

        [Test]
        public async Task GetById_NotFound()
        {
            //Act
            var response = await _client.GetAsync("/api/v1/applications/1");
            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Test]
        public async Task PostAudit()
        {
            //Arrange
            var createAuditApplicationCommand = new CreateAuditApplicationCommand
            {
                Name = "NewTestAuditApplication",
                Description = "New Audit Application",
                ClientId = "aClientId"
            };

            //Act
            var response = await _client.PostAsJsonAsync("/api/v1/applications", createAuditApplicationCommand);
            response.EnsureSuccessStatusCode();

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

    }
}
