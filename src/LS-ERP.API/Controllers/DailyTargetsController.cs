using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
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
using Ultils.Extensions;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyTargetsController : ControllerBase
    {
        private readonly ILogger<DailyTargetsController> logger;
        private readonly IMediator mediator;
        private readonly IDailyTargetQueries dailyTargetQueries;

        public DailyTargetsController(
            ILogger<DailyTargetsController> logger,
            IMediator mediator,
            IDailyTargetQueries DailyTargetQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.dailyTargetQueries = DailyTargetQueries;
        }
        [HttpGet]        
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> GetDailyTargetById(
        [FromQuery] int Id)
        {
            var data = dailyTargetQueries.GetById(Id);
            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);            
        }
        [HttpGet]
        [Route("cutting")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> GetCuttingTarget(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string Operation )
        {
            var data = dailyTargetQueries.GetCuttingTarget(CompanyID,FromDate,ToDate,Operation);
            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("sewing")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> GetSewingTarget(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string Operation)
        {
            var data = dailyTargetQueries.GetSewingTarget(CompanyID, FromDate, ToDate, Operation);
            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("merge_block_lsstyle")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetMergeBlockLSStyle(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] bool IsPrint)
        {
            var data = dailyTargetQueries.GetMergeBlockLSStyle(CompanyID, FromDate, ToDate,IsPrint);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("all_merge_block_lsstyle")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetAllMergeBlockLSStyle(
        [FromQuery] string CompanyID)
        {
            var data = dailyTargetQueries.GetAllMergeBlockLSStyle(CompanyID);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("merge_block_lsstyle_quantity")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetMergeBlockLSStyleQuantity(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate
            , [FromQuery] string MergeBlockLSStyle, [FromQuery] string size)
        {
            var data = dailyTargetQueries.GetMergeBlockLSStyleQuantity(CompanyID, FromDate, ToDate, MergeBlockLSStyle,size);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("merge_lsstyle_quantity")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetMergeLSStyleQuantity(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate
            , [FromQuery] string MergeLSStyle, [FromQuery] string Size)
        {
            var data = dailyTargetQueries.GetMergeLSStyleQuantity(CompanyID, FromDate, ToDate, MergeLSStyle,Size);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("lsstyle_quantity")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetLSStyleQuantity(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate
            , [FromQuery] string LSStyle, [FromQuery] string Size)
        {
            var data = dailyTargetQueries.GetLSStyleQuantity(CompanyID, FromDate, ToDate, LSStyle,Size);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("cutting_customer")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
       (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetCuttingCustomer(
       [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate)
        {
            var data = dailyTargetQueries.GetCuttingCustomer(CompanyID, FromDate, ToDate);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("cutting_lsstyle")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetCuttingLSStyle(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate,
        [FromQuery] string MergeLSStyle, [FromQuery] string MergeBlockLSStyle, [FromQuery] bool IsPrint)
        {
            var data = dailyTargetQueries.GetCuttingLSStyle(CompanyID, FromDate, ToDate,MergeLSStyle,MergeBlockLSStyle,IsPrint);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("merge_lsstyle")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<string>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<string>>>> GetMergeLSStyle(
        [FromQuery] string CompanyID, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string MergeBlockLSStyle, [FromQuery] bool IsPrint)
        {
            var data = dailyTargetQueries.GetMergeLSStyle(CompanyID, FromDate, ToDate, MergeBlockLSStyle,IsPrint);
            return new CommonResponseModel<IEnumerable<string>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_centerid")]        
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> GetDailyTargetByWorkCenterId(
        [FromQuery] string WorkCenterID)
        {
            var data = dailyTargetQueries.GetByWorkCenterId(WorkCenterID);
            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_center")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> GetDailyTargetByWorkCenterId(
        [FromQuery] DateTime ProduceDate,[FromQuery] string DepartmentID)
        {
            var data = dailyTargetQueries.GetByWorkCenter(ProduceDate,DepartmentID);
            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_center_ids")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDtos>>),
       (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> GetDailyTargetByListWorkCenterId(
       [FromQuery] string listWorkCenterID)
        {
            var str = listWorkCenterID;
            str = str.Replace("\"", "");
            str = str.Replace("[", "");
            str = str.Replace("]", "");
            var arr = str.Split(',');
            var list = new List<string>();
            for(int i = 0; i< arr.Length; i++)
            {
                list.Add(arr[i].Trim());
            }
            var data = dailyTargetQueries.GetByListWorkCenterId(list);
           
            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> GetDailyTargetByDate(
            [FromQuery] DateTime ProduceDate, [FromQuery] List<string> DepartmentIds)
        {
            var data = dailyTargetQueries.GetByDate(ProduceDate,DepartmentIds);
            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpGet]
        [Route("with_companyid_date")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTargetDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> GetDailyTargetByCompanyID(
            [FromQuery] string companyID, [FromQuery] DateTime ProduceDate)
        {
            var data = dailyTargetQueries.GetByCompanyID(companyID, ProduceDate);
            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpGet]
        [Route("overview")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTagetOverviewDtos>>),
            (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTagetOverviewDtos>>>> GetDailyTargetOverview(
            [FromQuery] string CompanyID)
        {
            var data = dailyTargetQueries.GetOverview(CompanyID);
            return new CommonResponseModel<IEnumerable<DailyTagetOverviewDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<DailyTarget>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<DailyTarget>>> UpdateDailyTarget(
            [FromBody] UpdateDailyTargetCommand command)
        {
            var commandResult = new CommonCommandResultHasData<DailyTarget>();
            logger.LogInformation("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<DailyTarget>();

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
        [ProducesResponseType(typeof(CommonResponseModel<DailyTarget>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<DailyTarget>>> CreateDailyTarget(
            [FromBody] CreateDailyTargetCommand command)
        {
            var commandResult = new CommonCommandResultHasData<DailyTarget>();
            logger.LogInformation("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<DailyTarget>();

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
        [Route("with_center_ids")]
        [ProducesResponseType(typeof(CommonResponseModel<DailyTargetDtos>), (int)HttpStatusCode.Created)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTargetDtos>>>> getDailyTargetByListWorkCenterID(
            [FromBody] List<string> listIds)
        {
            var data = dailyTargetQueries.GetByListWorkCenterId(listIds);

            return new CommonResponseModel<IEnumerable<DailyTargetDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);

        }
        [HttpPost]
        [Route("check_offset_lsstyle")]
        //[ProducesResponseType(typeof(CommonResponseModel<LSStyleCompareDtos>), (int)HttpStatusCode.Created)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<ActionResult<CommonResponseModel<IEnumerable<LSStyleCompareDtos>>>> checkOffsetLSStyle(
            [FromBody] BulkLSStyleCompareDtos BulkLSStyleCompareDtos)
        {
            var data = dailyTargetQueries.CheckOffsetLSStyle(BulkLSStyleCompareDtos);

            return new CommonResponseModel<IEnumerable<LSStyleCompareDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);

        }
        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTarget>>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTarget>>>> Bulk(
            [FromBody] BulkDailyTargetCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<DailyTarget>>();
            logger.LogInformation("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            command.UserName = HttpContext.GetClaimByKey("UserName");
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<IEnumerable<DailyTarget>>();

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
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<DailyTarget>>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<DailyTarget>>>> BulkOffset(
            [FromBody] BulkOffsetDailyTargetCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<DailyTarget>>();
            logger.LogInformation("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            command.UserName = HttpContext.GetClaimByKey("UserName");
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<IEnumerable<DailyTarget>>();

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
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<ActionResult<CommonResponseModel>> ImportDailyTarget(
            [FromForm] ImportDailyTargetCommand command)
        {
            var commandResult = new ImportDailyTargetResult();
            logger.LogInformation("{@time} - Sending import daily target command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending import daily target command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
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

        [HttpDelete]
        [Route("{Id:int}")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteDailyTarget(
            int Id)
        {
            var command = new DeleteDailyTargetCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending update job output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update job output command with request {@request}",
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
