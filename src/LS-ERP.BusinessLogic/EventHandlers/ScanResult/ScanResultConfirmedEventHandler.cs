using LS_ERP.BusinessLogic.Events;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.EventHandlers
{
    public class ScanResultConfirmedEventHandler : INotificationHandler<ScanResultConfirmedEvent>
    {
        private readonly ILogger<ScanResultConfirmedEventHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public ScanResultConfirmedEventHandler(ILogger<ScanResultConfirmedEventHandler> logger,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
        }
        public Task Handle(ScanResultConfirmedEvent notification, 
            CancellationToken cancellationToken)
        {
            try
            {
                var connectionString = configuration.GetSection("ConnectionString")
                    .GetSection("DbConnection").Value ?? string.Empty;

                /// Update PO scan result
                SqlHelper.ExecuteNonQuery(connectionString, "sp_UpdatePONumberScanResult");

                /// Create/Update On Hand Quantity FG
                var periods = context.InventoryPeriod
                    .Where(x => x.IsClosed != true).OrderBy(x => x.FromDate).ToList();

                foreach (var preiod in periods)
                {
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@UserName","Hangfire"),
                        new SqlParameter("@PeriodID",preiod.ID)
                    };

                    SqlHelper.ExecuteNonQuery(connectionString, "sp_CreateUpdateFinishGoodInventory", parameters);
                }
            }
            catch(Exception ex)
            {
                logger.LogError("Scan result confirm has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Scan result confirm has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;
        }
    }
}
