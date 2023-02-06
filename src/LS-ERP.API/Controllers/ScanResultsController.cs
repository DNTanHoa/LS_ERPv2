using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanResultsController : ControllerBase
    {
        private readonly ILogger<ScanResultsController> logger;
        private readonly IMediator mediator;
        private readonly IScanResultQueries scanResultQueries;

        public ScanResultsController(ILogger<ScanResultsController> logger,
           IMediator mediator,
           IScanResultQueries scanResultQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.scanResultQueries = scanResultQueries;
        }

        [HttpGet]
        [Route("Summary")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<ScanResultDto>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<ScanResultDto>>>> GetSummaryScanResult(
            [FromQuery]string company, [FromQuery]DateTime summaryDate)
        {
            var data = scanResultQueries.GetSummaryScanResult(company, summaryDate);
            return new CommonResponseModel<IEnumerable<ScanResultDto>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<object>),
        (int) HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<object>>> Bulk(
            [FromBody]BulkScanResultCommand command)
        {
            var commanResult = await mediator.Send(command);
            return new CommonResponseModel<object>()
                .SetResult(commanResult.Success, commanResult.Message);
        }

        [HttpPut]
        [Route("confirm")]
        [ProducesResponseType(typeof(CommonResponseModel<object>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<object>>> ConfirmScanResult(
            [FromBody] ConfirmScanResultCommand command)
        {
            var commanResult = await mediator.Send(command);
            return new CommonResponseModel<object>()
                .SetResult(commanResult.Success, commanResult.Message);
        }

        [HttpPut]
        [Route("po-numbers")]
        [ProducesResponseType(typeof(CommonResponseModel<object>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<object>>> UpdatePONumberScanResult(
            [FromBody] UpdatePONumberScanResultCommand command)
        {
            var commanResult = await mediator.Send(command);
            return new CommonResponseModel<object>()
                .SetResult(commanResult.Success, commanResult.Message);
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteScanResultDetail(
            [FromBody] DeleteScanResultDetailCommand command)
        {
            var commanResult = await mediator.Send(command);
            return new CommonResponseModel<object>()
                .SetResult(commanResult.Success, commanResult.Message);
        }
    }
}
