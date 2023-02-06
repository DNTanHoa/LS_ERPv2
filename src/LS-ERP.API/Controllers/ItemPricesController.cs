using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class ItemPricesController : ControllerBase
    {
        private readonly ILogger<ItemPricesController> logger;
        private readonly IMediator mediator;

        public ItemPricesController(ILogger<ItemPricesController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("import")]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> Import([FromForm] ImportItemPriceCommand command)
        {
            var commandResult = new CommonCommandResult<List<ItemPrice>>();
            logger.LogInformation("{@time} - Sending import item price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending import item price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Import successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("create-multi")]
        public async Task<ActionResult<CommonResponseModel>> CreateMulti([FromBody] CreateMultiItemPriceCommand command)
        {
            var commandResult = new CommonCommandResult<List<ItemPrice>>();
            logger.LogInformation("{@time} - Sending create multi item price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Create successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
