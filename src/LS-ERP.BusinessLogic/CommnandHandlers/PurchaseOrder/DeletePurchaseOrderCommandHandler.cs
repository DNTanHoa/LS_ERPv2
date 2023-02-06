using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
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

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeletePurchaseOrderCommandHandler
        : IRequestHandler<DeletePurchaseOrderCommand, DeletePurchaseOrderResult>
    {
        private readonly ILogger<DeletePurchaseOrderCommandHandler> logger;
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly IProductionBOMRepository productionBOMRepository;
        private readonly PurchaseOrderValidator purchaseOrderValidator;
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;

        public DeletePurchaseOrderCommandHandler(ILogger<DeletePurchaseOrderCommandHandler> logger,
            IPurchaseOrderRepository purchaseOrderRepository,
            IProductionBOMRepository productionBOMRepository,
            PurchaseOrderValidator purchaseOrderValidator,
            IConfiguration configuration,
            AppDbContext context)
        {
            this.logger = logger;
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.productionBOMRepository = productionBOMRepository;
            this.purchaseOrderValidator = purchaseOrderValidator;
            this.configuration = configuration;
            this.context = context;
        }

        public async Task<DeletePurchaseOrderResult> Handle(DeletePurchaseOrderCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new DeletePurchaseOrderResult();

            logger.LogInformation("{@time} - Exceute delete purchase order command",
                DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute delete purchase order command",
                DateTime.Now.ToString());

            var purchaseOrder = purchaseOrderRepository.GetPurchaseOrder(request.PurchaseOrderID);

            if (purchaseOrder == null)
            {
                result.Message = "Can't find purchase order to delete";
                return result;
            }

            if(!purchaseOrderValidator.CanDelete(purchaseOrder, out string errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                /// Lấy thông tin purchase order để cập nhật thông tin
                var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
                var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@PurchaseOrderID", request.PurchaseOrderID),
                };
                var resultSQL = SqlHelper.ExecuteNonQuery(connectionString,
                    "sp_DeletePurchaseOrder", sqlParameter);

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.InnerException?.Message;
                LogHelper.Instance.Error(
                    "{@time} - Exceute update purchase order command error with message {@message}",
                    DateTime.Now.ToString(), ex.InnerException?.Message);
            }

            return result;
        }
    }
}
