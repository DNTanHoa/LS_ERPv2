using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class BalanceQuantityForecastOverallHandler
        : IRequestHandler<BalanceQuantityForecastOverallCommand, BalanceQuantityForecastOverallResult>
    {
        public BalanceQuantityForecastOverallHandler(ILogger<BalanceQuantityForecastOverallCommand> logger,
            //IReservaionEntryForecast
            SqlServerAppDbContext context)
        {

        }
        public Task<BalanceQuantityForecastOverallResult> Handle(BalanceQuantityForecastOverallCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
