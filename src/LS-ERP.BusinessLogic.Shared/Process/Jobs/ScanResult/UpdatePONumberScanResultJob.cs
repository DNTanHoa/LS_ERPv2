using Hangfire;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdatePONumberScanResultJob
    {
        private readonly ILogger<UpdatePONumberScanResultJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public UpdatePONumberScanResultJob(ILogger<UpdatePONumberScanResultJob> logger,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
        }

        [JobDisplayName("Update PO Number Scan Result Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute()
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

                foreach(var preiod in periods)
                {
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@UserName","Hangfire"),
                        new SqlParameter("@PeriodID",preiod.ID)
                    };

                    SqlHelper.ExecuteNonQuery(connectionString, "sp_CreateUpdateFinishGoodInventory", parameters);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Update PO number scan result job has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Update  PO number scan result job has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
