using Common.Model;
using LS_ERP.API.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Ressult;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemStylesController : ControllerBase
    {
        private readonly ILogger<ItemStylesController> logger;
        private readonly IItemStyleQueries itemStyleQueries;
        private readonly IMediator mediator;

        public ItemStylesController(ILogger<ItemStylesController> logger,
            IItemStyleQueries itemStyleQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.itemStyleQueries = itemStyleQueries;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("pull-bom")]
        public async Task<ActionResult<CommonResponseModel>> PullBom(
            [FromBody] PullBOMItemStyleCommand command)
        {
            var commandResult = new PullBOMItemStyleResult();

            logger.LogInformation("{@time} - Pull bom for styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Pull bom for styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("pull-bom-type")]
        public async Task<ActionResult<CommonResponseModel>> PullBomType(
            [FromBody] PullBomTypeItemStyleCommand command)
        {
            var commandResult = new PullBomTypeItemStyleResult();

            logger.LogInformation("{@time} - Pull bom type for styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Pull bom type for styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("clearing-bom")]
        public async Task<ActionResult<CommonResponseModel>> ClearingBOM(
            [FromBody] ClearingBOMCommand command)
        {
            var commandResult = new ClearingBOMResult();
            logger.LogInformation("{@time} - Clearing Bom for styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("clearing-style")]
        public async Task<ActionResult<CommonResponseModel>> ClearingBOM(
            [FromBody] ClearingStyleCommand command)
        {
            var commandResult = new ClearingStyleResult();
            logger.LogInformation("{@time} - Clearing styles for styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("calculate-required-quantity")]
        public async Task<ActionResult<CommonResponseModel>> CalculateRequiredQuantity(
            [FromBody] CalculateRequiredQuantityCommand command)
        {
            var commandResult = new CalculateRequiredQuantityResult();
            logger.LogInformation("{@time} - Calculated required quantity for styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("bulk-cancel")]
        public async Task<ActionResult<CommonResponseModel>> BulkCancel(
            [FromBody] CancelItemStyleCommand command)
        {
            var commandResult = new CancelItemStyleResult();
            logger.LogInformation("{@time} - Cancel styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Cancel styles command with request with request {@request}",
               DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.IsSuccess)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("bulk-issue")]
        public async Task<ActionResult<CommonResponseModel>> BulkCancel(
            [FromBody] IssueItemStyleCommand command)
        {
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Issued styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Issued styles command with request with request {@request}",
               DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.Success)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("bulk-update-infor")]
        public async Task<ActionResult<CommonResponseModel>> BulkUpdateInfor(
            [FromBody] BulkUpdateItemStyleInforCommand command)
        {
            var commandResult = new CommonCommandResult();
            logger.LogInformation("{@time} - Bulk update styles command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Bulk update styles command with request with request {@request}",
               DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.Success)
            {
                response.SetResult("000", commandResult.Message);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("import-update-infor")]
        public async Task<ActionResult<CommonResponseModel>> ImportUpdateInfor(
            [FromForm] UpdateSaleOrdersInformationCommand command)
        {
            var commandResult = new CommonCommandResultHasData<IEnumerable<ItemStyleInforDtos>>();
            logger.LogInformation("{@time} - Import update infor command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Import update infor command with request with request {@request}",
               DateTime.Now.ToString(), JsonConvert.SerializeObject(command));

            commandResult = await mediator.Send(command);
            var response = new CommonResponseModel();

            if (commandResult.Success)
            {
                response.SetResult("000", commandResult.Message)
                    .SetData(commandResult.Data);
            }
            else
            {
                response.SetResult("100", commandResult.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("style-materials")]
        public async Task<ActionResult<CommonResponseModel>> GetStyleMaterial(
            [FromQuery]string styles)
        {
            var response = new CommonResponseModel();
            var data = itemStyleQueries.GetMaterials(styles);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        
        
        [HttpGet]
        [Route("customer-styles")]
        public async Task<ActionResult<CommonResponseModel>> GetCustomerStyle(
            [FromQuery] string customerId)
        {
            var response = new CommonResponseModel();
            var data = itemStyleQueries.GetCustomerStyles(customerId);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        [HttpGet]
        [Route("ls-styles")]
        public async Task<ActionResult<CommonResponseModel>> GetLSStyle(
            [FromQuery] string customerStyle)
        {
            var response = new CommonResponseModel();
            var data = itemStyleQueries.GetLSStyles(customerStyle);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        [HttpGet]
        [Route("all-ls-styles")]
        public async Task<ActionResult<CommonResponseModel>> GetAllLSStyle(
        )
        {
            var response = new CommonResponseModel();
            var data = itemStyleQueries.GetAllLSStyles();
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }

        [HttpGet]
        [Route("all-po-numbers")]
        public async Task<ActionResult<CommonResponseModel>> GetAllPONumber(
            [FromQuery] string customerId)
        {
            var response = new CommonResponseModel();
            var data = itemStyleQueries.GetAllPONumbers(customerId);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
    }
}
