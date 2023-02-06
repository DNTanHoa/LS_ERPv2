using AutoMapper;
using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdateOnHandQuantityFabricStorageDetailJob
    {
        private readonly ILogger<UpdateOnHandQuantityFabricStorageDetailJob> logger;
        private readonly SqlServerAppDbContext context;

        public UpdateOnHandQuantityFabricStorageDetailJob(ILogger<UpdateOnHandQuantityFabricStorageDetailJob> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Update OnHand Quantity Fabric Storage Detail Job")]
        [AutomaticRetry(Attempts = 1)]
        public Task Execute(int StorageImportID)
        {
            var materialTransactions = new List<MaterialTransaction>();
            //var updateStorageDetails = new List<StorageDetail>();
            var addStorageDetails = new List<StorageDetail>();
            var addMaterialTransactions = new List<MaterialTransaction>();
            var updateFabricPurchaseOrders = new List<FabricPurchaseOrder>();

            try
            {
                var storageImport = context.StorageImport
                    .Include(x => x.Details)
                    .FirstOrDefault(x => x.ID == StorageImportID);

                //var storageDetails = context.StorageDetail
                //    .Where(x => x.StorageCode == storageImport.StorageCode &&
                //                x.CustomerID == storageImport.CustomerID).ToList();

                List<string> fabricPurchaseNumbers = new List<string>();

                if (storageImport != null)
                {
                    if (storageImport.Details != null &&
                       storageImport.Details.Any())
                    {
                        foreach (var detail in storageImport.Details)
                        {

                            var materialTransaction = new MaterialTransaction()
                            {
                                ItemCode = detail.ItemCode,
                                ItemID = detail.ItemID,
                                ItemName = detail.ItemName,
                                ItemColorCode = detail.ItemColorCode,
                                ItemColorName = detail.ItemColorName,
                                Position = detail.Position,
                                Specify = detail.Specify,
                                GarmentColorCode = detail.GarmentColorCode,
                                GarmentColorName = detail.GarmentColorName,
                                GarmentSize = detail.GarmentSize,
                                CustomerStyle = detail.CustomerStyle,
                                LSStyle = detail.LSStyle,
                                UnitID = detail.UnitID,
                                Quantity = detail.Quantity,
                                Roll = detail.Roll,
                                StorageImportDetailID = detail.ID,
                                StorageCode = storageImport.StorageCode,
                                PurchaseOrderNumber = detail.PurchaseOrderNumber,
                                StorageBinCode = detail.StorageBinCode,
                                LotNumber = detail.LotNumber,
                                //Season = detail.Season,
                                InvoiceNumber = detail.InvoiceNumber,
                                DocumentNumber = detail.DocumentNumber,
                                Remark = detail.Remark,
                                InvoiceNumberNoTotal = detail.InvoiceNumberNoTotal,
                                DyeLotNumber = detail.DyeLotNumber,
                                FabricPurchaseOrderNumber = detail.FabricPurchaseOrderNumber,
                                FabricContent = detail.FabricContent,
                                Note = detail.Note,
                                Zone = detail.Zone,
                                UserFollow = detail.UserFollow,
                                Offset = detail.Offset,
                                TransactionDate = detail.TransactionDate,
                                StorageStatusID = detail.StorageStatusID,
                                ProductionMethodCode = detail.ProductionMethodCode,
                                Supplier = detail.Supplier
                            };

                            fabricPurchaseNumbers.Add(materialTransaction.FabricPurchaseOrderNumber);
                            materialTransactions.Add(materialTransaction);
                        }
                    }
                }

                //foreach (var storageDetail in storageDetails)
                //{
                //    var transactions = materialTransactions
                //        .Where(x => (x.FabricPurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (x.ItemID ?? string.Empty).Trim().ToUpper() +
                //                    (x.ItemName ?? string.Empty).Trim().ToUpper() + (x.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                //                    (x.ItemColorName ?? string.Empty).Trim().ToUpper() + (x.TransactionDate ?? DateTime.Now).ToString().Trim().ToUpper() +
                //                    (x.LotNumber ?? string.Empty).Trim().ToUpper() + (x.DyeLotNumber ?? string.Empty).Trim().ToUpper()
                //                    ==
                //                    (storageDetail.FabricPurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (storageDetail.ItemID ?? string.Empty).Trim().ToUpper() +
                //                    (storageDetail.ItemName ?? string.Empty).Trim().ToUpper() + (storageDetail.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                //                    (storageDetail.ItemColorName ?? string.Empty).Trim().ToUpper() + (x.TransactionDate ?? DateTime.Now).ToString().Trim().ToUpper() +
                //                    (storageDetail.LotNumber ?? string.Empty).Trim().ToUpper() + (storageDetail.DyeLotNumber ?? string.Empty).Trim().ToUpper()).ToList();

                //    foreach (var transaction in transactions)
                //    {
                //        transaction.StorageCode = storageDetail.StorageCode;
                //        transaction.StorageDetailID = storageDetail.ID;
                //        materialTransactions.Remove(transaction);
                //        addMaterialTransactions.Add(transaction);
                //    }

                //    if (transactions.Any())
                //    {
                //        storageDetail.OnHandQuantity = transactions.Sum(x => x.Quantity);
                //        updateStorageDetails.Add(storageDetail);
                //    }
                //    else
                //    {
                //        storageDetail.OnHandQuantity = 0;
                //        updateStorageDetails.Add(storageDetail);
                //    }
                //}

                materialTransactions.ForEach(x =>
                {
                    var existsStorageDetail = addStorageDetails
                        .FirstOrDefault(y => (x.FabricPurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (x.ItemID ?? string.Empty).Trim().ToUpper() +
                                             (x.ItemName ?? string.Empty).Trim().ToUpper() + (x.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                                             (x.ItemColorName ?? string.Empty).Trim().ToUpper() + (x.Specify ?? string.Empty).Trim().ToUpper() +
                                             (x.LotNumber ?? string.Empty).Trim().ToUpper() + (x.DyeLotNumber ?? string.Empty).Trim().ToUpper()
                                             ==
                                             (y.FabricPurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (y.ItemID ?? string.Empty).Trim().ToUpper() +
                                             (y.ItemName ?? string.Empty).Trim().ToUpper() + (y.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                                             (y.ItemColorName ?? string.Empty).Trim().ToUpper() + (y.Specify ?? string.Empty).Trim().ToUpper() +
                                             (y.LotNumber ?? string.Empty).Trim().ToUpper() + (y.DyeLotNumber ?? string.Empty).Trim().ToUpper());

                    if (existsStorageDetail != null)
                    {
                        existsStorageDetail.OnHandQuantity += x.Quantity;
                        existsStorageDetail.MaterialTransactions.Add(x);
                        addMaterialTransactions.Add(x);
                    }
                    else
                    {
                        var storageDetail = new StorageDetail()
                        {
                            ItemCode = x.ItemCode,
                            ItemID = x.ItemID,
                            ItemName = x.ItemName,
                            ItemColorCode = x.ItemColorCode,
                            ItemColorName = x.ItemColorName,
                            Position = x.Position,
                            Specify = x.Specify,
                            //Season = x.Season,
                            GarmentColorCode = x.GarmentColorCode,
                            GarmentColorName = x.GarmentColorName,
                            GarmentSize = x.GarmentSize,
                            CustomerStyle = x.CustomerStyle,
                            LSStyle = x.LSStyle,
                            UnitID = x.UnitID,
                            OnHandQuantity = x.Quantity,
                            Roll = x.Roll,
                            StorageCode = storageImport.StorageCode,
                            StorageBinCode = x.StorageBinCode,
                            PurchaseOrderNumber = x.PurchaseOrderNumber,
                            CustomerID = storageImport.CustomerID,
                            ProductionMethodCode = x.ProductionMethodCode,
                            LotNumber = x.LotNumber,
                            InvoiceNumber = x.InvoiceNumber,
                            DocumentNumber = x.DocumentNumber,
                            Remark = x.Remark,
                            InvoiceNumberNoTotal = x.InvoiceNumberNoTotal,
                            DyeLotNumber = x.DyeLotNumber,
                            FabricPurchaseOrderNumber = x.FabricPurchaseOrderNumber,
                            FabricContent = x.FabricContent,
                            Note = x.Note,
                            Zone = x.Zone,
                            UserFollow = x.UserFollow,
                            Offset = x.Offset,
                            StorageStatusID = x.StorageStatusID,
                        };
                        storageDetail.MaterialTransactions.Add(x);
                        addStorageDetails.Add(storageDetail);
                        addMaterialTransactions.Add(x);
                    }
                });

                /// update FB Purchase Order
                var dicFabricPurchaseNumbers = context.FabricPurchaseOrder.Where(x => fabricPurchaseNumbers.Contains(x.Number)).ToDictionary(x => x.Number);

                var config = new MapperConfiguration(
                           cfg => cfg.CreateMap<FabricPurchaseOrder, FabricPurchaseOrder>()
                           .ForMember(d => d.ID, o => o.Ignore())
                           );
                var mapperFB = new Mapper(config);

                var configStore = new MapperConfiguration(
                           cfg => cfg.CreateMap<StorageDetail, FabricPurchaseOrder>()
                           .ForMember(d => d.ID, o => o.Ignore())
                           .ForMember(d => d.Number, o => o.MapFrom(s => s.FabricPurchaseOrderNumber))
                           );
                var mapperStore = new Mapper(configStore);

                Dictionary<string, FabricPurchaseOrder> dicNewFabricPurchaseOrders = new Dictionary<string, FabricPurchaseOrder>();

                //foreach (var itemStorageDetail in updateStorageDetails)
                //{

                //    if (!string.IsNullOrEmpty(itemStorageDetail.FabricPurchaseOrderNumber) &&
                //        dicFabricPurchaseNumbers.TryGetValue(itemStorageDetail.FabricPurchaseOrderNumber, out FabricPurchaseOrder fabricPurchaseOrder))
                //    {
                //        if (fabricPurchaseOrder.ReceivedQuantity == null)
                //            fabricPurchaseOrder.ReceivedQuantity = 0;

                //        if (fabricPurchaseOrder.OnHandQuantity == null)
                //        {
                //            fabricPurchaseOrder.OnHandQuantity = 0;
                //        }

                //        fabricPurchaseOrder.ReceivedQuantity += itemStorageDetail.OnHandQuantity;

                //        fabricPurchaseOrder.DeliveredQuantity = fabricPurchaseOrder.ReceivedQuantity + fabricPurchaseOrder.ShippedQuantity;
                //        fabricPurchaseOrder.OnHandQuantity = fabricPurchaseOrder.ReceivedQuantity + fabricPurchaseOrder.OnHandQuantity;
                //    }
                //    else
                //    {
                //        if (!string.IsNullOrEmpty(itemStorageDetail.FabricPurchaseOrderNumber) &&
                //            dicNewFabricPurchaseOrders.TryGetValue(itemStorageDetail.FabricPurchaseOrderNumber, out FabricPurchaseOrder rsFB))
                //        {
                //            if (rsFB.ReceivedQuantity == null)
                //                rsFB.ReceivedQuantity = 0;

                //            if (rsFB.OnHandQuantity == null)
                //            {
                //                rsFB.OnHandQuantity = 0;
                //            }

                //            rsFB.ReceivedQuantity += itemStorageDetail.OnHandQuantity;

                //            rsFB.DeliveredQuantity = rsFB.ReceivedQuantity + rsFB.ShippedQuantity;
                //            rsFB.OnHandQuantity = rsFB.ReceivedQuantity + rsFB.OnHandQuantity;

                //            dicNewFabricPurchaseOrders[itemStorageDetail.FabricPurchaseOrderNumber] = rsFB;
                //        }
                //        else if (!string.IsNullOrEmpty(itemStorageDetail.FabricPurchaseOrderNumber))
                //        {
                //            FabricPurchaseOrder newFabricPurchaseOrder = new FabricPurchaseOrder();
                //            mapperStore.Map(itemStorageDetail, newFabricPurchaseOrder);

                //            if (newFabricPurchaseOrder.ReceivedQuantity == null)
                //                newFabricPurchaseOrder.ReceivedQuantity = 0;

                //            if (newFabricPurchaseOrder.OnHandQuantity == null)
                //            {
                //                newFabricPurchaseOrder.OnHandQuantity = 0;
                //            }

                //            newFabricPurchaseOrder.ReceivedQuantity += itemStorageDetail.OnHandQuantity;

                //            newFabricPurchaseOrder.DeliveredQuantity = newFabricPurchaseOrder.ReceivedQuantity + newFabricPurchaseOrder.ShippedQuantity;
                //            newFabricPurchaseOrder.OnHandQuantity = newFabricPurchaseOrder.ReceivedQuantity + newFabricPurchaseOrder.OnHandQuantity;
                //            dicNewFabricPurchaseOrders[newFabricPurchaseOrder.Number] = newFabricPurchaseOrder;
                //        }
                //    }
                //}

                if (addStorageDetails != null && addStorageDetails.Count > 0)
                {
                    foreach (var itemStorageDetail in addStorageDetails)
                    {
                        if (dicFabricPurchaseNumbers.TryGetValue(itemStorageDetail.FabricPurchaseOrderNumber, out FabricPurchaseOrder rsFB))
                        {

                            if (rsFB.ReceivedQuantity == null)
                                rsFB.ReceivedQuantity = 0;

                            if (rsFB.OnHandQuantity == null)
                            {
                                rsFB.OnHandQuantity = 0;
                            }

                            rsFB.ReceivedQuantity += itemStorageDetail.OnHandQuantity;

                            rsFB.DeliveredQuantity = rsFB.ReceivedQuantity + rsFB.ShippedQuantity;
                            rsFB.OnHandQuantity = rsFB.ReceivedQuantity;

                            dicFabricPurchaseNumbers[itemStorageDetail.FabricPurchaseOrderNumber] = rsFB;
                        }
                    }

                }

                foreach (var itemFB in dicFabricPurchaseNumbers)
                {
                    updateFabricPurchaseOrders.Add(itemFB.Value);
                }

                var newFabricPurchaseOrders = new List<FabricPurchaseOrder>();

                foreach (var itemFB in dicNewFabricPurchaseOrders)
                {
                    newFabricPurchaseOrders.Add(itemFB.Value);
                }

                context.MaterialTransaction.AddRange(addMaterialTransactions);
                //context.StorageDetail.UpdateRange(updateStorageDetails);
                context.StorageDetail.AddRange(addStorageDetails);
                context.FabricPurchaseOrder.UpdateRange(updateFabricPurchaseOrders);
                context.FabricPurchaseOrder.AddRange(newFabricPurchaseOrders);

                context.SaveChanges();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Fabric storage import event handler has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Fabric storage import event handler has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
                return Task.CompletedTask;
            }
        }
    }
}
