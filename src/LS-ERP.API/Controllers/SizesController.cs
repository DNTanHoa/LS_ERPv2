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
    public class SizesController : ControllerBase
    {
        private readonly ILogger<SizesController> logger;
        private readonly ISizeQueries SizeQueries;
        private readonly IMediator mediator;

        public SizesController(ILogger<SizesController> logger,
            ISizeQueries SizeQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.SizeQueries = SizeQueries;
            this.mediator = mediator;
        }
        [HttpGet]
        [Route("sizes_with_customerid")]
        public async Task<ActionResult<CommonResponseModel>> GetByCustomerId(
        [FromQuery] string customerId)
        {
            var response = new CommonResponseModel();
            var data = SizeQueries.GetByCustomerId(customerId);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }
        [HttpGet]
        [Route("sizes_with_lsstyle")]
        public async Task<ActionResult<CommonResponseModel>> GetByLSStyle(
        [FromQuery] string lsStyle)
        {
            var response = new CommonResponseModel();
            var data = SizeQueries.GetByLSStyle(lsStyle);
            return Ok(response.SetData(data).SetResult("000", string.Empty));
        }


    }
}
