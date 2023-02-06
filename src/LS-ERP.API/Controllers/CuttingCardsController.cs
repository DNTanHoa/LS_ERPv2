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
    public class CuttingCardsController : ControllerBase
    {
        private readonly ILogger<CuttingCardsController> logger;
        private readonly ICuttingCardQueries CuttingCardQueries;
        private readonly IMediator mediator;

        public CuttingCardsController(ILogger<CuttingCardsController> logger,
            ICuttingCardQueries CuttingCardQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.CuttingCardQueries = CuttingCardQueries;
            this.mediator = mediator;
        }
        #region For compling

        // Get list LSStyle cutting - compling status
        [HttpGet]
        [Route("compling_report_status")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<ComplingReportStatusDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<ComplingReportStatusDtos>>>> GetComplingReportStatus(
         [FromQuery] string CompanyID, [FromQuery] string CustomerName, [FromQuery] string KeyWord, [FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate)
        {
            var data = CuttingCardQueries.GetComplingReportStatus(CompanyID, CustomerName, KeyWord, FromDate,ToDate);
            return new CommonResponseModel<IEnumerable<ComplingReportStatusDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        #endregion

        [HttpGet]
        [Route("with_cutting_output_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingCardDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetByCuttingOutputID(
         [FromQuery] int CuttingOutputID)
        {
            var data = CuttingCardQueries.GetByCuttingOutputID(CuttingOutputID);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingCardDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetByID(
         [FromQuery] string Id)
        {
            var data = CuttingCardQueries.GetByID(Id);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("search_location_with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingCardDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetLocationByID(
         [FromQuery] string Id)
        {
            var data = CuttingCardQueries.GetLocationByID(Id);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_master_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingCardDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetByMasterID(
         [FromQuery] string Id)
        {
            var data = CuttingCardQueries.GetByMasterID(Id);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_company_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingCardDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetByCompanyID(
         [FromQuery] string CompanyID, [FromQuery] string Operation, [FromQuery] string KeyWord, [FromQuery] DateTime ProduceDate)
        {
            var data = CuttingCardQueries.GetByCompanyID(CompanyID,Operation,KeyWord,ProduceDate);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("master_with_company_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingCardDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetMasterByCompanyID(
         [FromQuery] string CompanyID, [FromQuery] string Operation, [FromQuery] string KeyWord, [FromQuery] DateTime ProduceDate)
        {
            var data = CuttingCardQueries.GetMasterByCompanyID(CompanyID, Operation, KeyWord,ProduceDate);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("for_supper_market_with_company_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CuttingCardDtos>>),
       (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetByCompanyIDForSupperMarket(
        [FromQuery] string CompanyID, [FromQuery] string Operation, [FromQuery] string KeyWord )
        {
            var data = CuttingCardQueries.GetByCompanyIDForSupperMarket(CompanyID, Operation, KeyWord);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<CuttingCard>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<CuttingCard>>> UpdateCuttingCard(
            [FromBody] UpdateCuttingCardCommand command)
        {
            var commandResult = new CommonCommandResultHasData<CuttingCard>();
            logger.LogInformation("{@time} - Sending update cutting card command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update cutting card command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<CuttingCard>();

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
        [HttpPut]
        [Route("multi_cards_location")]
        [ProducesResponseType(typeof(CommonResponseModel<CuttingCard>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<CuttingCard>>> UpdateCuttingCard(
            [FromBody] UpdateBulkCuttingCardCommand command)
        {
            var commandResult = new CommonCommandResultHasData<CuttingCard>();
            logger.LogInformation("{@time} - Sending update multi cutting card command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update multi cutting card command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<CuttingCard>();

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
        [ProducesResponseType(typeof(CommonResponseModel<CuttingCard>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<CuttingCard>>> CreateCuttingCard(
            [FromBody] CreateCuttingCardCommand command)
        {
            var commandResult = new CommonCommandResultHasData<CuttingCard>();
            logger.LogInformation("{@time} - Sending create cutting card command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create cutting card command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<CuttingCard>();

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
        [Route("with_multi_master_id")] 
        [ProducesResponseType(typeof(CommonResponseModel<CuttingCardDtos>), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<CuttingCardDtos>>>> GetByMultiMasterID(
         [FromBody] List<string> listID)
        {
            var data = CuttingCardQueries.GetByMultiMasterID(listID);
            return new CommonResponseModel<IEnumerable<CuttingCardDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
    }
}
