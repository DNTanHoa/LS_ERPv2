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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastEntriesController : ControllerBase
    {
        private readonly ILogger<ForecastEntriesController> logger;
        private readonly IMediator mediator;

        public ForecastEntriesController(ILogger<ForecastEntriesController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpDelete]
        [Route("{ID:long}")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> DeleteForecastEntry(long ID)
        {
            var command = new DeleteForecastEnrtryCommand
            {
                ForecastEntryID = ID,
            };

            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending delete forecast entry command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete salesOrder command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.Success)
            {
                response.SetResult("001", "Delete successfully");
            }
            else
            {
                response.SetResult("101", commandResult.Message);
            }

            return response;
        }
    }
}
