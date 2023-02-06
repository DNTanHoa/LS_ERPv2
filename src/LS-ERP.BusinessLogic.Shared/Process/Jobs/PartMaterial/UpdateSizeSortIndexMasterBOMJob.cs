using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdateSizeSortIndexMasterBOMJob
    {
        private readonly ILogger<UpdateSizeSortIndexMasterBOMJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public UpdateSizeSortIndexMasterBOMJob(ILogger<UpdateSizeSortIndexMasterBOMJob> logger,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            this.configuration = configuration;
        }

        [JobDisplayName("Update Size Sort Index for BOM & Forcast Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute()
        {
            try
            {
                var connectionString = configuration.GetSection("ConnectionString")
                            .GetSection("DbConnection").Value ?? string.Empty;
                
                DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_UpdateSizeSortIndex_BOM_Forcast");
                if(table.Rows.Count > 0)
                {
                    if(int.Parse(table.Rows[0][0].ToString()) == 0)
                    {
                        logger.LogError("{@time} - Update size sort index master bom job failed",
                            DateTime.Now);
                        LogHelper.Instance.Error("{@time} - Update size sort index master bom job failed",
                            DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Update size sort index master bom job has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Update  size sort index master bom job has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
