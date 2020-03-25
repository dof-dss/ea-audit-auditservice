using EA.Audit.AuditService.Application.Features.Shared;
using EA.Audit.Common.Infrastructure;
using EA.Audit.Common.Application.Commands;
using EA.Audit.Common.Infrastructure.Extensions;
using EA.Audit.Common.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using EA.Audit.Common.Infrastructure.Functional;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Constants.Auth.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditApplicationsAsync([FromQuery]GetAuditApplicationsQuery request)
        {
            var result = await _mediator.Send(request).ConfigureAwait(false);
            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));
        }

        [HttpGet(ApiRoutes.AuditApplications.Get)]
        [Authorize(Constants.Auth.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditApplicationAsync(int id)
        {
            var result = await _mediator.Send(new GetAuditApplicationDetailsQuery() { Id = id }).ConfigureAwait(false);
            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));
        }

        [HttpPost(ApiRoutes.AuditApplications.Create)]
        [Authorize(Constants.Auth.Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAuditApplicationAsync([FromBody]CreateAuditApplicationCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestCreateAuditApplication = new IdentifiedCommand<CreateAuditApplicationCommand, Result<long>>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - ({@Command})",
                    requestCreateAuditApplication.GetGenericTypeName(),
                    requestCreateAuditApplication);

                var commandResult = await _mediator.Send(requestCreateAuditApplication).ConfigureAwait(false);

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

    }
}
