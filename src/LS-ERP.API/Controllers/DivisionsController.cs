using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Dtos.Division;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class DivisionsController : ControllerBase
    {
        private readonly ILogger<DivisionsController> logger;
        private readonly IMediator mediator;
        private readonly IDivisionQueries divisionQueries;

        public DivisionsController(ILogger<DivisionsController> logger,
            IMediator mediator,
            IDivisionQueries divisionQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.divisionQueries = divisionQueries;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DivisionDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<DivisionDtos>> GetDivisions()
        {
            return Ok(divisionQueries.GetDivisions());
        }

        [HttpGet]
        [Route("{ID}")]
        [ProducesResponseType(typeof(DivisionDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<DivisionDtos>> GetDivision(string ID)
        {
            var division = divisionQueries.GetDivision(ID);

            if (division != null)
                return Ok(division);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async  Task<ActionResult<CommonResponseModel>> CreateDivision([FromBody]CreateDivisionCommand command)
        {
            var commandResult = new CommonCommandResult<Division>();
            logger.LogInformation("{@time} - Sending create division command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> UpdateDivision([FromBody] UpdateDivisionCommand command)
        {
            var commandResult = new CommonCommandResult<Division>();
            logger.LogInformation("{@time} - Sending update division command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> DeleteDivision([FromBody] DeleteDivisionCommand command)
        {
            var commandResult = new CommonCommandResult<Division>();
            logger.LogInformation("{@time} - Sending delete division command with request {@request}",
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
