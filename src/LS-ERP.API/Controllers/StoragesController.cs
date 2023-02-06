using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoragesController : ControllerBase
    {
        private readonly IMediator mediator;

        public StoragesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel<List<ImportStorageDto>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<ImportStorageDto>>>> Import(
            [FromForm]ImportStorageCommand command)
        {
            var response = new CommonResponseModel<List<ImportStorageDto>>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message)
                .SetData(commandResult.Data));
        }

        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<ImportStorageDto>>>> Bulk(
            [FromBody] BulkStorageCommand command)
        {
            var response = new CommonResponseModel<object>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message));
        }
    }
}
