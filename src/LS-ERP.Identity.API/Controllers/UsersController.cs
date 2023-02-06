using LS_ERP.Identity.API.Model;
using LS_ERP.Identity.BusinessLogic.Command;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;

        public UsersController(ILogger<UsersController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommonResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CommonResponseModel>> Login([FromBody]LoginCommand command)
        {
            return Ok();
        }
    }
}
