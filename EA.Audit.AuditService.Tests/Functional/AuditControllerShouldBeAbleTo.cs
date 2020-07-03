using EA.Audit.Common.Application.Features.Audits.Commands;
using EA.Audit.Common.Application.Features.Audits.Queries;
using EA.Audit.Common.Application.Features.Shared;
using EA.Audit.Common.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace EA.Audit.AuditService.Tests.Functional
{
    [TestFixture]
    public class AuditControllerShouldBeAbleTo
    {
        private readonly HttpClient _client;

        public AuditControllerShouldBeAbleTo()
        {
            var factory = new CustomWebApplicationFactory<TestStartup>().WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot("EA.Audit.AuditService");

                builder.ConfigureTestServices(services =>
                {
                    services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);
                    services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
                });
            });
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add(Constants.XRequest.XRequestIdHeaderName, "b0ed668d-7ef2-4a23-a333-94ad278f45d7");
        }

        [Test]
        public async Task GetAllForApplication()
        {
            //Act
            var response = await _client.GetAsync("/api/v1/audits?PageNumber=1&PageSize=1");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            
            var result = JsonConvert.DeserializeObject<PagedResponse<AuditDto>>(stringResponse);
            
            //Assert
            Assert.AreEqual(10, result.Total);
            Assert.AreEqual(1, result.Data.Count());
        }

        [Test]
        public async Task GetById()
        {
            //Act
            var response = await _client.GetAsync("/api/v1/audits/1");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuditDto>(stringResponse);

            //Assert
            Assert.AreEqual("Actor Info", result.Actor);
            Assert.AreEqual("Subject Info", result.Subject);
            Assert.AreEqual("Subject updated", result.Description);
        }

        [Test]
        public async Task GetById_NotFound()
        {
            //Act
            var response = await _client.GetAsync("/api/v1/audits/999");
            
            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Test]
        public async Task PostAudit()
        {
            //Arrange
            var publishAuditCommand = new PublishAuditCommand
            {
                SubjectId = 1,
                Subject = "Subject",
                ActorId = 1,
                Actor = "Actor",
                Description = "Descirption",
                Properties = "some json string"
            };

            //Act
            var response = await _client.PostAsJsonAsync("/api/v1/audits", publishAuditCommand);
            response.EnsureSuccessStatusCode();

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

    }
}
