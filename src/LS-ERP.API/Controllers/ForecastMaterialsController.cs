using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastMaterialsController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<ForecastMaterialsController> logger;

        public ForecastMaterialsController(IMediator mediator,
            ILogger<ForecastMaterialsController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpPost]
        [Route("group-to-purchase-order-line")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> GroupToPurchaseOrderLine(
            [FromBody]GroupForecastMaterialToPurchaseOrderLineCommand command)
        {
            var commandResult = new GroupForecastMaterialToPurchaseOrderLineResult();
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Action successfully")
                    .SetData(new { commandResult.PurchaseOrderLines, commandResult.PurchaseOrderGroupLines });
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return Ok(response);
        }
    }
}
