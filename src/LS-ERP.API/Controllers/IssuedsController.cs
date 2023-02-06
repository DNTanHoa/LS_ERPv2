using AutoMapper;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using LS_ERP.XAF.Module.Service;
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
    public class IssuedsController : ControllerBase
    {
        private readonly ILogger<IssuedsController> logger;
        private readonly IReportQueries reportQueries;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public IssuedsController(ILogger<IssuedsController> logger,
            IReportQueries reportQueries,
            IMapper mapper,
            IMediator mediator)
        {
            this.logger = logger;
            this.reportQueries = reportQueries;
            this.mapper = mapper;
            this.mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CommonResponseModel> CreateIssued([FromBody] CreateIssuedCommand command)
        {
            var commandResult = new CommonCommandResult<Issued>();
            logger.LogInformation("{@time} - Sending create issued command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create issued command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Create successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("create-fabric")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CommonResponseModel> CreateIssuedFabric([FromBody] CreateIssuedFabricCommand command)
        {
            var commandResult = new CommonCommandResult<Issued>();
            logger.LogInformation("{@time} - Sending create issued fabric command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create issued fabric command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Create successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CommonResponseModel> UpdateIssued([FromBody] UpdateIssuedCommand command)
        {
            var commandResult = new CommonCommandResult<Issued>();
            logger.LogInformation("{@time} - Sending update issued command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update issued command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Update successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("get-supplier")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> Import(GetIssuedSupplierInfoCommand command)
        {
            var commandResult = new GetIssuedSupplierInfoResult();
            logger.LogInformation("{@time} - Sending get supplier issued command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending get supplier issued command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpDelete]
        [Route("{Number}")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CommonResponseModel> DeleteIssued(string Number)
        {
            var commandResult = new CommonCommandResult<object>();
            var command = new DeleteIssuedCommand()
            {
                IssuedNumber = Number
            };
            logger.LogInformation("{@time} - Sending update issued command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update issued command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Update successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("report")]
        [ProducesResponseType(typeof(CommonResponseModel), 200)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CommonResponseModel> GetIssued([FromQuery] string customerID,
            [FromQuery] string storageCode, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var response = new CommonResponseModel();
            var data = reportQueries.GetIssuedReports(customerID, storageCode, fromDate, toDate);
            return response.SetResult("000", string.Empty)
                .SetData(data);
        }
    }
}
