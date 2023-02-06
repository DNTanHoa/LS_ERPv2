using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using LS_ERP.XAF.Module.Dtos;
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
    public class CuttingOutputsController : ControllerBase
    {
        private readonly ILogger<CuttingOutputsController> logger;
        private readonly ICuttingOutputQueries CuttingOutputQueries;
        private readonly IMediator mediator;

        public CuttingOutputsController(ILogger<CuttingOutputsController> logger,
            ICuttingOutputQueries CuttingOutputQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.CuttingOutputQueries = CuttingOutputQueries;
            this.mediator = mediator;
        }
        [HttpGet]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingOutputDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingOutputDtos>>>> Get(
         [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate)
        {
            var data = CuttingOutputQueries.Get(CompanyID, FromDate, ToDate);
            return new CommonResponseModel<IEnumerable<CuttingOutputDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("daily_report")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingOutputDailyReportDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingOutputDailyReportDtos>>>> GetDailyReport(
         [FromQuery] string CompanyID, [FromQuery] DateTime ProduceDate)
        {
            var data = CuttingOutputQueries.GetDailyReport(CompanyID, ProduceDate);
            return new CommonResponseModel<IEnumerable<CuttingOutputDailyReportDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("month_report")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingOutputDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingOutputDtos>>>> GetMonthReport(
         [FromQuery] string CompanyID, [FromQuery] DateTime ProduceDate)
        {
            var data = CuttingOutputQueries.GetMonthReport(CompanyID, ProduceDate);
            return new CommonResponseModel<IEnumerable<CuttingOutputDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("card_merge_block_lsstyle")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingCardDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetCardMergeBlockLSStyle (
         [FromQuery] string CompanyID, [FromQuery] DateTime ProduceDate)
        {
            var data = CuttingOutputQueries.GetCardMergeBlockLSStyle(CompanyID, ProduceDate);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("work_center_report")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingOutputDailyReportDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingOutputDailyReportDtos>>>> GetWorkCenterReportByMonth(
         [FromQuery] string CompanyID, [FromQuery] DateTime ProduceDate)
        {
            var data = CuttingOutputQueries.GetWorkCenterReportByMonth(CompanyID, ProduceDate);
            return new CommonResponseModel<IEnumerable<CuttingOutputDailyReportDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("cutting_lot")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<PivotCuttingLotDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<PivotCuttingLotDtos>>>> GetPivotCutingLot(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string MergeBlockLSStyle, [FromQuery] string MergeLSStyle)
        {
            var data = CuttingOutputQueries.GetPivotCuttingLot(CompanyID, FromDate, ToDate, MergeBlockLSStyle,MergeLSStyle);
            return new CommonResponseModel<IEnumerable<PivotCuttingLotDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("cutting_lot_issue")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetIssueCutingLot(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string MergeBlockLSStyle, [FromQuery] int FabricContrastID)
        {
            var data = CuttingOutputQueries.GetIssueCuttingLot(CompanyID, FromDate, ToDate, MergeBlockLSStyle,FabricContrastID);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("size")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetCuttingSize(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string MergeBlockLSStyle
            , [FromQuery] string LSStyle, [FromQuery] string MergeLSStyle)
        {
            var data = CuttingOutputQueries.GetCuttingSize(CompanyID, FromDate, ToDate, MergeBlockLSStyle,MergeLSStyle,LSStyle);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("set")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetCuttingSet(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string MergeBlockLSStyle
            , [FromQuery] string LSStyle)
        {
            var data = CuttingOutputQueries.GetCuttingSet(CompanyID, FromDate, ToDate, MergeBlockLSStyle, LSStyle);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("fabric_contrast")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<FabricContrastDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<FabricContrastDtos>>>> GetCuttingFabricContrast(
        [FromQuery] string MergeBlockLSStyle)
        {
            var data = CuttingOutputQueries.GetCuttingFabricContrast(MergeBlockLSStyle);
            return new CommonResponseModel<IEnumerable<FabricContrastDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("cutting_status")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingOutputStatusDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingOutputStatusDtos>>>> GetCutingStatus(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string CustomerName, [FromQuery] string LSStyle)
        {
            var data = CuttingOutputQueries.GetCuttingOutputStatus(CompanyID, FromDate, ToDate,CustomerName,LSStyle);
            return new CommonResponseModel<IEnumerable<CuttingOutputStatusDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingOutputDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingOutputDtos>>>> GetByID(
         [FromQuery] int ID)
        {
            var data = CuttingOutputQueries.Get(ID);
            return new CommonResponseModel<IEnumerable<CuttingOutputDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("alloc_detail_with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingOutputAllocDetailDtos>>),
       (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingOutputAllocDetailDtos>>>> GetAllocDetailByCuttingOutputID(
        [FromQuery] int ID)
        {
            var data = CuttingOutputQueries.GetAllocDetail(ID);
            return new CommonResponseModel<IEnumerable<CuttingOutputAllocDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("check_change_order_quantity")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<AllocDailyOutputChangeDtos>>),
      (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<AllocDailyOutputChangeDtos>>>> CheckChangeOrderQuanity(
       [FromQuery] string CompanyID)
        {
            var data = CuttingOutputQueries.CheckChangeOrderQuantity(CompanyID);
            return new CommonResponseModel<IEnumerable<AllocDailyOutputChangeDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPost]
        [Route("update_change_order_quantity")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<AllocDailyOutputChangeDtos>>),
      (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<AllocDailyOutputChangeDtos>>>> UpdateChangeOrderQuanity(
       [FromBody] List<string> listId)
        {
            var data = CuttingOutputQueries.UpdateChangeOrderQuantity(listId);
            return new CommonResponseModel<IEnumerable<AllocDailyOutputChangeDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<CuttingOutput>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<CuttingOutput>>> UpdateCuttingOutput(
            [FromBody] UpdateCuttingOutputCommand command)
        {
            var commandResult = new CommonCommandResultHasData<CuttingOutput>();
            logger.LogInformation("{@time} - Sending update cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<CuttingOutput>();

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
        [ProducesResponseType(typeof(CommonResponseModel<CuttingOutput>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<CuttingOutput>>> CreateCuttingOutput(
            [FromBody] CreateCuttingOutputCommand command)
        {
            var commandResult = new CommonCommandResultHasData<CuttingOutput>();
            logger.LogInformation("{@time} - Sending create cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<CuttingOutput>();

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
        [Route("{Id:int}")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteCuttingOutput(
            int Id)
        {
            var command = new DeleteCuttingOutputCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending delete cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete cutting output command with request {@request}",
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

        [HttpGet]
        [Route("operation_report")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingOutputReportDtos>>),
          (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingOutputReportDtos>>>> GetCuttingOutputReport(
          [FromQuery] string customerID, [FromQuery] string lsStyle, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate)
        {
            var data = CuttingOutputQueries.GetCuttingOutputReport(customerID, lsStyle, FromDate, ToDate);
            return new CommonResponseModel<IEnumerable<CuttingOutputReportDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
    }
}
