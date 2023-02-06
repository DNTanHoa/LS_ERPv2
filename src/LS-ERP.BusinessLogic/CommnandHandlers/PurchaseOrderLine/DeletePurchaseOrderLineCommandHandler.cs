using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeletePurchaseOrderLineCommandHandler
        : IRequestHandler<DeletePurchaseOrderLineCommand, DeletePurchaseOrderLineResult>
    {
        private readonly ILogger<DeletePurchaseOrderLineCommandHandler> logger;
        private readonly IProductionBOMRepository productionBOMRepository;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public DeletePurchaseOrderLineCommandHandler(
            ILogger<DeletePurchaseOrderLineCommandHandler> logger,
            IProductionBOMRepository productionBOMRepository,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.productionBOMRepository = productionBOMRepository;
            this.context = context;
            this.configuration = configuration;
        }
        public async Task<DeletePurchaseOrderLineResult> Handle(
            DeletePurchaseOrderLineCommand request, CancellationToken cancellationToken)
        {
            var result = new DeletePurchaseOrderLineResult();

            logger.LogInformation("{@time} - Exceute delete purchase order line command",
                DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute delete purchase order line command",
                DateTime.Now.ToString());
            
            try
            {
                /// Lấy thông tin purchase order để cập nhật thông tin
                var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
                var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@PurchaseOrderLineIDs", string.Join(",",
                        request.PurchaseOrderLineIDs.Select(x => x.ToString()))),
                };
                var resultSQL = SqlHelper.ExecuteNonQuery(connectionString,
                    "sp_DeletePurchaseOrderLine", sqlParameter);

                result.IsSuccess = true;
            }
            catch(DbUpdateException ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
