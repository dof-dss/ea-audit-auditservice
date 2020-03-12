using EA.Audit.AuditService.Application.Commands;
using EA.Audit.AuditService.Application.Extensions;
using EA.Audit.AuditService.Application.Features.Application.Queries;
using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EA.Audit.AuditService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class AuditApplicationController : ControllerBase
    {
        private readonly ILogger<AuditApplicationController> _logger;
        private readonly IMediator _mediator;

        public AuditApplicationController(ILogger<AuditApplicationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet(ApiRoutes.AuditApplications.GetAll)]
        [Authorize("audit-api/audit_admin")]
        public async Task<ActionResult> GetAuditApplicationsAsync([FromQuery]GetAuditApplicationsQuery request)
        {
            var audits = await _mediator.Send(request).ConfigureAwait(false);
            return Ok(audits);
        }

        [HttpGet(ApiRoutes.AuditApplications.Get)]
        [Authorize("audit-api/audit_admin")]
        public async Task<ActionResult> GetAuditApplicationAsync(int id)
        {
            var audit = await _mediator.Send(new GetAuditApplicationDetailsQuery() { Id = id }).ConfigureAwait(false);
            return Ok(audit);
        }

        [HttpPost(ApiRoutes.AuditApplications.Create)]
        [Authorize("audit-api/audit_admin")]
        public async Task<IActionResult> CreateAuditApplicationAsync([FromBody]CreateAuditApplicationCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            long commandResult = -1;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestCreateAuditApplication = new IdentifiedCommand<CreateAuditApplicationCommand, long>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - ({@Command})",
                    requestCreateAuditApplication.GetGenericTypeName(),
                    requestCreateAuditApplication);

                commandResult = await _mediator.Send(requestCreateAuditApplication).ConfigureAwait(false);
            }

            if (commandResult == -1)
            {
                return BadRequest();
            }

            return Ok(commandResult);
        }

    }
}
