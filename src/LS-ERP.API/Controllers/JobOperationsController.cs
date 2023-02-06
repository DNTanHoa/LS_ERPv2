using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class JobOperationsController : ControllerBase
    {
        private readonly ILogger<JobOperationsController> logger;
        private readonly IMediator mediator;
        private readonly IJobOperationQueries jobOperationQueries;

        public JobOperationsController(ILogger<JobOperationsController> logger,
            IMediator mediator,
            IJobOperationQueries jobOperationQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.jobOperationQueries = jobOperationQueries;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel<JobOperation>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<JobOperation>>> CreateJobOperation(
            [FromBody] CreateJobOperationCommand command)
        {
            var commandResult = new CommonCommandResultHasData<JobOperation>();
            logger.LogInformation("{@time} - Sending create job operation command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job operation command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<JobOperation>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Create successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }

        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<JobOperation>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<JobOperation>>> UpdateJobOperation(
            [FromBody] UpdateJobOperationCommand command)
        {
            var commandResult = new CommonCommandResultHasData<JobOperation>();
            logger.LogInformation("{@time} - Sending update job operation command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update job operation command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<JobOperation>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Update successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobOperation>>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<JobOperation>>>> BulkJobOperation(
            [FromBody] BulkJobOperationCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<JobOperation>>();
            logger.LogInformation("{@time} - Sending bulk job operation command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending bulk job operation command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<IEnumerable<JobOperation>>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Update successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobOperation>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<JobOperation>>>> 
            GetJobOperations([FromQuery]string customerID, [FromQuery]string workCenterID,
            [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var response = new CommonResponseModel<IEnumerable<JobOperation>>();
            var data = jobOperationQueries.GetJobOperations(customerID, workCenterID,
                fromDate, toDate);
            return Ok(response.SetData(data).SetResult(true, string.Empty));
        }

        [HttpGet]
        [Route("{ID}")]
        [ProducesResponseType(typeof(CommonResponseModel<JobOperation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<CommonResponseModel<JobOperation>>>
            GetJobOpperation(string ID)
        {
            var response = new CommonResponseModel<JobOperation>();
            var data = await jobOperationQueries.GetJobOperation(ID);
            return Ok(response.SetData(data).SetResult(true, string.Empty));
        }
    }
}
