using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using LS_ERP.XAF.Module.Service.Common;
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
    public class ReceiptsController : ControllerBase
    {
        private readonly ILogger<ReceiptsController> logger;
        private readonly IMediator mediator;
        private readonly IReceiptQueries receiptQueries;

        public ReceiptsController(ILogger<ReceiptsController> logger,
            IMediator mediator,
            IReceiptQueries receiptQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.receiptQueries = receiptQueries;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> CreateReceipt(
            [FromBody] CreateReceiptCommand command)
        {
            var commandResult = new CommonCommandResult<Receipt>();
            logger.LogInformation("{@time} - Sending create receipt command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create receipt command with request {@request}",
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

        [HttpPost]
        [Route("create-fabric")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> CreateReceiptFabric([FromBody] CreateReceiptFabricCommand command)
        {
            var commandResult = new CommonCommandResult<Receipt>();
            logger.LogInformation("{@time} - Sending create receipt fabric command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create receipt fabric command with request {@request}",
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


        [HttpGet]
        [Route("report/{fromDate}/{toDate}")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> ReportReceipt(
            DateTime fromDate, DateTime toDate, [FromQuery] string numbers, [FromQuery] string storageCode)
        {
            var response = new CommonResponseModel();
            var data = receiptQueries.GetReceiptSummary(numbers, storageCode, fromDate, toDate);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> DeleteReceipt(DeleteReceiptCommand command)
        {
            var response = new CommonResponseModel();
            logger.LogInformation("{@time} - Sending delete receipt command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete receipt command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            var commandResult = await mediator.Send(command);

            if (commandResult.Success)
            {
                response.SetResult("000", "Action successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return Ok(response);
        }
    }
}
