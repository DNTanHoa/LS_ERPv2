using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Queries;
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
    public class ProductionBOMsController : ControllerBase
    {
        private readonly ILogger<ProductionBOMsController> logger;
        private readonly IMediator mediator;
        private readonly IProductionBOMQueries productionBOMQueries;

        public ProductionBOMsController(ILogger<ProductionBOMsController> logger,
            IMediator mediator, IProductionBOMQueries productionBOMQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.productionBOMQueries = productionBOMQueries;
        }

        [HttpPost]
        [Route("group-to-purchase-order-line")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> GroupToPurchaseOrderLine(
            [FromBody]GroupToPurchaseOrderLineCommand command)
        {
            var commandResult = new GroupToPurchaseOrderLineResult();
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

        [HttpPost]
        [Route("delete-range")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> Delete([FromBody] 
        DeleteProductionBOMCommand command)
        {
            var commandResult = new DeleteProductionBOMResult();
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Action successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("group-to-purchase-request-line")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        public async Task<ActionResult<CommonResponseModel>> GroupToPurchaseRequestLine(
            [FromBody]GroupToPurchaseRequestLineCommand command)
        {
            var commandResult = new GroupToPurchaseRequestLineResult();
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Action successfully")
                    .SetData(new { commandResult.PurchaseRequestLines, 
                        commandResult.PurchaseRequestGroupLines });
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("group-to-issued-line")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        public async Task<ActionResult<CommonResponseModel>> GroupToIssuedLine(
            [FromBody]GroupToIssuedLineCommand command)
        {
            var commandResult = new GroupToIssuedLineResult();
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if(commandResult.IsSuccess)
            {
                response.SetResult("000", "Action successfully")
                   .SetData(new
                   {
                       commandResult.IssuedLines,
                       commandResult.IssuedGroupLines
                   });
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return Ok(response);
        }
        [HttpGet]
        [Route("fabric-items")]
        public async Task<ActionResult<CommonResponseModel>> GetFabricItems(
            [FromQuery] string MergeBlockLSStyle)
        {
            var response = new CommonResponseModel();
            var data = productionBOMQueries.GetFabricItems(MergeBlockLSStyle);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
    }

}
