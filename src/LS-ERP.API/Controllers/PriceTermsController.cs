using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Dtos.PriceTerm;
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
    public class PriceTermsController : ControllerBase
    {
        private readonly ILogger<PriceTermsController> logger;
        private readonly IPriceTermQueries priceTermQueries;
        private readonly IMediator mediator;

        public PriceTermsController(ILogger<PriceTermsController> logger,
            IPriceTermQueries priceTermQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.priceTermQueries = priceTermQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PriceTermDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<PriceTermDtos>> GetDivisions()
        {
            return Ok(priceTermQueries.GetPriceTerms());
        }

        [HttpGet]
        [Route("{Code}")]
        [ProducesResponseType(typeof(PriceTermDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<PriceTermDtos>> GetDivision(string Code)
        {
            var division = priceTermQueries.GetPriceTerm(Code);

            if (division != null)
                return Ok(division);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreatePriceTerm([FromBody] CreatePriceTermCommand command)
        {
            var commandResult = new CommonCommandResult<PriceTerm>();
            logger.LogInformation("{@time} - Sending create PriceTerm command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> UpdatePriceTerm([FromBody] UpdatePriceTermCommand command)
        {
            var commandResult = new CommonCommandResult<PriceTerm>();
            logger.LogInformation("{@time} - Sending update PriceTerm command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> DeletePriceTerm([FromBody] DeletePriceTermCommand command)
        {
            var commandResult = new CommonCommandResult<PriceTerm>();
            logger.LogInformation("{@time} - Sending delete PriceTerm command with request {@request}",
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
