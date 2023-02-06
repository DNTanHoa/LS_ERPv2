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
using Ultils.Extensions;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyTargetDetailsController : ControllerBase
    {
        private readonly ILogger<DailyTargetDetailsController> logger;
        private readonly IMediator mediator;
        private readonly IDailyTargetDetailQueries DailyTargetDetailQueries;

        public DailyTargetDetailsController(
            ILogger<DailyTargetDetailsController> logger,
            IMediator mediator,
            IDailyTargetDetailQueries DailyTargetDetailQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.DailyTargetDetailQueries = DailyTargetDetailQueries;
        }
        [HttpGet]        
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetDailyTargetDetailById(
        [FromQuery] int Id)
        {
            var data = DailyTargetDetailQueries.GetById(Id);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);            
        }
        [HttpGet]
        [Route("with_operation")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetDailyTargetDetailByOperation(
        [FromQuery] string companyId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string operation)
        {
            var data = DailyTargetDetailQueries.GetByOperation(companyId, fromDate,toDate,operation);
             return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_operation_date_company")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetDailyTargetDetailByOperation(
        [FromQuery] string companyId, [FromQuery] DateTime produceDate, [FromQuery] string operation)
        {
            var data = DailyTargetDetailQueries.GetByOperation(companyId, produceDate, operation);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
               .SetResult(true, string.Empty)
               .SetData(data);
        }
        [HttpGet]
        [Route("with_alloc_by_size")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<AllocDailyOutputDtos>>>> GetAllocQuantityBySize(
        [FromQuery] string companyId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string operation)
        {
            var data = DailyTargetDetailQueries.GetAllocBySize(companyId, fromDate, toDate, operation);
            return new CommonResponseModel<IEnumerable<AllocDailyOutputDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_offset")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetByOffset(
        [FromQuery] string companyId, [FromQuery] DateTime produceDate, [FromQuery] string operation)
        {
            var data = DailyTargetDetailQueries.GetByOffset(companyId, produceDate, operation);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_work_centerid")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetDailyTargetDetailByWorkCenterId(
        [FromQuery] string departmentId, [FromQuery] DateTime produceDate)
        {
            var data = DailyTargetDetailQueries.GetByWorkCenterId(departmentId, produceDate);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetDailyTargetDetailByDate(
            [FromQuery] DateTime ProduceDate)
        {
            var data = DailyTargetDetailQueries.GetByDate(ProduceDate);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_month")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
           (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<MonthDailyTargetDetailDtos>>>> GetDailyTargetDetailByMonth(
         [FromQuery] string CompanyId,  [FromQuery] DateTime ProduceDate)
        {
            var data = DailyTargetDetailQueries.GetByMonth(CompanyId,ProduceDate);
            return new CommonResponseModel<IEnumerable<MonthDailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("to_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetDailyTargetDetailToDate(
          [FromQuery] string CompanyId,  [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate)
        {
            var data = DailyTargetDetailQueries.GetToDate(CompanyId,FromDate,ToDate);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpGet]
        [Route("with_operation_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetDailyTargetDetailByOperationDate(
            [FromQuery] string customerId, [FromQuery] string operation, [FromQuery] DateTime produceDate)
        {
            var data = DailyTargetDetailQueries.GetByOperationDate(customerId, operation,produceDate);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpGet]
        [Route("order_summary")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<XAFDailyTargetDetailSummaryDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<XAFDailyTargetDetailSummaryDtos>>>> GetDailyTargetDetailSummaryXAF(
            [FromQuery] string customerID, [FromQuery] string style, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate)
        {
            var data = DailyTargetDetailQueries.GetDailyTargetDetailSummary(customerID, style, FromDate, ToDate);
            return new CommonResponseModel<IEnumerable<XAFDailyTargetDetailSummaryDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpGet]
        [Route("summary_month")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailSummaryDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailSummaryDtos>>>> GetDailyTargetDetailSummaryByMonth(
        [FromQuery] string CompanyId,[FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate)
        {
            var data = DailyTargetDetailQueries.GetSummaryByMonth(CompanyId,FromDate,ToDate);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailSummaryDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPost]
        [Route("with_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>>> GetAllDailyTargetDetailByDate(
            [FromQuery] DailyTargetSummarySearchModel model)
        {
            var data = DailyTargetDetailQueries.GetByDate(model.ProduceDate, model.DepartmentIds);
            return new CommonResponseModel<IEnumerable<DailyTargetDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPost]
        [Route("total_order_quantity")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<LSStyleOrderQuantityDtos>>),
           (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<LSStyleOrderQuantityDtos>>>> GetTotalOrderQuantity(
           [FromBody] List<string> LSStyles)
        {
            var data = DailyTargetDetailQueries.GetOrderOutputQuantity(LSStyles);
            return new CommonResponseModel<IEnumerable<LSStyleOrderQuantityDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<DailyTargetDetail>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<DailyTargetDetail>>> UpdateDailyTargetDetail(
            [FromBody] UpdateDailyTargetDetailCommand command)
        {
            var commandResult = new CommonCommandResultHasData<DailyTargetDetail>();
            logger.LogInformation("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            //command.UserName = HttpContext.GetClaimByKey("UserName");
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<DailyTargetDetail>();

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
        [ProducesResponseType(typeof(CommonResponseModel<DailyTargetDetail>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<DailyTargetDetail>>> CreateDailyTargetDetail(
            [FromBody] CreateDailyTargetDetailCommand command)
        {
            var commandResult = new CommonCommandResultHasData<DailyTargetDetail>();
            logger.LogInformation("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            //command.UserName = HttpContext.GetClaimByKey("UserName");
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<DailyTargetDetail>();

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
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteDailyTargetDetail(
            int Id)
        {
            var command = new DeleteDailyTargetDetailCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            command.UserName = HttpContext.GetClaimByKey("UserName");
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
        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetail>>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetail>>>> Bulk(
            [FromBody] BulkDailyTargetDetailCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>();
            logger.LogInformation("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            command.UserName = HttpContext.GetClaimByKey("UserName");
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<IEnumerable<DailyTargetDetail>>();

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
        [HttpPost]
        [Route("bulk_offset")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDetail>>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDetail>>>> BulkOffset(
            [FromBody] BulkOffsetDailyTargetDetailCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>();
            logger.LogInformation("{@time} - Sending create bulk offset daily target detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create bulk offset daily target detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            command.UserName = HttpContext.GetClaimByKey("UserName");
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<IEnumerable<DailyTargetDetail>>();

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
    }
}
