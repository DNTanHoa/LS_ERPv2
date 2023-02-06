using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Dtos.Customer;
using LS_ERP.BusinessLogic.Queries.Customer;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> logger;
        private readonly ICustomerQueries customerQueries;
        private readonly IMediator mediator;

        public CustomersController(ILogger<CustomersController> logger,
            ICustomerQueries customerQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.customerQueries = customerQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<CustomerSummaryDtos>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<CustomerSummaryDtos>>), 
            (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<CustomerSummaryDtos>> GetCustomers()
        {
            var data = customerQueries.GetCustomerSummary();
            return Ok(data);          
        }

        [Route("{Id}")]
        [HttpGet]
        [ProducesResponseType(typeof(CustomerDetailDtos), (int)HttpStatusCode.OK)]
        public ActionResult<CustomerDetailDtos> GetCustomer(string Id)
        {
            var data = customerQueries.GetCustomerByID(Id);
            if(data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreateDivision([FromBody]CreateCustomerCommand command)
        {
            var commandResult = new CommonCommandResult<Customer>();
            logger.LogInformation("{@time} - Sending create customer command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> UpdateDivision([FromBody]UpdateCustomerCommand command)
        {
            var commandResult = new CommonCommandResult<Customer>();
            logger.LogInformation("{@time} - Sending update customer command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> DeleteDivision([FromBody]DeleteCustomerCommand command)
        {
            var commandResult = new CommonCommandResult<Customer>();
            logger.LogInformation("{@time} - Sending delete customer command with request {@request}",
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
