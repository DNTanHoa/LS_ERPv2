using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
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
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly ILogger<PurchaseOrdersController> logger;
        private readonly IPurchaseOrderQueries purchaseOrderQueries;
        private readonly IMediator mediator;

        public PurchaseOrdersController(ILogger<PurchaseOrdersController> logger,
            IPurchaseOrderQueries purchaseOrderQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.purchaseOrderQueries = purchaseOrderQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PurchaseOrderSummaryDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<PurchaseOrderSummaryDtos>> GetPurchase()
        {
            var data = purchaseOrderQueries.GetPurchaseOrders();
            return Ok(data);
        }

        [HttpGet]
        [Route("{Number}")]
        [ProducesResponseType(typeof(PurchaseOrderDetailDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<PurchaseOrderDetailDtos> GetPurchase(string Number)
        {
            var data = purchaseOrderQueries.GetPurchaseOrder(Number);
            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("receipt-info/{Number}")]
        [ProducesResponseType(typeof(List<PurchaseOrderInforDtos>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<List<PurchaseOrderInforDtos>> GetPurchaseForReceipt(string Number)
        {
            var data = purchaseOrderQueries.GetPurchaseOrderInfors(Number);
            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }
        [HttpGet]
        [Route("with_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<PurchaseOrderReportDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<PurchaseOrderReportDtos>>>> GetPurchaseForReport(
            string CustomerID, string VendorID
            , DateTime FromDate, DateTime ToDate)
        {
            var data = purchaseOrderQueries.GetPurchaseOrderReport(CustomerID, VendorID, FromDate, ToDate);
            return new CommonResponseModel<IEnumerable<PurchaseOrderReportDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }


        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreatePurchaseOrder([FromBody]
            CreatePurchaseOrderCommand command)
        {
            var commandResult = new CommonCommandResult<PurchaseOrder>();
            logger.LogInformation("{@time} - Sending create purchase order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create purchase order command with request {@request}",
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

        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CommonResponseModel>> UpdatePurchaseOrder([FromBody]
            UpdatePurchaseOrderCommand command)
        {
            var commandResult = new CommonCommandResult<PurchaseOrder>();
            logger.LogInformation("{@time} - Sending update purchase order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update purchase order command with request {@request}",
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

        [HttpDelete]
        [Route("{PurchaseOrderID}/{Username}")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CommonResponseModel>> DeletePurchaseOrder(
            string PurchaseOrderID, string Username)
        {
            var command = new DeletePurchaseOrderCommand
            {
                PurchaseOrderID = PurchaseOrderID,
                Username = Username,
            };
            var commandResult = new DeletePurchaseOrderResult();
            logger.LogInformation(
                "{@time} - Sending delete purchase order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information(
                "{@time} - Sending delete purchase order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            var response = new CommonResponseModel();

            commandResult = await mediator.Send(command);

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Delete successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return Ok(response);

        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [RequestSizeLimit(int.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult<CommonResponseModel>> Import([FromForm] ImportPurchaseOrderCommand command)
        {
            var commandResult = new ImportPurchaseOrderResult();
            logger.LogInformation("{@time} - Sending import purchase order quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending import purchase order quantity command with request {@request}",
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
        [Route("matching-shipment")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CommonResponseModel>> MatchingShipment([FromBody] MatchingShipmentPurchaseOrderCommand command)
        {
            var commandResult = new MatchingShipmentPurchaseOrderResult();
            logger.LogInformation("{@time} - Sending matching shipment purchase order quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending  matching shipment purchase order quantity command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Matching shipment successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("received")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> PurchaseReceived(
            [FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] string storageCode, [FromQuery] string customerID)
        {
            logger.LogInformation("{@time} - Sending purchase received command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(fromDate));
            LogHelper.Instance.Information("{@time} - Sending  purchase received command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(fromDate));

            var response = new CommonResponseModel();
            //DateTime fromDateTo = fromDate.Date;

            var data = purchaseOrderQueries.GetPurchaseOrderReceived(customerID, storageCode,
                fromDate != null ? DateTime.Parse(fromDate) : null,
                toDate != null ? DateTime.Parse(toDate) : null);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
    }
}
