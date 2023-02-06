using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Dtos.SalesOrder;
using LS_ERP.BusinessLogic.Queries.SalesOrder;
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
    public class SalesOrdersController : ControllerBase
    {
        private readonly ILogger<SalesOrdersController> logger;
        private readonly IMediator mediator;
        private readonly ISalesOrderQueries salesOrderQueries;

        public SalesOrdersController(ILogger<SalesOrdersController> logger,
            IMediator mediator,
            ISalesOrderQueries salesOrderQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.salesOrderQueries = salesOrderQueries;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SalesOrderSummaryDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<SalesOrderSummaryDtos>> GetSalesOrders()
        {
            var data = salesOrderQueries.GetSalesOrderSummaries();
            return Ok(data);
        }

        [HttpGet]
        [Route("{ID}")]
        [ProducesResponseType(typeof(IEnumerable<SalesOrderSummaryDtos>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<SalesOrderSummaryDtos>> GetSalesOrder(string ID)
        {
            var data = salesOrderQueries.GetSalesOrderByID(ID);

            if (data != null)
                return Ok(data);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreateSalesOrder([FromBody] CreateSalesOrderCommand command)
        {
            var commandResult = new CommonCommandResult<SalesOrder>();
            logger.LogInformation("{@time} - Sending create sale order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create sale order command with request {@request}",
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
        [Route("update-quantity")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> UpdateQuantity([FromForm] UpdateQuantityCommand command)
        {
            var commandResult = new UpdateQuantityResult();
            logger.LogInformation("{@time} - Sending update sale order quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update sale order quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Update successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("update")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> Update([FromForm] UpdateSalesOrderCommand command)
        {
            var commandResult = new UpdateSalesOrderResult();
            logger.LogInformation("{@time} - Sending update sale order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update sale order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Update successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> Import([FromForm] ImportSaleOrderCommand command)
        {
            var commandResult = new ImportSaleOrderResult();
            logger.LogInformation("{@time} - Sending import sale order quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending import sale order quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Import successfully")
                    .SetData(new { ID = commandResult.Result?.ID });
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("import-offset")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> ImportOffset(
            [FromForm] ImportSalesOrderOffsetCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<SalesOrderOffsetDto>>();
            logger.LogInformation("{@time} - Sending import sale order offset command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending import sale order offset command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.Success)
            {
                response.SetResult("000", "Import successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("offset")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> Offset(
            [FromBody] SalesOrderOffsetCommand command)
        {
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending offset sales order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending offset sales order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.Success)
            {
                response.SetResult("000", "Action successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("compare-data")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CompareData([FromForm] CompareDataCommand command)
        {
            var commandResult = new CompareDataResult();
            logger.LogInformation("{@time} - Sending compare data quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending compare data quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Commpare successfully")
                    .SetData(commandResult.GroupCompare);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("save-compare")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> SaveCompare([FromBody] SaveCompareCommand command)
        {
            var commandResult = new CompareDataResult();
            logger.LogInformation("{@time} - Sending save compare data command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending save compare data command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Commpare successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpDelete]
        [Route("{ID}")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> DeleteSalesOrder(string ID)
        {
            var command = new DeleteSalesOrderCommand
            {
                ID = ID,
            };

            var commandResult = new CommonCommandResult<SalesOrder>();
            logger.LogInformation("{@time} - Sending delete salesOrder command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete salesOrder command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("001", "Delete successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("101", commandResult.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("purchase_orders")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<CommonResponseModel>> GetPurchaseForOrder(
            [FromQuery] string customerID, [FromQuery] string saleOrderID, [FromQuery] string style)
        {
            var response = new CommonResponseModel();
            var data = salesOrderQueries.GetPurchaseForOrder(customerID, style, saleOrderID);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }

        [HttpPost]
        [Route("update-ship-quantity")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> UpdateShipQuantity([FromForm] UpdateShipQuantityCommand command)
        {
            var commandResult = new UpdateShipQuantityResult();
            logger.LogInformation("{@time} - Sending update sale order ship quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update sale order ship quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Update successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("update-sample-quantity")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> UpdateSampleQuantity([FromForm] UpdateSampleQuantityCommand command)
        {
            var commandResult = new UpdateSampleQuantityResult();
            logger.LogInformation("{@time} - Sending update sale order simple quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update sale order simple quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Update successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("update-CAC-CHD-EHD")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> UpdateCAC_CHD_EHD([FromForm] UpdateCAC_CHD_EHDCommand command)
        {
            var commandResult = new UpdateCAC_CHD_EHDResult();
            logger.LogInformation("{@time} - Sending update sale order CAC/CHD/EHD command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update sale order CAC/CHD/EHD command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Update successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
