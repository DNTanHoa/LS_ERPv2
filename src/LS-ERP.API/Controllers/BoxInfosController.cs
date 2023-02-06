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
    public class BoxInfosController : ControllerBase
    {
        private readonly ILogger<BoxInfosController> logger;
        private readonly IMediator mediator;
        private readonly IBoxInfoQueries boxInfoQueries;

        public BoxInfosController(ILogger<BoxInfosController> logger,
            IMediator mediator,
            IBoxInfoQueries boxInfoQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.boxInfoQueries = boxInfoQueries;
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel<List<BoxInfo>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<BoxInfo>>>> Import(
            [FromForm] ImportBoxInfoCommand command)
        {
            var response = new CommonResponseModel<List<BoxInfo>>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message)
                .SetData(commandResult.Data));
        }

        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<BoxInfo>>>> Bulk(
            [FromBody] BulkBoxInfoCommand command)
        {
            var response = new CommonResponseModel<object>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message));
        }

        [HttpGet]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<BoxInfo>>>> Get()
        {
            var response = new CommonResponseModel<object>();
            var data = boxInfoQueries.GetAll();
            return Ok(response
                .SetResult(true, string.Empty)
                .SetData(data));
        }
    }
}
