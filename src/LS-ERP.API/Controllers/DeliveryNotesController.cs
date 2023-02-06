using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
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
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryNotesController : ControllerBase
    {
        private readonly ILogger<DeliveryNotesController> logger;
        private readonly IDeliveryNoteQueries DeliveryNoteQueries;
        private readonly IMediator mediator;

        public DeliveryNotesController(ILogger<DeliveryNotesController> logger,
            IDeliveryNoteQueries DeliveryNoteQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.DeliveryNoteQueries = DeliveryNoteQueries;
            this.mediator = mediator;
        }
        [HttpGet]        
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DeliveryNoteDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DeliveryNoteDtos>>>> GetAll([FromQuery] string CompanyID
            , [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate)
        
        {
            var data = DeliveryNoteQueries.GetByCompany(CompanyID,FromDate,ToDate);
            return new CommonResponseModel<IEnumerable<DeliveryNoteDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("for_scan_qrcode")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DeliveryNoteDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DeliveryNoteDtos>>>> GetForCsanQR([FromQuery] string CompanyID, [FromQuery] string Type
        )
        {
            var data = DeliveryNoteQueries.GetForScanQrcode(CompanyID,Type);
            return new CommonResponseModel<IEnumerable<DeliveryNoteDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DeliveryNoteDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DeliveryNoteDtos>>>> GetByID(
         [FromQuery] string Id)
        {
            var data = DeliveryNoteQueries.GetByID(Id);
            return new CommonResponseModel<IEnumerable<DeliveryNoteDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("status_report")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DeliveryNoteDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DeliveryNoteDetailDtos>>>> GetReport(
         [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string KeyWord)
        {
            var data = DeliveryNoteQueries.GetReport(CompanyID,FromDate,ToDate,KeyWord);
            return new CommonResponseModel<IEnumerable<DeliveryNoteDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("receive_status_report")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<LSStyleDetailReportDtos>>),
       (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<LSStyleDetailReportDtos>>>> GetReceiveReport(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string KeyWord)
        {
            var data = DeliveryNoteQueries.GetReceiveReport(CompanyID, FromDate, ToDate, KeyWord);
            return new CommonResponseModel<IEnumerable<LSStyleDetailReportDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<DeliveryNote>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<DeliveryNote>>> UpdateDeliveryNote(
            [FromBody] UpdateDeliveryNoteCommand command)
        {
            var commandResult = new CommonCommandResultHasData<DeliveryNote>();
            logger.LogInformation("{@time} - Sending update delivery note command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update delivery note command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<DeliveryNote>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Update successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }
        [HttpPost]
        [Route("delete_delivery_note_detail")]
        [ProducesResponseType(typeof(CommonResponseModel<DeliveryNoteDetail>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<DeliveryNoteDetail>>> UpdateCuttingCard(
            [FromBody] UpdateDeliveryNoteDetailCommand command)
        {
            var commandResult = new CommonCommandResultHasData<DeliveryNoteDetail>();
            logger.LogInformation("{@time} - Sending delete delivery note detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete delivery note detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<DeliveryNoteDetail>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Update successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }
        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel<DeliveryNote>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<DeliveryNote>>> CreateDeliveryNote(
            [FromBody] CreateDeliveryNoteCommand command)
        {
            var commandResult = new CommonCommandResultHasData<DeliveryNote>();
            logger.LogInformation("{@time} - Sending create delivery note command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create delivery note command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<DeliveryNote>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Create successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }
        [HttpDelete]
        //[Route("{Id:int}")]
        //[Route("id")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteDeliveryNote(
           string Id)
        {
            var command = new DeleteDeliveryNoteCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending delete delivery note command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete delivery note command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<object>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Delete successfully");
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }
    }
}
