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
    public class WorkingTimesController : ControllerBase
    {
        private readonly ILogger<WorkingTimesController> logger;
        private readonly IWorkingTimeQueries workingTimeQueries;
        private readonly IMediator mediator;

        public WorkingTimesController(ILogger<WorkingTimesController> logger,
            IWorkingTimeQueries workingTimeQueries,
            IMediator mediator)
        {
            this.logger = logger;
            this.workingTimeQueries = workingTimeQueries;
            this.mediator = mediator;
        }
        [HttpGet]
        [Route("working-times")]
        public async Task<ActionResult<CommonResponseModel>> Get()
        {
            var response = new CommonResponseModel();
            var data = workingTimeQueries.GetAll();
            return Ok(response.SetData(data).SetResult("000", string.Empty));       
        }
    }
}
