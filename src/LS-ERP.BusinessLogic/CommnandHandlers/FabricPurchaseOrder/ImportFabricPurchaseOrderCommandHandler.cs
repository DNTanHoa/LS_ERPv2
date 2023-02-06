using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
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
    public class ImportFabricPurchaseOrderCommandHandler
        : IRequestHandler<ImportFabricPurchaseOrderCommand, ImportFabricPurchaseOrderResult>
    {
        private readonly ILogger<ImportFabricPurchaseOrderCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricPurchaseOrderRepository fabricPurchaseOrderRepository;

        public ImportFabricPurchaseOrderCommandHandler(ILogger<ImportFabricPurchaseOrderCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator,
            IFabricPurchaseOrderRepository fabricPurchaseOrderRepository)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
            this.fabricPurchaseOrderRepository = fabricPurchaseOrderRepository;
        }

        public async Task<ImportFabricPurchaseOrderResult> Handle(ImportFabricPurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import fabric purchase order command", DateTime.Now.ToString());
            var result = new ImportFabricPurchaseOrderResult();
            //string fileName = request.File.FileName;

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var oldListFabricPurchase = fabricPurchaseOrderRepository.GetFabricPurchaseOrders(request.CustomerID);

            var listFabricPurchaseOrder = ImportFabricPurchaseOrderProcessor.Import(fullPath, subPath, request.File.FileName
                , request.UserName, request.CustomerID, request.ProductionMethodCode, oldListFabricPurchase.ToList(),
                out List<FabricPurchaseOrder> updateFabricPurchaseOrder,
                out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                if (updateFabricPurchaseOrder != null && updateFabricPurchaseOrder.Count > 0)
                {
                    context.FabricPurchaseOrder.UpdateRange(updateFabricPurchaseOrder);
                }

                context.FabricPurchaseOrder.AddRange(listFabricPurchaseOrder);

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;

                result.Result = listFabricPurchaseOrder;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
