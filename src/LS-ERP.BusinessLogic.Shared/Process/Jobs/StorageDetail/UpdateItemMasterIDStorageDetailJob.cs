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
    public class UpdateItemMasterIDStorageDetailJob
    {
        private readonly ILogger<UpdateItemMasterIDStorageDetailJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public UpdateItemMasterIDStorageDetailJob(ILogger<UpdateItemMasterIDStorageDetailJob> logger,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
        }

        [JobDisplayName("Update Item Master ID Storage Detail Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute()
        {
            try
            {
                var connectionString = configuration.GetSection("ConnectionString")
                    .GetSection("DbConnection").Value ?? string.Empty;

                SqlHelper.ExecuteNonQuery(connectionString, "sp_UpdateItemMasterIDStorageDetailFromPartMaster");
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Update item master id storage detail job has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Update item master id storage detail job has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
