using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class UpdateStorageDetailJob
    {
        private readonly ILogger<CreatePartPriceJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public UpdateStorageDetailJob(ILogger<CreatePartPriceJob> logger,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
        }

        [JobDisplayName("Update Quantity Storage Detail Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute()
        {
            try
            {
                var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
                SqlHelper.ExecuteNonQuery(connectionString, "sp_UpdateStorageDetailFromTransactions", null);
            }
            catch(Exception ex)
            {
                logger.LogError("{@time} - Update LS style storage detail job has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Update LS style storage detail job has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
