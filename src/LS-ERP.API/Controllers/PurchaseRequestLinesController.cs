using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseRequestLinesController : ControllerBase
    {
        private readonly ILogger<PurchaseRequestLinesController> logger;
        private readonly IMediator mediator;

        public PurchaseRequestLinesController(ILogger<PurchaseRequestLinesController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("group-to-purchase-order-line")]
        public async Task<ActionResult<CommonResponseModel<object>>> GroupToPurchaseOrderLine(
            [FromBody] GroupPurchaseRequestToPurchaseOrderLineCommand command)
        {
            var commandResult = new GroupPurchaseRequestToPurchaseOrderLineResult();
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<object>();

            if (commandResult.IsSuccess)
            {
                response.SetResult(commandResult.IsSuccess, "Action successfully")
                    .SetData(new
                    {
                        commandResult.PurchaseOrderLines,
                        commandResult.PurchaseOrderGroupLines
                    });
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return Ok(response);
        }
    }
}
