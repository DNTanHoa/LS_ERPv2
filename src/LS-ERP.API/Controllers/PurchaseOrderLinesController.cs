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
    public class PurchaseOrderLinesController : ControllerBase
    {
        private readonly ILogger<PurchaseOrderLinesController> logger;
        private readonly IMediator mediator;

        public PurchaseOrderLinesController(ILogger<PurchaseOrderLinesController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("map-upc")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CommonResponseModel>> MapUPC([FromBody] MapUPCCommand command)
        {
            logger.LogInformation("{@time} - Send map upc command with {@request}", 
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Send map upc command with {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            
            var commandResult = new MapUPCResult();
            var respone = new CommonResponseModel();

            commandResult = await mediator.Send(command);

            if(commandResult.IsSuccess)
            {
                respone.SetResult("000", "Map UPC successfully")
                    .SetData(commandResult.PurchaseOrderLines);
            }
            else
            {
                respone.SetResult("100", commandResult.Message);
            }

            return Ok(respone);
        }

        [HttpPost]
        [Route("map-price")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CommonResponseModel>> MapPrice([FromBody] MapPriceCommand command)
        {
            logger.LogInformation("{@time} - Send map price command with {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Send map price command with {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            var commandResult = new MapPriceResult();
            var respone = new CommonResponseModel();

            commandResult = await mediator.Send(command);

            if (commandResult.IsSuccess)
            {
                respone.SetResult("000", "Map price successfully")
                    .SetData(commandResult.PurchaseOrderLines);
            }
            else
            {
                respone.SetResult("100", commandResult.Message);
            }

            return Ok(respone);
        }

        [HttpPost]
        [Route("bulk-delete")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CommonResponseModel>> Delete([FromBody] DeletePurchaseOrderLineCommand command)
        {
            logger.LogInformation("{@time} - Send delete purchase order line command {@request}",
                DateTime.Now.ToString(), 
                JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Send delete purchase order line command {@request}",
                DateTime.Now.ToString(),
                JsonConvert.SerializeObject(command));

            var commandResult = new DeletePurchaseOrderLineResult();
            var respone = new CommonResponseModel();

            commandResult = await mediator.Send(command);

            if (commandResult.IsSuccess)
            {
                respone.SetResult("000", "Delete purchase order line successfully");
            }
            else
            {
                respone.SetResult("100", commandResult.ErrorMessage);
            }

            return Ok(respone);
        }
    }
}
