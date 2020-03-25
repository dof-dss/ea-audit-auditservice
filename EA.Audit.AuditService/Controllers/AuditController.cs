using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Threading.Tasks;
using System;
using EA.Audit.AuditService.Application.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using EA.Audit.Common.Infrastructure.Extensions;
using EA.Audit.Common.Application.Features.Audits.Queries;
using EA.Audit.Common.Application.Features.Audits.Commands;
using Microsoft.AspNetCore.Http;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Infrastructure.Functional;

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
        [Authorize(Constants.Auth.ReadAudits)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditsAsync([FromQuery]GetAuditsQuery request)
        {
            var result = await _mediator.Send(request).ConfigureAwait(false);
            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));
        }

        [HttpGet(ApiRoutes.Audits.Get)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Constants.Auth.ReadAudits)]
        public async Task<IActionResult> GetAuditAsync(int id)
        { 
            var result = await _mediator.Send(new GetAuditDetailsQuery() { Id = id }).ConfigureAwait(false);
            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));
        }


        /**********************************************************************
         * CONSIDER PULLING INTO SEPARATE API FOR SCALING INDEPENDENT OF READ
         * ********************************************************************/
        [HttpPost(ApiRoutes.Audits.Create)]
        [Authorize(Constants.Auth.CreateAudits)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAuditAsync([FromBody]PublishAuditCommand command, [FromHeader(Name = Constants.XRequest.XRequestIdHeaderName)] string requestId)
        {

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestCreateAudit = new IdentifiedCommand<PublishAuditCommand, Result<string>>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - ({@Command})",
                    requestCreateAudit.GetGenericTypeName(),
                    requestCreateAudit);

                var commandResult = await _mediator.Send(requestCreateAudit).ConfigureAwait(false);

                if (commandResult.IsFailure)
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest(Constants.ErrorMessages.XRequestIdIsMissing);
            }

            return StatusCode(201);
        }       


        [HttpGet(ApiRoutes.Audits.Search)]
        [Authorize(Constants.Auth.ReadAudits)]
        public async Task<ActionResult> SearchAuditsAsync([FromQuery]SearchAuditsQuery request)
        {
            var audits = await _mediator.Send(request).ConfigureAwait(false);
            return Ok(audits);
        }

    }     
}
