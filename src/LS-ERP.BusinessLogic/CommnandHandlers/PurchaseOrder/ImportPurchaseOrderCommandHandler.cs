using EFCore.BulkExtensions;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ImportPurchaseOrderCommandHandler
        : IRequestHandler<ImportPurchaseOrderCommand, ImportPurchaseOrderResult>
    {
        private readonly ILogger<ImportSalesOrderCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly PurchaseOrderValidator validator;
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly IPartRevisionRepository partRevisionRepository;
        private readonly IPartMaterialRepository partMaterialRepository;
        private readonly IWastageSettingRepository wastageSettingRepository;
        private readonly IProductionBOMRepository productionBOMRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IVendorRepository vendorRepository;
        private readonly IPurchaseOrderGroupLineRepository purchaseOrderGroupLineRepository;
        private readonly IPurchaseOrderLineRepository purchaseOrderLineRepository;

        public ImportPurchaseOrderCommandHandler(ILogger<ImportSalesOrderCommandHandler> logger,
            SqlServerAppDbContext context,
            PurchaseOrderValidator validator,
            IPurchaseOrderRepository purchaseOrderRepository,
            IPartRevisionRepository partRevisionRepository,
            IPartMaterialRepository partMaterialRepository,
            IWastageSettingRepository wastageSettingRepository,
            IProductionBOMRepository productionBOMRepository,
            IItemStyleRepository itemStyleRepository,
            IVendorRepository vendorRepository,
            IPurchaseOrderGroupLineRepository purchaseOrderGroupLineRepository,
            IPurchaseOrderLineRepository purchaseOrderLineRepository)
        {
            this.logger = logger;
            this.context = context;
            this.validator = validator;
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.partRevisionRepository = partRevisionRepository;
            this.partMaterialRepository = partMaterialRepository;
            this.wastageSettingRepository = wastageSettingRepository;
            this.productionBOMRepository = productionBOMRepository;
            this.itemStyleRepository = itemStyleRepository;
            this.vendorRepository = vendorRepository;
            this.purchaseOrderGroupLineRepository = purchaseOrderGroupLineRepository;
            this.purchaseOrderLineRepository = purchaseOrderLineRepository;
        }

        public async Task<ImportPurchaseOrderResult> Handle(ImportPurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import purchase order command", DateTime.Now.ToString());
            var result = new ImportPurchaseOrderResult();
            string fileName = request.File.FileName;

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var vendors = vendorRepository.GetVendors().ToList();

            ImportPurchaseOrderProcess.Import(fullPath, fileName, request.GetUser(), request.CustomerID,
                                                            vendors, request.Type,
                                                            out List<PurchaseOrderType> newPurchaseOrderTypes,
                                                            out List<string> contractNos,
                                                            out List<string> purchaseOrderNumbers,
                                                            out List<Vendor> newVendors,
                                                            out Dictionary<string, List<PurchaseOrderLine>> dicTrackingPO,
                                                            out Dictionary<string, List<PurchaseOrderLine>> dicDetailPO,
                                                            out Dictionary<string, List<PurchaseOrderLine>> dicInternationalOCL,
                                                            out Dictionary<string, List<PurchaseOrderLine>> dicCareLabelPO,
                                                            out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            if (newVendors.Any())
            {
                context.BulkInsert(newVendors);
            }

            //List<ProductionBOM> productionBOMs = productionBOMRepository.GetProductionBOMsOfContractNos(contractNos).ToList();

            

            var dicOldPurchaseOrders = new Dictionary<string, PurchaseOrder>();
            var listOldPurchaseOrderGroupLine = new List<PurchaseOrderGroupLine>();
            var listOldPurchaseOrderLine = new List<PurchaseOrderLine>();

            if (purchaseOrderNumbers.Any())
            {
                dicOldPurchaseOrders = purchaseOrderRepository.GetOnlyPurchaseOrders(purchaseOrderNumbers).ToDictionary(x => x.Number);

                var POIDs = new List<string>();

                foreach (var item in dicOldPurchaseOrders)
                {
                    POIDs.Add(item.Value.ID);
                }

                listOldPurchaseOrderGroupLine = purchaseOrderGroupLineRepository.GetPurchaseOrderGroupLines(POIDs).ToList();
                listOldPurchaseOrderLine = purchaseOrderLineRepository.GetPurchaseOrderLinesFollowPO(POIDs).ToList();
            }
            else
            {
                dicOldPurchaseOrders = purchaseOrderRepository.GetPurchaseOrders(request.CustomerID).ToDictionary(x => x.Number);
            }

            List<PurchaseOrder> newPurchaseOrders = new List<PurchaseOrder>();
            List<PurchaseOrderGroupLine> updateOldPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            List<PurchaseOrderGroupLine> newOldPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            List<PurchaseOrderGroupLine> newPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            List<PurchaseOrderLine> updateOldPurchaseOrderLines = new List<PurchaseOrderLine>();
            List<PurchaseOrderLine> newOldPurchaseOrderLines = new List<PurchaseOrderLine>();
            List<PurchaseOrderLine> newPurchaseOrderLines = new List<PurchaseOrderLine>();
            try
            {

                switch (request.Type)
                {
                    case XAF.Module.DomainComponent.TypeImportPurchaseOrder.FabricTracking:
                    case XAF.Module.DomainComponent.TypeImportPurchaseOrder.TrimTracking:
                        {
                            ImportPurchaseOrderProcess.CreateOrUpdatePurchaseOrder(request.Username, dicOldPurchaseOrders, dicTrackingPO,
                                listOldPurchaseOrderGroupLine,
                                out newPurchaseOrders,
                                out updateOldPurchaseOrderGroupLines,
                                out newOldPurchaseOrderGroupLines,
                                out newPurchaseOrderGroupLines,
                                out errorMessage);
                        }
                        break;
                    case XAF.Module.DomainComponent.TypeImportPurchaseOrder.PurchaseDetail:
                        {
                            var partMaterials = partMaterialRepository.GetPartMaterials(contractNos)?.ToList();
                            List<ItemStyle> itemStyles = itemStyleRepository.GetOnlyItemStylesFollowContractNo(contractNos).ToList();

                            ImportPurchaseOrderProcess.CreateOrUpdatePurchaseOrderLine(request.Username, dicOldPurchaseOrders, dicDetailPO,
                                dicInternationalOCL, listOldPurchaseOrderGroupLine, listOldPurchaseOrderLine, itemStyles, partMaterials,
                                out updateOldPurchaseOrderGroupLines,
                                out updateOldPurchaseOrderLines,
                                out newOldPurchaseOrderLines,
                                out errorMessage);
                        }
                        break;
                    case XAF.Module.DomainComponent.TypeImportPurchaseOrder.Carelabel:
                        {
                            var partMaterials = partMaterialRepository.GetPartMaterials(contractNos)?.ToList();
                            List<ItemStyle> itemStyles = itemStyleRepository.GetOnlyItemStylesFollowContractNo(contractNos).ToList();

                            ImportPurchaseOrderProcess.CreateOrUpdatePurchaseOrder_CareLabel(request.Username, dicOldPurchaseOrders,
                                dicCareLabelPO, listOldPurchaseOrderGroupLine, listOldPurchaseOrderLine, itemStyles, partMaterials,
                                out newPurchaseOrders,
                                out updateOldPurchaseOrderGroupLines,
                                out newOldPurchaseOrderGroupLines,
                                out newPurchaseOrderGroupLines,
                                out updateOldPurchaseOrderLines,
                                out newOldPurchaseOrderLines,
                                //out newPurchaseOrderLines,
                                out errorMessage);
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    result.Message = errorMessage;
                    return result;
                }

                if (newOldPurchaseOrderGroupLines.Any())
                {
                    context.BulkInsert(newOldPurchaseOrderGroupLines);
                }

                if (updateOldPurchaseOrderGroupLines.Any())
                {
                    context.BulkUpdate(updateOldPurchaseOrderGroupLines);
                }

                if (updateOldPurchaseOrderLines.Any())
                {
                    context.BulkUpdate(updateOldPurchaseOrderLines);
                }

                if (newOldPurchaseOrderLines.Any())
                {
                    context.BulkInsert(newOldPurchaseOrderLines);
                }

                var bulkConfigOldPO = new BulkConfig()
                {
                    SetOutputIdentity = true,
                    PreserveInsertOrder = true
                };

                // new PO
                var bulkConfig = new BulkConfig()
                {
                    SetOutputIdentity = true,
                    PreserveInsertOrder = true
                };

                var newReservationEntry = new List<ReservationEntry>();

                context.BulkInsert(newPurchaseOrders, bulkConfig);

                var newGroupLine = new List<PurchaseOrderGroupLine>();

                foreach (var purchaseOrder in newPurchaseOrders)
                {
                    foreach (PurchaseOrderGroupLine purchaseOrderGroupLine in newPurchaseOrderGroupLines)
                    {
                        if (purchaseOrder.Number == purchaseOrderGroupLine.CustomerPurchaseOrderNumber)
                        {
                            purchaseOrderGroupLine.PurchaseOrderID = purchaseOrder.ID;
                        }
                    }
                }

                context.BulkInsert(newPurchaseOrderGroupLines, bulkConfig);

                foreach (PurchaseOrderGroupLine purchaseOrderGroupLine in newPurchaseOrderGroupLines)
                {
                    foreach (var purchaseOrderLine in purchaseOrderGroupLine.PurchaseOrderLines)
                    {
                        purchaseOrderLine.PurchaseOrderGroupLineID = purchaseOrderGroupLine.ID;
                        purchaseOrderLine.PurchaseOrderID = purchaseOrderGroupLine.PurchaseOrderID;
                    }
                }

                newPurchaseOrderLines = newPurchaseOrderGroupLines.SelectMany(x => x.PurchaseOrderLines).ToList();

                await context.BulkInsertAsync(newPurchaseOrderLines);

                //foreach (PurchaseOrderLine purchaseOrderLine in newPurchaseOrderLines)
                //{
                //    foreach (ReservationEntry reservationEntry in purchaseOrderLine.ReservationEntries)
                //    {
                //        reservationEntry.PurchaseOrderLineID = purchaseOrderLine.ID;
                //    }
                //}
                //var newReservationEntries = newPurchaseOrderLines.SelectMany(x => x.ReservationEntries).ToList();

                //await context.BulkInsertAsync(newReservationEntries);
                result.IsSuccess = true;

                result.Message = "Import purchase order sucessfully";

                //await mediator.Publish(new ItemStylePullBomEvent()
                //{
                //    ItemStyleNumbers = request.ItemStyleNumbers.ToList()
                //}).ConfigureAwait(false);
            }
            catch (DbUpdateException exception)
            {
                result.Message = exception.InnerException.Message;
            }


            return result;
        }
    }
}
