using Hangfire;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdateFabricPurchaseOrderFromTransactionsJob
    {
        private readonly ILogger<UpdateFabricPurchaseOrderFromTransactionsJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public UpdateFabricPurchaseOrderFromTransactionsJob(ILogger<UpdateFabricPurchaseOrderFromTransactionsJob> logger,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
        }

        [JobDisplayName("Update Fabric Purchase Order Quantity Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute()
        {
            try
            {
                var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
                SqlHelper.ExecuteNonQuery(connectionString, "sp_UpdateFabricPurchaseOrderFromTransactions", null);
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Update Fabric Purchase Order Quantity job has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Update Fabric Purchase Order Quantity job has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
