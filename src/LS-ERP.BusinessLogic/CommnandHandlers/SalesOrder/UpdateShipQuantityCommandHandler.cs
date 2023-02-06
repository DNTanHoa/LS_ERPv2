using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class UpdateShipQuantityCommandHandler : IRequestHandler<UpdateShipQuantityCommand, UpdateShipQuantityResult>
    {
        private readonly ILogger<UpdateShipQuantityCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IMediator mediator;

        public UpdateShipQuantityCommandHandler(ILogger<UpdateShipQuantityCommandHandler> logger,
            SqlServerAppDbContext context,
            ISalesOrderRepository salesOrderRepository,
            IItemStyleRepository itemStyleRepository,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.salesOrderRepository = salesOrderRepository;
            this.itemStyleRepository = itemStyleRepository;
            this.mediator = mediator;
        }

        public async Task<UpdateShipQuantityResult> Handle(UpdateShipQuantityCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Execute update ship quantity for sales order command", DateTime.Now.ToString());
            var result = new UpdateShipQuantityResult();

            if (!FileHelpers.SaveFile(request.UpdateFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var salesOrders = salesOrderRepository
                .GetSalesOrders(request.CustomerID);

            var updateResult = UpdateShipQuantityProcess
                .UpdateMultiSalesOrder(salesOrders,
                     request.UserName, fullPath,
                     out List<OrderDetail> orderDetails, out List<ItemStyleBarCode> itemStyleBarcodes,
                     out string errorMessage);

            if (!updateResult)
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                context.OrderDetail.UpdateRange(orderDetails);
                //context.ItemStyleBarCode.UpdateRange(itemStyleBarcodes);
                context.SaveChanges();
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                LogHelper.Instance.Error("{@time} - Execute update ship quantity for sales order command with message {@message}",
                    DateTime.Now.ToString(), ex.InnerException?.Message);
            }
           
            return result;
        }

    }
}
