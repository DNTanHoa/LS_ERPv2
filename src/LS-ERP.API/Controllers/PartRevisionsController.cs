using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartRevisionsController : ControllerBase
    {
        private readonly ILogger<PartRevisionsController> logger;
        private readonly IPartRevisionQueries partRevisionQueries;
        private readonly IMediator mediator;

        public PartRevisionsController(ILogger<PartRevisionsController> logger,
            IPartRevisionQueries partRevisionQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.partRevisionQueries = partRevisionQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PartRevisionSummaryDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<PartRevisionSummaryDtos>> GetPartRevisions()
        {
            return Ok(partRevisionQueries.GetSummaryDtos());
        }

        [HttpGet]
        [Route("{ID:int}")]
        [ProducesResponseType(typeof(PartRevisionSummaryDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<PartRevisionSummaryDtos>> GetPartRevision(int ID)
        {
            var PartRevision = partRevisionQueries.GetDetailDtos(ID);

            if (PartRevision != null)
                return Ok(PartRevision);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreatePartRevision(
            [FromBody] CreatePartRevisionCommand command)
        {
            var commandResult = new CommonCommandResult<PartRevision>();
            logger.LogInformation("{@time} - Sending create partRevision command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create partRevision command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Create successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> UpdatePartRevision(
            [FromBody] UpdatePartRevisionCommand command)
        {
            var commandResult = new CommonCommandResult<PartRevision>();
            logger.LogInformation("{@time} - Sending update PartRevision command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create partRevision command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("001", "Update successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpDelete]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> DeletePartRevision(
            [FromBody] DeletePartRevisionCommand command)
        {
            var commandResult = new CommonCommandResult<PartRevision>();
            logger.LogInformation("{@time} - Sending delete PartRevision command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("002", "Delte successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        
        public async Task<ActionResult<CommonResponseModel>> ImportPartRevision(
            [FromForm] ImportPartRevisionCommand command)
        {
            var commandResult = new ImportPartRevisionResult();
            logger.LogInformation("{@time} - Sending update PartRevision command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("001", "Import successfully")
                    .SetData(new { commandResult.PartRevision, commandResult.Items });
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("up-version")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> UpVersion(
            [FromBody] UpVersionCommand command)
        {
            var commandResult = new UpVersionResult();
            logger.LogInformation("{@time} - Sending up version PartRevision command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("001", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
