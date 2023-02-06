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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseRequestsController : ControllerBase
    {
        private readonly ILogger<PurchaseRequestsController> logger;
        private readonly IMediator mediator;

        public PurchaseRequestsController(ILogger<PurchaseRequestsController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("save-request")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CommonResponseModel>> SavePurchaseRequest(
            [FromBody]SavePurchaseRequestCommand command)
        {
            var commandResult = new CommonCommandResult<PurchaseRequest>();
            logger.LogInformation("{@time} - Sending save purchase request command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending save purchase request command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            var response = new CommonResponseModel();

            commandResult = await mediator.Send(command);

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Create successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return Ok(response);
        }
    }
}
