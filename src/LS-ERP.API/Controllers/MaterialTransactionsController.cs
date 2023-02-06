using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LS_ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialTransactionsController : ControllerBase
    {
        private readonly ILogger<MaterialTransactionsController> logger;
        private readonly IMaterialTransactionQueries materialTransactionQueries;

        public MaterialTransactionsController(ILogger<MaterialTransactionsController> logger,
            IMaterialTransactionQueries materialTransactionQueries)
        {
            this.logger = logger;
            this.materialTransactionQueries = materialTransactionQueries;
        }

        [HttpGet]
        [Route("storage/{storageCode}/{fromDate}/{toDate}")]
        [ProducesResponseType(typeof(IEnumerable<MaterialTransactionSummaryDto>), (int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<MaterialTransactionSummaryDto>> GetSummary(string storageCode,
            DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate == null)
                fromDate = DateTime.Now;
            if (toDate == null)
                toDate = DateTime.Now;

            var data = materialTransactionQueries
                .GetSummaryDtos(storageCode, (DateTime)fromDate, (DateTime)toDate);

            return Ok(data);
        }
    }
}
