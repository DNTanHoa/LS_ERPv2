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
using Ultils.Extensions;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartPricesController : ControllerBase
    {
        private readonly ILogger<PartPricesController> logger;
        private readonly IPartPriceQueries partPriceQueries;
        private readonly IMediator mediator;

        public PartPricesController(ILogger<PartPricesController> logger,
            IPartPriceQueries partPriceQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.partPriceQueries = partPriceQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<PartPriceDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<PartPriceDtos>>>> Get(
         [FromQuery] string companyId, [FromQuery] string StyleNO)
        {
            var data = partPriceQueries.Get(companyId, StyleNO);
            return new CommonResponseModel<IEnumerable<PartPriceDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("{Id}")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<PartPriceDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<PartPriceDtos>>>> GetById(
         int Id)
        {
            var data = partPriceQueries.GetById(Id);
            return new CommonResponseModel<IEnumerable<PartPriceDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("style_no")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<PartPriceDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<PartPriceDtos>>>> GetWithDailyTarget(
         [FromQuery] string companyId, [FromQuery] string StyleNO)
        {
            var data = partPriceQueries.GetWithDailyTarget(companyId, StyleNO);
            return new CommonResponseModel<IEnumerable<PartPriceDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpGet]
        [Route("with_item")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<PartPriceDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<PartPriceDtos>>>> GetWithDailyTarget(
         [FromQuery] string companyId, [FromQuery] string StyleNO, string Item)
        {
            var data = partPriceQueries.GetWithDailyTarget(companyId, StyleNO, Item);
            return new CommonResponseModel<IEnumerable<PartPriceDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel<PartPrice>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<PartPrice>>> CreatePartPrice(
            [FromBody] CreatePartPriceCommand command)
        {
            var commandResult = new CommonCommandResultHasData<PartPrice>();
            logger.LogInformation("{@time} - Sending create part price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create part price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<PartPrice>();

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

        [HttpPut]
        [ProducesResponseType(typeof(CommonResponseModel<PartPrice>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<PartPrice>>> UpdatePartPrice(
            [FromBody] UpdatePartPriceCommand command)
        {
            var commandResult = new CommonCommandResultHasData<PartPrice>();
            logger.LogInformation("{@time} - Sending update part price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update part price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<PartPrice>();

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

        [HttpDelete]
        [Route("{Id:int}")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<object>>> DeletePartPrice(
            int Id)
        {
            var command = new DeletePartPriceCommand()
            {
                ID = Id
            };
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Sending delete part price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete part price command with request {@request}",
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
        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<PartPrice>>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<PartPrice>>>> Bulk(
            [FromBody] BulkPartPriceCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<PartPrice>>();
            logger.LogInformation("{@time} - Sending create part price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create part price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            command.UserName = HttpContext.GetClaimByKey("UserName");
            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel<IEnumerable<PartPrice>>();

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

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> ImportPartPrice(
            [FromForm] ImportPartPriceCommand command)
        {
            var commandResult = new ImportPartPriceResult();
            logger.LogInformation("{@time} - Sending import part price command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending import part price command with request {@request}",
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
    }
}
