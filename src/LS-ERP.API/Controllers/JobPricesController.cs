using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class JobPricesController : ControllerBase
    {
        private readonly ILogger<JobPrice> logger;
        private readonly IJobPriceQueries jobPriceQueries;
        private readonly IMediator mediator;

        public JobPricesController(ILogger<JobPrice> logger,
            IJobPriceQueries jobPriceQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.jobPriceQueries = jobPriceQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobPriceDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<JobPriceDtos>>>> Get(
         [FromQuery] string companyId)
        {
            var data = jobPriceQueries.Get(companyId);
            return new CommonResponseModel<IEnumerable<JobPriceDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpGet]
        [Route("{Id}")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobPriceDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<JobPriceDtos>>>> GetById(
         int Id)
        {
            var data = jobPriceQueries.GetById(Id);
            return new CommonResponseModel<IEnumerable<JobPriceDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel<JobPrice>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<JobPrice>>> CreatePartPrice(
            [FromBody] CreateJobPriceCommand command)
        {
            var commandResult = new CommonCommandResultHasData<JobPrice>();
            logger.LogInformation("{@time} - Sending create job price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<JobPrice>();

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
        [ProducesResponseType(typeof(CommonResponseModel<JobPrice>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<JobPrice>>> UpdatePartPrice(
           [FromBody] UpdateJobPriceCommand command)
        {
            var commandResult = new CommonCommandResultHasData<JobPrice>();
            logger.LogInformation("{@time} - Sending update job price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update job price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<JobPrice>();

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
        [Route("{Id:int}")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteJobPrice(
            int Id)
        {
            var command = new DeleteJobPriceCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending delete job price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete job price command with request {@request}",
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
