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
    public class OperationsController : ControllerBase
    {
        private readonly ILogger<OperationsController> logger;
        private readonly IOperationQueries OperationQueries;
        private readonly IMediator mediator;

        public OperationsController(ILogger<OperationsController> logger,
            IOperationQueries OperationQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.OperationQueries = OperationQueries;
            this.mediator = mediator;
        }
        [HttpGet]        
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<OperationDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<OperationDtos>>>> GetAll(
        )
        {
            var data = OperationQueries.GetAll();
            return new CommonResponseModel<IEnumerable<OperationDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<OperationDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<OperationDtos>>>> GetByCuttingOutputID(
         [FromQuery] string Id)
        {
            var data = OperationQueries.GetByID(Id);
            return new CommonResponseModel<IEnumerable<OperationDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_group")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<OperationDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<OperationDtos>>>> GetByGroup(
         [FromQuery] string Group)
        {
            var data = OperationQueries.GetByGroup(Group);
            return new CommonResponseModel<IEnumerable<OperationDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<Operation>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<Operation>>> UpdateOperation(
            [FromBody] UpdateOperationCommand command)
        {
            var commandResult = new CommonCommandResultHasData<Operation>();
            logger.LogInformation("{@time} - Sending update cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<Operation>();

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
        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel<Operation>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<Operation>>> CreateOperation(
            [FromBody] CreateOperationCommand command)
        {
            var commandResult = new CommonCommandResultHasData<Operation>();
            logger.LogInformation("{@time} - Sending create cutting card command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create cutting card command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<Operation>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Create successfully")
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }
        [HttpDelete]
        //[Route("{Id:int}")]
        //[Route("id")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteOperation(
           string Id)
        {
            var command = new DeleteOperationCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending delete cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete cutting output command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<object>();

            if (commandResult.Success)
            {
                response.SetResult(true, "Delete successfully");
            }
            else
            {
                response.SetResult(false, commandResult.Message);
            }

            return response;
        }
    }
}
