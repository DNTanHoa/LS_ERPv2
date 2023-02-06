using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Queries.SalesOrder;
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

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesContractsController : ControllerBase
    {
        private readonly ILogger<SalesContractsController> logger;
        private readonly IMediator mediator;

        public SalesContractsController(ILogger<SalesContractsController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public async Task<ActionResult<CommonResponseModel>> Import([FromForm] ImportSalesContractCommand command)
        {
            var commandResult = new ImportSalesContractResult();
            logger.LogInformation("{@time} - Sending import sales contract command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending import sales contract command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Import successfully")
                    .SetData(commandResult.Result);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPut]
        [Route("update")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public async Task<ActionResult<CommonResponseModel>> Update([FromForm] UpdateSalesContractCommand command)
        {
            var commandResult = new ImportSalesContractResult();
            logger.LogInformation("{@time} - Sending update sales contract command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update sales contract command with request {@request}",
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

        [HttpPut]
        [Route("update-purchase-order-number")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public async Task<ActionResult<CommonResponseModel>> UpdatePurchaseOrderNumber([FromForm] UpdatePurchaseOrderNumberSalesContractCommand command)
        {
            var commandResult = new ImportSalesContractResult();
            logger.LogInformation("{@time} - Sending update purchase order number sales contract command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update purchase order number  sales contract command with request {@request}",
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

        [HttpPut]
        [Route("update-contract-info")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public async Task<ActionResult<CommonResponseModel>> UpdateContractInfo([FromForm] UpdateContractInfoCommand command)
        {
            var commandResult = new ImportSalesContractResult();
            logger.LogInformation("{@time} - Sending update contract info command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update contract info command with request {@request}",
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

        [HttpDelete]
        [Route("{ID}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> Delete(string ID)
        {
            var command = new DeleteSalesContractCommand()
            {
                ID = ID
            };

            var commandResult = new DeleteSalesContractResult();
            logger.LogInformation("{@time} - Sending delete sales contract command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete sales contract command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", "Delete successfully");
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }
    }
}
