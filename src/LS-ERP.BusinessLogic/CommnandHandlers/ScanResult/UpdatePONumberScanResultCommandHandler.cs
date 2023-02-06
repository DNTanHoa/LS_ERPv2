using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class UpdatePONumberScanResultCommandHandler : IRequestHandler<UpdatePONumberScanResultCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public UpdatePONumberScanResultCommandHandler(SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public Task<CommonCommandResult> Handle(UpdatePONumberScanResultCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

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

                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
