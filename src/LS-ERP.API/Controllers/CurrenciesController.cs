using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Dtos.Currency;
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
    public class CurrenciesController : ControllerBase
    {
        private readonly ILogger<CurrenciesController> logger;
        private readonly ICurrencyQueries currencyQueries;
        private readonly IMediator mediator;

        public CurrenciesController(ILogger<CurrenciesController> logger,
            ICurrencyQueries currencyQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.currencyQueries = currencyQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CurrencyDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<CurrencyDtos>> GetCurrencies()
        {
            return Ok(currencyQueries.GetCurrencies());
        }

        [HttpGet]
        [Route("{ID}")]
        [ProducesResponseType(typeof(CurrencyDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<CurrencyDtos>> GetCurrency(string ID)
        {
            var division = currencyQueries.GetCurrency(ID);

            if (division != null)
                return Ok(division);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreateCurrency([FromBody] CreateCurrencyCommand command)
        {
            var commandResult = new CommonCommandResult<Currency>();
            logger.LogInformation("{@time} - Sending create Currency command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> UpdateCurrency([FromBody] UpdateCurrencyCommand command)
        {
            var commandResult = new CommonCommandResult<Currency>();
            logger.LogInformation("{@time} - Sending update currency command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> DeleteCurrency([FromBody] DeleteCurrencyCommand command)
        {
            var commandResult = new CommonCommandResult<Currency>();
            logger.LogInformation("{@time} - Sending delete Currency command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("002", "Delete successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
