//using DevExpress.XtraEditors.Filtering.Templates;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class StorageDetailsController : ControllerBase
    {
        private readonly ILogger<StorageDetailsController> logger;
        private readonly IMediator mediator;
        private readonly IStorageDetailQueries storageDetailQueries;

        public StorageDetailsController(ILogger<StorageDetailsController> logger,
            IMediator mediator,
            IStorageDetailQueries storageDetailQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.storageDetailQueries = storageDetailQueries;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StorageDetail>), (int)HttpStatusCode.OK)]
        public ActionResult<List<StorageDetail>> GetStorageDetails()
        {
            var data = storageDetailQueries.GetStorageDetails();
            return Ok(data);
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> Import([FromForm] ImportStorageDetailCommand command)
        {
            var commandResult = new ImportStorageDetailResult();
            logger.LogInformation("{@time} - Sending create sale order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create sale order command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Import successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("report")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> ReportStorageDetail(
            [FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] string storageCode,
            [FromQuery] string customerID, [FromQuery] string productionMethodCode,
            [FromQuery] decimal onHandQuantity)
        {
            var response = new CommonResponseModel();
            //DateTime fromDateTo = fromDate.Date;

            var data = storageDetailQueries.GetSummaryStorageDetailReports(customerID, storageCode,
                fromDate != null ? DateTime.Parse(fromDate) : null,
                toDate != null ? DateTime.Parse(toDate) : null,
                productionMethodCode, onHandQuantity);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
    }
}
