using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadingPlansController : ControllerBase
    {
        private readonly ILogger<LoadingPlansController> logger;
        private readonly IMediator mediator;
        private readonly ILoadingPlanQueries loadingPlanQueries;

        public LoadingPlansController(ILogger<LoadingPlansController> logger,
            IMediator mediator,
            ILoadingPlanQueries loadingPlanQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.loadingPlanQueries = loadingPlanQueries;
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel<List<LoadingPlan>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<LoadingPlan>>>> Import(
            [FromForm] ImportLoadingPlanCommand command)
        {
            var response = new CommonResponseModel<List<LoadingPlan>>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message)
                .SetData(commandResult.Data));
        }

        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<LoadingPlan>>>> Bulk(
            [FromBody] BulkLoadingPlanCommand command)
        {
            var response = new CommonResponseModel<object>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message));
        }

        [HttpGet]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<LoadingPlan>>>> Get()
        {
            var response = new CommonResponseModel<object>();
            var data = loadingPlanQueries.GetAll();
            return Ok(response
                .SetResult(true, string.Empty)
                .SetData(data));
        }
    }
}
