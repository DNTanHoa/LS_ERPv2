using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Dtos.PaymentTerm;
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
    public class PaymentTermsController : ControllerBase
    {
        private readonly ILogger<PaymentTermsController> logger;
        private readonly IPaymentTermQueries paymentTermQueries;
        private readonly IMediator mediator;

        public PaymentTermsController(ILogger<PaymentTermsController> logger,
            IPaymentTermQueries paymentTermQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.paymentTermQueries = paymentTermQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentTermDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<PaymentTermDtos>> GetPaymentTerms()
        {
            return Ok(paymentTermQueries.GetPaymentTerms());
        }

        [HttpGet]
        [Route("{Code}")]
        [ProducesResponseType(typeof(PaymentTermDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<PaymentTermDtos>> GetPaymentTerm(string Code)
        {
            var division = paymentTermQueries.GetPaymentTerm(Code);

            if (division != null)
                return Ok(division);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreatePaymentTerm([FromBody] CreatePaymentTermCommand command)
        {
            var commandResult = new CommonCommandResult<PaymentTerm>();
            logger.LogInformation("{@time} - Sending create PaymentTerm command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> UpdatePaymentTerm([FromBody] UpdatePaymentTermCommand command)
        {
            var commandResult = new CommonCommandResult<PaymentTerm>();
            logger.LogInformation("{@time} - Sending update PaymentTerm command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> DeletePaymentTerm([FromBody] DeletePaymentTermCommand command)
        {
            var commandResult = new CommonCommandResult<PaymentTerm>();
            logger.LogInformation("{@time} - Sending delete PaymentTerm command with request {@request}",
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

