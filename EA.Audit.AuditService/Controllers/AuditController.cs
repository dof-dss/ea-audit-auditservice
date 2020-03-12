using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Threading.Tasks;
using EA.Audit.AuditService.Infrastructure;
using System;
using EA.Audit.AuditService.Application.Extensions;
using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.AuditService.Application.Features.Audits.Queries;
using EA.Audit.AuditService.Application.Features.Audits.Commands;
using Microsoft.AspNetCore.Authorization;

namespace EA.Audit.AuditService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class AuditController : ControllerBase
    { 
        private readonly ILogger<AuditController> _logger;
        private readonly IMediator _mediator;

        public AuditController(ILogger<AuditController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet(ApiRoutes.Audits.GetAll)]
        [Authorize("audit-api/read_audits")]
        public async Task<ActionResult> GetAuditsAsync([FromQuery]GetAuditsQuery request)
        {
            var audits = await _mediator.Send(request).ConfigureAwait(false);
            return Ok(audits);
        }

        [HttpGet(ApiRoutes.Audits.Get)]
        [Authorize("audit-api/read_audits")]
        public async Task<ActionResult> GetAuditAsync(int id)
        { 
            var audit = await _mediator.Send(new GetAuditDetailsQuery() { Id = id }).ConfigureAwait(false);
            return Ok(audit);
        }


        /**********************************************************************
         * CONSIDER PULLING INTO SEPARATE API FOR SCALING INDEPENDENT OF READ
         * ********************************************************************/
        [HttpPost(ApiRoutes.Audits.Create)]
        [Authorize("audit-api/create_audit")]
        public async Task<IActionResult> CreateAuditAsync([FromBody]CreateAuditCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            long commandResult = -1;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestCreateAudit = new IdentifiedCommand<CreateAuditCommand, long>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - ({@Command})",
                    requestCreateAudit.GetGenericTypeName(),
                    requestCreateAudit);

                commandResult = await _mediator.Send(requestCreateAudit);
            }

            if (commandResult == -1)
            {
                return BadRequest();
            }

            return Ok(commandResult);
        }


        [HttpGet(ApiRoutes.Audits.Search)]
        [Authorize("audit-api/read_audits")]
        public async Task<ActionResult> SearchAuditsAsync([FromQuery]SearchAuditsQuery request)
        {
            var audits = await _mediator.Send(request).ConfigureAwait(false);
            return Ok(audits);
        }

    }     
}
