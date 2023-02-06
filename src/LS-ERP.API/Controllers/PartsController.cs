using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly ILogger<PartsController> logger;
        private readonly IMediator mediator;

        public PartsController(ILogger<PartsController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<ActionResult<CommonResponseModel>> Import(
            [FromForm] ImportPartCommand command)
        {
            var commandResult = new CommonCommandResult<Part>();
            logger.LogInformation("{@time} - Sending import Part command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("001", "Import successfully").SetData(commandResult.Result); ;
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
