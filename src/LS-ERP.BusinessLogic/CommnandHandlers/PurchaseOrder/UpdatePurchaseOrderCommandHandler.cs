using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class UpdatePurchaseOrderCommandHandler
        : IRequestHandler<UpdatePurchaseOrderCommand, CommonCommandResult<PurchaseOrder>>
    {
        private readonly ILogger<UpdatePurchaseOrderCommandHandler> logger;
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly IProductionBOMRepository productionBOMRepository;
        private readonly PurchaseOrderValidator validator;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public UpdatePurchaseOrderCommandHandler(ILogger<UpdatePurchaseOrderCommandHandler> logger,
            IPurchaseOrderRepository purchaseOrderRepository,
            IProductionBOMRepository productionBOMRepository,
            PurchaseOrderValidator validator,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.productionBOMRepository = productionBOMRepository;
            this.validator = validator;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CommonCommandResult<PurchaseOrder>> Handle(UpdatePurchaseOrderCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<PurchaseOrder>();

            logger.LogInformation("{@time} - Exceute update purchase order command",
                DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update purchase order command",
                DateTime.Now.ToString());

            if (!purchaseOrderRepository.IsExist(request.ID, out PurchaseOrder purchaseOrder))
            {
                result.Message = "Can't not find purchase order";
                return result;
            }

            purchaseOrder = purchaseOrderRepository.GetPurchaseOrder(request.ID);
            
            mapper.Map(request, purchaseOrder);
            
            if (!validator.IsValid(purchaseOrder, out string validateMessage))
            {
                result.Message = validateMessage;
                return result;
            }

            purchaseOrder.SetUpdateAudit(request.GetUser());

            try
            {
                var reservations = purchaseOrder.PurchaseOrderGroupLines?
                    .SelectMany(x => x.PurchaseOrderLines)
                    .SelectMany(l => l.ReservationEntries);

                var productionBomIDs = reservations
                    .Where(x => x.ProductionBOMID != null)
                    .Select(r => (long)r.ProductionBOMID)
                    .Distinct();

                ///Update production BOM quantity
                var productionBoms = productionBOMRepository.GetProductionBOMs(productionBomIDs.ToList());
                UpdateProductionBOMQuantityProcess.CalculateReservedQuantity(reservations.ToList(),
                    productionBoms.ToList(),
                    out string errorMessage);
                context.ProductionBOM.UpdateRange(productionBoms);

                ///Update reserved quantity
                UpdatePurchaseOrderLineQuantity.UpdateReservedQuantity(
                    purchaseOrder.PurchaseOrderGroupLines.SelectMany(x => x.PurchaseOrderLines).ToList());

                var lines = purchaseOrder.PurchaseOrderLines;
                var groupLines = purchaseOrder.PurchaseOrderGroupLines;
                groupLines.Where(x => x.ID == 0).ToList().ForEach(x => 
                {
                    x.PurchaseOrderID = purchaseOrder.ID;
                });

                purchaseOrder.PurchaseOrderLines = null;
                purchaseOrder.PurchaseOrderGroupLines = null;

                context.PurchaseOrderGroupLine
                    .AddRange(request.PurchaseOrderGroupLines.Where(x => x.ID == 0));
                context.PurchaseOrderGroupLine.UpdateRange(groupLines.Where(x => x.ID != 0));

                context.PurchaseOrder.Update(purchaseOrder);
                await context.SaveChangesAsync(cancellationToken);

                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
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
