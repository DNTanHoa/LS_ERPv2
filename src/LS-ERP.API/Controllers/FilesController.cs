using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.Ultilities.Global;
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
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> logger;
        private readonly IMediator mediator;

        public FilesController(ILogger<FilesController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> Upload([FromForm]UploadCommand command)
        {
            var commandResult = new UploadResult();
            logger.LogInformation("{@time} - Sending upload file command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending upload file command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Upload successfully")
                    .SetData(AppGlobal.Host + commandResult.Url);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
