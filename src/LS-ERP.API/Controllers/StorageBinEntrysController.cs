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

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageBinEntrysController : ControllerBase
    {
        private readonly ILogger<StorageBinEntrysController> logger;
        private readonly IStorageBinEntryQueries StorageBinEntryQueries;
        private readonly IMediator mediator;

        public StorageBinEntrysController(ILogger<StorageBinEntrysController> logger,
            IStorageBinEntryQueries StorageBinEntryQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.StorageBinEntryQueries = StorageBinEntryQueries;
            this.mediator = mediator;
        }
        [HttpGet]
        [Route("with_storage_code")]
        [ProducesResponseType(typeof(CommonResponseModel<IEnumerable<StorageBinEntryDtos>>),
        (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<IEnumerable<StorageBinEntryDtos>>>> GetByCompanyStorageID(
         [FromQuery] string storageCode)
        {
            var data = StorageBinEntryQueries.GetStorageBinEntry(storageCode);
            return new CommonResponseModel<IEnumerable<StorageBinEntryDtos>>()
                .SetResult(true, string.Empty)
                .SetData(data);
        }

        [HttpPost]
        [Route("import")]
        [ProducesResponseType(typeof(CommonResponseModel<List<ImportStorageBinEntryDto>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<ImportStorageBinEntryDto>>>> Import(
            [FromForm] ImportStorageBinEntryCommand command)
        {
            var response = new CommonResponseModel<List<ImportStorageBinEntryDto>>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message)
                .SetData(commandResult.Data));
        }

        [HttpPost]
        [Route("bulk")]
        [ProducesResponseType(typeof(CommonResponseModel<object>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel<List<ImportStorageBinEntryDto>>>> Bulk(
            [FromBody] BulkStorageBinEntryCommand command)
        {
            var response = new CommonResponseModel<object>();
            var commandResult = await mediator.Send(command);
            return Ok(response
                .SetResult(commandResult.Success, commandResult.Message));
        }
    }
}
