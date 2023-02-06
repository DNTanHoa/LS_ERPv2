using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class MatchingShipmentPurchaseOrderCommandHandler
        : IRequestHandler<MatchingShipmentPurchaseOrderCommand, MatchingShipmentPurchaseOrderResult>
    {
        private readonly ILogger<MatchingShipmentPurchaseOrderCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IPurchaseOrderLineRepository purchaseOrderLineRepository;
        private readonly IShipmentDetailRepository shipmentDetailRepository;

        public MatchingShipmentPurchaseOrderCommandHandler(ILogger<MatchingShipmentPurchaseOrderCommandHandler> logger,
            SqlServerAppDbContext context,
            IPurchaseOrderLineRepository purchaseOrderLineRepository,
            IShipmentDetailRepository shipmentDetailRepository)
        {
            this.logger = logger;
            this.context = context;
            this.purchaseOrderLineRepository = purchaseOrderLineRepository;
            this.shipmentDetailRepository = shipmentDetailRepository;
        }
        public async Task<MatchingShipmentPurchaseOrderResult> Handle(MatchingShipmentPurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute matching shipment purchase order command", DateTime.Now.ToString());
            var result = new MatchingShipmentPurchaseOrderResult();

            var shipmentDetails = shipmentDetailRepository.GetShipmentDetails(request.CustomerID).ToList();
            var purchaseOrderNumbers = shipmentDetails.Select(x => x.CustomerPurchaseOrderNumber).ToList();
            var purchaseOrderLines = purchaseOrderLineRepository.GetPurchaseOrderLinesMatching(purchaseOrderNumbers).ToList();

            MatchingShipmentPurchaseOrderProcessor.Matching(shipmentDetails, ref purchaseOrderLines, out List<ShipmentDetail> updateShipmentDetails, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                if (purchaseOrderLines.Any())
                {
                    context.PurchaseOrderLine.UpdateRange(purchaseOrderLines);
                }

                if (updateShipmentDetails.Any())
                {
                    context.ShipmentDetail.UpdateRange(updateShipmentDetails);
                }

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }
            return result;
        }
    }
}
