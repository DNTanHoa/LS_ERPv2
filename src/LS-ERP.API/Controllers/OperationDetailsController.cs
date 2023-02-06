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
    public class OperationDetailsController : ControllerBase
    {
        private readonly ILogger<OperationDetailsController> logger;
        private readonly IOperationDetailQueries OperationDetailQueries;
        private readonly IMediator mediator;

        public OperationDetailsController(ILogger<OperationDetailsController> logger,
            IOperationDetailQueries OperationDetailQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.OperationDetailQueries = OperationDetailQueries;
            this.mediator = mediator;
        }
        [HttpGet]        
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<OperationDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<OperationDetailDtos>>>> GetAll(
        )
        {
            var data = OperationDetailQueries.GetAll();
            return new CommonResponseModel<IEnumerable<OperationDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<OperationDetailDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<OperationDetailDtos>>>> GetByID(
         [FromQuery] string Id)
        {
            var data = OperationDetailQueries.GetByID(Id);
            return new CommonResponseModel<IEnumerable<OperationDetailDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
       
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<OperationDetail>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<OperationDetail>>> UpdateOperationDetail(
            [FromBody] UpdateOperationDetailCommand command)
        {
            var commandResult = new CommonCommandResultHasData<OperationDetail>();
            logger.LogInformation("{@time} - Sending update operation detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update operation detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<OperationDetail>();

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
        [ProducesResponseType(typeof(CommonResponseModel<OperationDetail>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<OperationDetail>>> CreateOperationDetail(
            [FromBody] CreateOperationDetailCommand command)
        {
            var commandResult = new CommonCommandResultHasData<OperationDetail>();
            logger.LogInformation("{@time} - Sending create operation detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create operation detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<OperationDetail>();

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
        public async Task<ActionResult<CommonResponseModel<object>>> DeleteOperationDetail(
           string Id)
        {
            var command = new DeleteOperationDetailCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending delete operation detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete operation detail command with request {@request}",
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
