using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Ressult;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkCentersController : ControllerBase
    {
        private readonly ILogger<WorkCentersController> logger;
        private readonly IWorkCenterQueries WorkCenterQueries;
        private readonly IMediator mediator;

        public WorkCentersController(ILogger<WorkCentersController> logger,
            IWorkCenterQueries WorkCenterQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.WorkCenterQueries = WorkCenterQueries;
            this.mediator = mediator;
        }
        [HttpGet]
        [Route("work_center")]
        public async Task<ActionResult<CommonResponseModel>> Get(
            [FromQuery] string departmentId)
        {
            var response = new CommonResponseModel();
            var data = WorkCenterQueries.Get(departmentId).FirstOrDefault();
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        [HttpGet]
        [Route("work_center_cutting")]
        public async Task<ActionResult<CommonResponseModel>> GetCuttingCenterByCompany(
            [FromQuery] string companyID)
        {
            var response = new CommonResponseModel();
            var data = WorkCenterQueries.GetCuttingCenterByCompany(companyID);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        [HttpGet]
        [Route("work_center_sewing")]
        public async Task<ActionResult<CommonResponseModel>> GetSewingCenterByCompany(
            [FromQuery] string companyID)
        {
            var response = new CommonResponseModel();
            var data = WorkCenterQueries.GetSewingCenterByCompany(companyID);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        [HttpGet]     
        public async Task<ActionResult<CommonResponseModel>> Get()
        {
            var response = new CommonResponseModel();
            var data = WorkCenterQueries.Get();
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        [HttpGet]
        [Route("work_center_with_listid")]
        public async Task<ActionResult<CommonResponseModel>> Get(
            [FromQuery] List<string> ListWorkCenterID)
        {
            var response = new CommonResponseModel();
            var data = WorkCenterQueries.Get(ListWorkCenterID);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
    }
}
