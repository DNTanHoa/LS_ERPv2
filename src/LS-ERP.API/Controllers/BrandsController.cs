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
    public class BrandsController : ControllerBase
    {
        private readonly ILogger<BrandsController> logger;
        private readonly IBrandQueries brandQueries;
        private readonly IMediator mediator;

        public BrandsController(ILogger<BrandsController> logger,
            IBrandQueries brandQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.brandQueries = brandQueries;
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrandDtos>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<BrandDtos>> GetBrands()
        {
            return Ok(brandQueries.GetBrands());
        }

        [HttpGet]
        [Route("{ID}")]
        [ProducesResponseType(typeof(BrandDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<BrandDtos>> GetBrand(string ID)
        {
            var brand = brandQueries.GetBrand(ID);

            if (brand != null)
                return Ok(brand);

            return NotFound();
        }

        [HttpGet]
        [Route("{CustomerID}/Brand")]
        [ProducesResponseType(typeof(BrandDtos), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<BrandDtos>> GetBrandByCustomer(string CustomerID)
        {
            var brands = brandQueries.GetBrandsByCustomer(CustomerID);

            if (brands != null)
                return Ok(brands);

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommonResponseModel>> CreateCurrency([FromBody]CreateBrandCommand command)
        {
            var commandResult = new CommonCommandResult<Brand>();
            logger.LogInformation("{@time} - Sending create Brand command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending create Brand command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> UpdateCurrency([FromBody] UpdateBrandCommand command)
        {
            var commandResult = new CommonCommandResult<Brand>();
            logger.LogInformation("{@time} - Sending update brand command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending update brand command with request {@request}",
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
        public async Task<ActionResult<CommonResponseModel>> DeleteCurrency([FromBody]DeleteBrandCommand command)
        {
            var commandResult = new CommonCommandResult<Brand>();
            logger.LogInformation("{@time} - Sending delete Brand command with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(command));
            LogHelper.Instance.Information("{@time} - Sending delete Brand command with request {@request}",
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
