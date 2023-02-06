using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingPlansController : ControllerBase
    {
        private readonly ILogger<SalesContractsController> logger;
        private readonly IMediator mediator;

        public ShippingPlansController(ILogger<SalesContractsController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("import")]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        [ProducesResponseType(typeof(CommonResponseModel<List<ShippingPlanDetail>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<ShippingPlanDetail>>>> Import([FromForm] ImportShippingPlanCommand command)
        {
            var commandResult = new CommonCommandResultHasData<List<ShippingPlanDetail>>();
            logger.LogInformation("{@time} - Sending import shipping plan command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending import shipping plan command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<List<ShippingPlanDetail>>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Import successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> CreateShippingPlan(
            [FromBody]  CreateShippingPlanCommand command)
        {
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending create shipping plan command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create shipping plan command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<object>();
            response.SetResult(commandResult.Success, commandResult.Message);
            return response;
        }

        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> UpdateShippingPlan(
            [FromBody] UpdateShippingPlanCommand command)
        {
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending update shipping plan command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update shipping plan command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<object>();
            response.SetResult(commandResult.Success, commandResult.Message);
            return response;
        }

        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<ShippingPlan>>>> Bulk(
            [FromBody] BulkShippingPlanCommand command)
        {
            var response = new CommonResponseModel<object>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message));
        }
    }
}
