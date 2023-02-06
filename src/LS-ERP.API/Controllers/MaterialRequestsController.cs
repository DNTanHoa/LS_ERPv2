using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialRequestsController : ControllerBase
    {
        private readonly ILogger<MaterialRequestsController> logger;
        private readonly IMediator mediator;

        public MaterialRequestsController(ILogger<MaterialRequestsController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("process-requests")]
        public async Task<ActionResult<CommonResponseModel>> PullBom(
            [FromBody] ProcessMaterialRequestCommand command)
        {
            var commandResult = new CommonCommandResult();

            logger.LogInformation("{@time} - Process material command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Process material command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.Success)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
