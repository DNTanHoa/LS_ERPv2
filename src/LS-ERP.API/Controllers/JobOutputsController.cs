using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class JobOutputsController : ControllerBase
    {
        private readonly ILogger<JobOutputsController> logger;
        private readonly IMediator mediator;
        private readonly IJobOutputQueries jobOutputQueries;

        public JobOutputsController(
            ILogger<JobOutputsController> logger,
            IMediator mediator,
            IJobOutputQueries jobOutputQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.jobOutputQueries = jobOutputQueries;
        }
     
        [HttpGet]
        [ProducesResponseType(typeof(CommonResponseModel<JobOutput>),
           (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> Get(
           [FromQuery] int id)
        {
            var response = new CommonResponseModel();
            var data = jobOutputQueries.GetJobOutputs(id).FirstOrDefault();
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        [HttpGet]
        [Route("with_work_centerid")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobOutput>>), 
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<JobOutput>>>> GetJobOutPut(
            [FromQuery]string departmentID, [FromQuery] DateTime AtTime)
        {
            var data = jobOutputQueries.GetJobOutputsByDepartment(departmentID,AtTime);
            return new CommonResponseModel<IEnumerable<JobOutput>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobOutput>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<JobOutput>>>> GetJobOutPutByDate(
            [FromQuery] DateTime AtTime)
        {
            var data = jobOutputQueries.GetJobOutputsByDate(AtTime);
            return new CommonResponseModel<IEnumerable<JobOutput>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("summary_with_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobOutput>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<JobOutputSummaryDtos>>>> GetJobOutPutSummaryByDate(
            [FromQuery] DateTime AtTime)
        {
            var data = jobOutputQueries.GetJobOutputsSummaryByDate(AtTime);
            return new CommonResponseModel<IEnumerable<JobOutputSummaryDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel<JobOutput>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<JobOutput>>> CreateJobOutput(
            [FromBody] CreateJobOutputCommand command)
        {
            var commandResult = new CommonCommandResultHasData<JobOutput>();
            logger.LogInformation("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<JobOutput>();

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

        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobOutput>>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<JobOutput>>>> Bulk(
            [FromBody] BulkJobOutputCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<JobOutput>>();
            logger.LogInformation("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<IEnumerable<JobOutput>>();

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
        [ProducesResponseType(typeof(CommonResponseModel<JobOutput>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<JobOutput>>> UpdateJobOutput(
            [FromBody] UpdateJobOutPutCommand command)
        {
            var commandResult = new CommonCommandResultHasData<JobOutput>();
            logger.LogInformation("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<JobOutput>();

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

        [HttpDelete]
        [Route("{Id:long}")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteJobOutput(
            long Id)
        {
            var command = new DeleteJobOutPutCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<object>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Delete successfully");
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }
    }
}
