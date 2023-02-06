using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastOverallsController : ControllerBase
    {
        private readonly ILogger<ForecastOverallsController> logger;
        private readonly IMediator mediator;

        public ForecastOverallsController(ILogger<ForecastOverallsController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("pull-bom")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        public async Task<ActionResult<CommonResponseModel>> PullBom(
            [FromBody] PullBOMForecastOverallCommand command)
        {
            var commandResult = new PullBOMForecastOverallResult();

            logger.LogInformation("{@time} - Pull bom for forecast command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Pull bom for forecast command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("caculate-required-quantity")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        public async Task<ActionResult<CommonResponseModel>> CalculateRequiredQuantity(
            [FromBody] CalculateRequiredQuantityForecastOverallCommand command)
        {
            var commandResult = new CalculateRequiredQuantityForecastOverallResult();

            logger.LogInformation("{@time} - Calculate required quantity for forecast command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Calculate required quantity for forecast command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("balance-quantity")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        public async Task<ActionResult<CommonResponseModel>> BalanceQuantity(
            [FromBody] CalculateRequiredQuantityForecastOverallCommand command)
        {
            var commandResult = new CalculateRequiredQuantityForecastOverallResult();

            logger.LogInformation("{@time} - Balance quantity for forecast command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Balance quantity for forecast command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
