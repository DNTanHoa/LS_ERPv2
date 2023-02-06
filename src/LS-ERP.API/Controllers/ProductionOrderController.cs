using LS_ERP.BusinessLogic.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionOrderController : ControllerBase
    {
        private readonly ILogger<SizesController> logger;
        private readonly IMediator mediator;

        public ProductionOrderController(ILogger<SizesController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }
    }
}
