using Common.Model;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.XAF.Module.DomainComponent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportQueries reportQueries;

        public ReportsController(IReportQueries reportQueries)
        {
            this.reportQueries = reportQueries;
        }

        [HttpGet]
        [Route("item-summary/{fromDate}/{toDate}")]
        [ProducesResponseType(typeof(CommonResponseModel<List<ItemReport>>), 200)]
        public async Task<ActionResult<CommonResponseModel<List<ItemReport>>>> GetItemReport(
            DateTime fromDate, DateTime toDate, [FromQuery]string customerID, [FromQuery]string keywords)
        {
            var response = new CommonResponseModel<List<ItemReportDto>>();
            var data = reportQueries.GetItemReportDtos(keywords, customerID, fromDate, toDate);
            return Ok(response.SetResult(true, string.Empty).SetData(data));
        }

        [HttpGet]
        [Route("item-season-report")]
        [ProducesResponseType(typeof(CommonResponseModel<List<SeasonReportDto>>), 200)]
        public async Task<ActionResult<CommonResponseModel<List<SeasonReportDto>>>> GetSeasonReport(
            [FromQuery]string season, [FromQuery]string customerID, [FromQuery]string style, [FromQuery]string keyword)
        {
            var response = new CommonResponseModel<List<SeasonReportDto>>();
            var data = reportQueries.GetSeasonReport(customerID, style, season, keyword);
            return Ok(response.SetResult(true, string.Empty).SetData(data));
        }

        [HttpGet]
        [Route("inventory_report")]
        public async Task<ActionResult<CommonResponseModel<List<InventoryReportDto>>>> GetInventoryReport(
            [FromQuery]string purchaseOrder = "", [FromQuery]string search = "", 
            [FromQuery]string storageCode = "", [FromQuery]string customerID = "")
        {
            var response = new CommonResponseModel<List<InventoryReportDto>>();
            var data = reportQueries.GetInventoryReport(purchaseOrder, search, storageCode, customerID);
            return Ok(response.SetResult(true, string.Empty).SetData(data));
        }
    }
}
