using Common.Model;
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

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly ILogger<VendorsController> logger;
        private readonly IMediator mediator;
        private readonly IVendorQueries vendorQueries;

        public VendorsController(ILogger<VendorsController> logger,
            IMediator mediator,
            IVendorQueries vendorQueries)
        {
            this.logger = logger;
            this.mediator = mediator;
            this.vendorQueries = vendorQueries;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VendorDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<VendorDtos>> GetVendors()
        {
            return Ok(vendorQueries.GetVendors());
        }

        [HttpGet]
        [Route("{ID}")]
        [ProducesResponseType(typeof(VendorDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<VendorDtos>> GetVendor(string ID)
        {
            var Vendor = vendorQueries.GetVendor(ID);

            if (Vendor != null)
                return Ok(Vendor);

            return NotFound();
        }
        [HttpGet]
        [Route("with_description")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<VendorDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<VendorDtos>>>> GetByDescription(
         [FromQuery] string description)
        {
            var data = vendorQueries.GetByDescription(description);
            return new CommonResponseModel<IEnumerable<VendorDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }
        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreateCurrency([FromBody] CreateVendorCommand command)
        {
            var commandResult = new CommonCommandResult<Vendor>();
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
        public async Task<ActionResult<CommonResponseModel>> UpdateCurrency([FromBody] UpdateVendorCommand command)
        {
            var commandResult = new CommonCommandResult<Vendor>();
            logger.LogInformation("{@time} - Sending update Vendor command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update Vendor command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> DeleteCurrency([FromBody] DeleteVendorCommand command)
        {
            var commandResult = new CommonCommandResult<Vendor>();
            logger.LogInformation("{@time} - Sending delete Vendor command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete Vendor command with request {@request}",
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
