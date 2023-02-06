using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobHeadsController : ControllerBase
    {
        private readonly ILogger<JobHeadsController> logger;
        private readonly IMediator mediator;
        private readonly IJobHeadQueries jobHeadQueries;

        public JobHeadsController(ILogger<JobHeadsController> logger,
            IMediator mediator,
            IJobHeadQueries jobHeadQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.jobHeadQueries = jobHeadQueries;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<JobHeadSummaryDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<JobHeadSummaryDtos>> GetJobHeads()
        {
            return Ok(jobHeadQueries.GetSummaryDtos());
        }

        [HttpGet]
        [Route("style/{style}")]
        [ProducesResponseType(typeof(IEnumerable<JobHeadSummaryDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<JobHeadSummaryDtos>> GetJobHeads(string style)
        {
            return Ok(jobHeadQueries.GetSummaryDtos(style));
        }

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<JobHeadSummaryDtos>>), (int)HttpStatusCode.OK)]
        public ActionResult<CommonResponseModel<IEnumerable<JobHeadSummaryDtos>>> FilterJobHeads(
            [FromQuery]string customerID, [FromQuery] string style, 
            [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            if (fromDate == null)
                fromDate = DateTime.Now;
            if (toDate == null)
                toDate = DateTime.Now.AddDays(1);

            var response = new CommonResponseModel<IEnumerable<JobHeadSummaryDtos>>();
            var data = jobHeadQueries.Filter(customerID, style, fromDate.Value, toDate.Value);
            return Ok(response.SetData(data).SetResult(true, string.Empty));
        }

        [HttpGet]
        [Route("{number}")]
        [ProducesResponseType(typeof(JobHeadSummaryDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<JobHeadSummaryDtos>> GetJobHead(string number)
        {
            var jobHead = jobHeadQueries.GetDetailDtos(number);

            if (jobHead != null)
                return Ok(jobHead);

            return NotFound();
        }

        [HttpGet]
        [Route("{keyword}/{pageIndex}/{pageSize}")]
        [ProducesResponseType(typeof(Model.CommonPagingResponseModel<IEnumerable<JobHeadSummaryDtos>>), 
            (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<JobHeadSummaryDtos>> SearchJobHeadPaging(string keyword,
            int pageIndex, int pageSize)
        {
            var (data, totalPage, totalCount) =
                jobHeadQueries.GetJobHeadSummaryDtosPaging(keyword, pageIndex, pageSize);

            var response = new Model.CommonPagingResponseModel<IEnumerable<JobHeadSummaryDtos>>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPage = totalPage,
                Data = data,
            };

            return Ok(response);
        }
    }
}
