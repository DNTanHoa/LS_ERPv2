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
    public class ImportShipmentCommandHandler
        : IRequestHandler<ImportShipmentCommand, ImportShipmentResult>
    {
        private readonly ILogger<ImportSalesOrderCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IShipmentRepository shipmentRepository;
        private readonly IShipmentDetailRepository shipmentDetailRepository;
        private readonly IPurchaseOrderLineRepository purchaseOrderLineRepository;

        public ImportShipmentCommandHandler(ILogger<ImportSalesOrderCommandHandler> logger,
            SqlServerAppDbContext context,
            IShipmentRepository shipmentRepository,
            IShipmentDetailRepository shipmentDetailRepository,
            IPurchaseOrderLineRepository purchaseOrderLineRepository)
        {
            this.logger = logger;
            this.context = context;
            this.shipmentRepository = shipmentRepository;
            this.shipmentDetailRepository = shipmentDetailRepository;
            this.purchaseOrderLineRepository = purchaseOrderLineRepository;
        }
        public async Task<ImportShipmentResult> Handle(ImportShipmentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import shipment command", DateTime.Now.ToString());
            var result = new ImportShipmentResult();
            string fileName = request.File.FileName;

            var check = shipmentRepository.IsExist(fileName);
            if (check)
            {
                result.Message = "File has exists, please re-check !!!";
                return result;
            }

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            List<Shipment> shipments = new List<Shipment>();

            var shipment = ImportShipmentProcessor.Import(fullPath, subPath, request.File.FileName, request.UserName,
                request.CustomerID, shipments, out string errorMessage);

            try
            {
                if (shipment != null && !string.IsNullOrEmpty(shipment.FilePath))
                {
                    context.Shipment.Add(shipment);
                    context.SaveChanges();

                    //var shipmentDetails = shipmentDetailRepository.GetShipmentDetails(request.CustomerID).ToList();
                    //var purchaseOrderNumbers = shipment.Details.Select(x => x.CustomerPurchaseOrderNumber).ToList();
                    //var purchaseOrderLines = purchaseOrderLineRepository.GetPurchaseOrderLinesMatching(purchaseOrderNumbers).ToList();

                    //MatchingShipmentPurchaseOrderProcessor.Matching(shipment.Details, ref purchaseOrderLines,
                    //    out List<ShipmentDetail> updateShipmentDetails, out errorMessage);

                    //if (!string.IsNullOrEmpty(errorMessage))
                    //{
                    //    result.Message = errorMessage;
                    //    return result;
                    //}

                    //if (purchaseOrderLines.Any())
                    //{
                    //    context.PurchaseOrderLine.UpdateRange(purchaseOrderLines);
                    //}

                    //if (updateShipmentDetails.Any())
                    //{
                    //    context.ShipmentDetail.UpdateRange(updateShipmentDetails);
                    //}

                    await context.SaveChangesAsync(cancellationToken);
                    result.IsSuccess = true;

                    result.Result = shipment;
                }
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
