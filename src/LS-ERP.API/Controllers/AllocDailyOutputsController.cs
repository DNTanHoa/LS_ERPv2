using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllocDailyOutputsController : ControllerBase
    {
        private readonly ILogger<AllocDailyOutputsController> logger;
        private readonly IAllocDailyOutputQueries AllocDailyOutputQueries;
        private readonly IMediator mediator;

        public AllocDailyOutputsController(ILogger<AllocDailyOutputsController> logger,
            IAllocDailyOutputQueries AllocDailyOutputQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.AllocDailyOutputQueries = AllocDailyOutputQueries;
            this.mediator = mediator;
        }
        [HttpGet]
        [Route("with_target_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<AllocDailyOutputDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<AllocDailyOutputDtos>>>> GetByTargetID(
         [FromQuery] int id)
        {
            var data = AllocDailyOutputQueries.GetByTargetID(id);
            return new CommonResponseModel<IEnumerable<AllocDailyOutputDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<AllocDailyOutputDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<AllocDailyOutputDtos>>>> GetByID(
         [FromQuery] int id)
        {
            var data = AllocDailyOutputQueries.GetByID(id);
            return new CommonResponseModel<IEnumerable<AllocDailyOutputDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<AllocDailyOutput>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<AllocDailyOutput>>> UpdateAllocDailyOutput(
            [FromBody] UpdateAllocDailyOutputCommand command)
        {
            var commandResult = new CommonCommandResultHasData<AllocDailyOutput>();
            logger.LogInformation("{@time} - Sending update alloc daily target command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update alloc daily target command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<AllocDailyOutput>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Update successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }
        
    }
}
