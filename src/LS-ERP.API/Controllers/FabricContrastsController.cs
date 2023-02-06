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
    public class FabricContrastsController : ControllerBase
    {
        private readonly ILogger<FabricContrastsController> logger;
        private readonly IFabricContrastQueries FabricContrastQueries;
        private readonly IMediator mediator;

        public FabricContrastsController(ILogger<FabricContrastsController> logger,
            IFabricContrastQueries FabricContrastQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.FabricContrastQueries = FabricContrastQueries;
            this.mediator = mediator;
        }
        [HttpGet]        
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<FabricContrastDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<FabricContrastDtos>>>> GetAll(
        )
        {
            var data = FabricContrastQueries.GetAll();
            return new CommonResponseModel<IEnumerable<FabricContrastDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_id")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<FabricContrastDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<FabricContrastDtos>>>> GetByID(
         [FromQuery] int Id)
        {
            var data = FabricContrastQueries.GetByID(Id);
            return new CommonResponseModel<IEnumerable<FabricContrastDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
       
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<FabricContrast>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<FabricContrast>>> UpdateFabricContrast(
            [FromBody] UpdateFabricContrastCommand command)
        {
            var commandResult = new CommonCommandResultHasData<FabricContrast>();
            logger.LogInformation("{@time} - Sending update operation detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update operation detail command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<FabricContrast>();

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
        //[HttpPost]
        //[ProducesResponseType(typeof(CommonResponseModel<FabricContrast>), (int)HttpStatusCode.Created)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //public async Task<ActionResult<CommonResponseModel<FabricContrast>>> CreateFabricContrast(
        //    [FromBody] CreateFabricContrastCommand command)
        //{
        //    var commandResult = new CommonCommandResultHasData<FabricContrast>();
        //    logger.LogInformation("{@time} - Sending create operation detail command with request {@request}",
        //        DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
        //    LogHelper.Instance.Information("{@time} - Sending create operation detail command with request {@request}",
        //        DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

        //    commandResult = await mediator.Send(command);
        //    var response = new CommonResponseModel<FabricContrast>();

        //    if (commandResult.Success)
        //    {
        //        response.SetResult(true, "Create successfully")
        //            .SetData(commandResult.Data);
        //    }
        //    else
        //    {
        //        response.SetResult(false, commandResult.Message);
        //    }

        //    return response;
        //}
        //[HttpDelete]
        ////[Route("{Id:int}")]
        ////[Route("id")]
        //[ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //public async Task<ActionResult<CommonResponseModel<object>>> DeleteFabricContrast(
        //   string Id)
        //{
        //    var command = new DeleteFabricContrastCommand()
        //    {
        //        ID = Id
        //    };
        //    var commandResult = new CommonCommandResult();
        //    logger.LogInformation("{@time} - Sending delete operation detail command with request {@request}",
        //        DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
        //    LogHelper.Instance.Information("{@time} - Sending delete operation detail command with request {@request}",
        //        DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

        //    commandResult = await mediator.Send(command);
        //    var response = new CommonResponseModel<object>();

        //    if (commandResult.Success)
        //    {
        //        response.SetResult(true, "Delete successfully");
        //    }
        //    else
        //    {
        //        response.SetResult(false, commandResult.Message);
        //    }

        //    return response;
        //}
    }
}
