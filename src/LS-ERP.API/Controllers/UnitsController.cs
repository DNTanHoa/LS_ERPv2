using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Unit = LS_ERP.EntityFrameworkCore.Entities.Unit;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly ILogger<Unit> logger;
        private readonly IUnitQueries unitQueries;
        private readonly IMediator mediator;

        public UnitsController(ILogger<Unit> logger,
            IUnitQueries unitQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.unitQueries = unitQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UnitDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<UnitDtos>> GetUnits()
        {
            return Ok(unitQueries.GetUnits());
        }

        [HttpGet]
        [Route("{ID}")]
        [ProducesResponseType(typeof(UnitDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<UnitDtos>> GetUnit(string ID)
        {
            var Unit = unitQueries.GetUnit(ID);

            if (Unit != null)
                return Ok(Unit);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreateCurrency([FromBody] CreateUnitCommand command)
        {
            var commandResult = new CommonCommandResult<Unit>();
            logger.LogInformation("{@time} - Sending create Currency command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create Currency command with request {@request}",
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

        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> UpdateCurrency([FromBody] UpdateUnitCommand command)
        {
            var commandResult = new CommonCommandResult<Unit>();
            logger.LogInformation("{@time} - Sending update Unit command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update Unit command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("001", "Update successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpDelete]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> DeleteCurrency([FromBody] DeleteUnitCommand command)
        {
            var commandResult = new CommonCommandResult<Unit>();
            logger.LogInformation("{@time} - Sending delete Unit command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete Unit command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("002", "Delte successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
