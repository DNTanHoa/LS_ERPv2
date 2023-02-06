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
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class CreateItemMasterJob
    {
        private readonly ILogger<CreateItemMasterJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public CreateItemMasterJob(ILogger<CreateItemMasterJob> logger,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
            this.logger = logger;
        }

        [JobDisplayName("Create Item Master Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute()
        {
            try
            {
                var customers =context.StitchingThread.Select(x => x.CustomerID).Distinct().ToList();
                
                var connectionString = configuration.GetSection("ConnectionString")
                            .GetSection("DbConnection").Value ?? string.Empty;

                foreach(var customer in customers)  
                {
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@UserName","Hangfire"),
                        new SqlParameter("@CustomerID",customer)
                    };

                    DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_CreateItemMaster", parameters);
                  if (table.Rows.Count > 0)
                    {
                        if (int.Parse(table.Rows[0][0].ToString()) == 0)
                        {
                            logger.LogError("{@time} - Create item master job failed",
                                DateTime.Now);
                            LogHelper.Instance.Error("{@time} - Create item master job failed",
                                DateTime.Now);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Create item master job has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Create item master job has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
