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
    public class UpdateOnHandQuantityStorageDetailJob
    {
        private readonly ILogger<UpdateOnHandQuantityStorageDetailJob> logger;
        private readonly SqlServerAppDbContext context;

        public UpdateOnHandQuantityStorageDetailJob(ILogger<UpdateOnHandQuantityStorageDetailJob> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Update OnHand Quantity Storage Detail Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(int StorageImportID)
        {
            var materialTransactions = new List<MaterialTransaction>();
            var updateStorageDetails = new List<StorageDetail>();
            var addStorageDetails = new List<StorageDetail>();
            var addMaterialTransactions = new List<MaterialTransaction>();
            var customerID = "";
            var storageCode = "";
            try
            {
                var storageImport = context.StorageImport
                    .Include(x => x.Details)
                    .FirstOrDefault(x => x.ID == StorageImportID);
                customerID = storageImport.CustomerID;
                storageCode = storageImport.StorageCode;

                var storageDetails = context.StorageDetail
                    .Where(x => x.StorageCode == storageImport.StorageCode &&
                                x.CustomerID == storageImport.CustomerID).ToList();

                if (storageImport != null)
                {
                    if (storageImport.Details != null &&
                       storageImport.Details.Any())
                    {
                        foreach (var detail in storageImport.Details)
                        {
                            //var lsStyles = ConvertLSStyle(detail.PurchaseOrderNumber ?? string.Empty,
                            //                              detail.CustomerStyle ?? string.Empty,
                            //                              detail.Season ?? string.Empty,
                            //                              detail.LSStyle ?? string.Empty);
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
                                Season = detail.Season,
                                InvoiceNumber = detail.InvoiceNumber,
                                DocumentNumber = detail.DocumentNumber,
                                Remark = detail.Remark,
                                OldImport = true
                            };

                            materialTransactions.Add(materialTransaction);
                        }
                    }
                }

                foreach (var storageDetail in storageDetails)
                {
                    //var transactions = materialTransactions
                    //    .Where(x => (x.ItemID ?? string.Empty).Trim().ToUpper() == (storageDetail.ItemID ?? string.Empty).Trim().ToUpper() &&
                    //                (x.ItemName ?? string.Empty).Trim().ToUpper() == (storageDetail.ItemName ?? string.Empty).Trim().ToUpper() &&
                    //                (x.ItemColorCode ?? string.Empty).Trim().ToUpper() == (storageDetail.ItemColorCode ?? string.Empty).Trim().ToUpper() &&
                    //                (x.ItemColorName ?? string.Empty).Trim().ToUpper() == (storageDetail.ItemColorName ?? string.Empty).Trim().ToUpper() &&
                    //                //(x.Specify ?? string.Empty).Trim().ToUpper() == (storageDetail.Specify ?? string.Empty).Trim().ToUpper() &&
                    //                //(x.Position ?? string.Empty).Trim().ToUpper() == (storageDetail.Position ?? string.Empty).Trim().ToUpper() &&
                    //                (x.GarmentColorCode ?? string.Empty).Trim().ToUpper() == (storageDetail.GarmentColorCode ?? string.Empty).Trim().ToUpper() &&
                    //                (x.GarmentColorName ?? string.Empty).Trim().ToUpper() == (storageDetail.GarmentColorName ?? string.Empty).Trim().ToUpper() &&
                    //                (x.GarmentSize ?? string.Empty).Trim().ToUpper() == (storageDetail.GarmentSize ?? string.Empty).Trim().ToUpper() &&
                    //                (x.PurchaseOrderNumber ?? string.Empty).Trim().ToUpper() == (storageDetail.PurchaseOrderNumber ?? string.Empty).Trim().ToUpper())
                    //                .ToList();

                    var transactions = materialTransactions
                        .Where(x => (x.PurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (x.ItemID ?? string.Empty).Trim().ToUpper() +
                                    (x.ItemName ?? string.Empty).Trim().ToUpper() + (x.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                                    (x.ItemColorName ?? string.Empty).Trim().ToUpper() + (x.GarmentColorCode ?? string.Empty).Trim().ToUpper() +
                                    (x.GarmentColorName ?? string.Empty).Trim().ToUpper() + (x.GarmentSize ?? string.Empty).Trim().ToUpper() +
                                    (x.CustomerStyle ?? string.Empty).Trim().ToUpper() + (x.Season ?? string.Empty).Trim().ToUpper() +
                                    (x.Specify ?? string.Empty).Trim().ToUpper() ==
                                    (storageDetail.PurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (storageDetail.ItemID ?? string.Empty).Trim().ToUpper() +
                                    (storageDetail.ItemName ?? string.Empty).Trim().ToUpper() + (storageDetail.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                                    (storageDetail.ItemColorName ?? string.Empty).Trim().ToUpper() + (storageDetail.GarmentColorCode ?? string.Empty).Trim().ToUpper() +
                                    (storageDetail.GarmentColorName ?? string.Empty).Trim().ToUpper() + (storageDetail.GarmentSize ?? string.Empty).Trim().ToUpper() +
                                    (storageDetail.CustomerStyle ?? string.Empty).Trim().ToUpper() + (storageDetail.Season ?? string.Empty).Trim().ToUpper() +
                                    (storageDetail.Specify ?? string.Empty).Trim().ToUpper()).ToList();

                    foreach (var transaction in transactions)
                    {
                        transaction.StorageCode = storageDetail.StorageCode;
                        transaction.StorageDetailID = storageDetail.ID;
                        materialTransactions.Remove(transaction);
                        addMaterialTransactions.Add(transaction);
                    }

                    if (transactions.Any())
                    {
                        storageDetail.OnHandQuantity = transactions.Sum(x => x.Quantity);
                        storageDetail.Roll = transactions.Sum(x => x.Roll);
                        updateStorageDetails.Add(storageDetail);
                    }
                    else
                    {
                        storageDetail.OnHandQuantity = 0;
                        storageDetail.Roll = 0;
                        updateStorageDetails.Add(storageDetail);
                    }
                }

                materialTransactions.ForEach(x =>
                {
                    var existsStorageDetail = addStorageDetails
                        .FirstOrDefault(y => (x.PurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (x.ItemID ?? string.Empty).Trim().ToUpper() +
                                             (x.ItemName ?? string.Empty).Trim().ToUpper() + (x.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                                             (x.ItemColorName ?? string.Empty).Trim().ToUpper() + (x.GarmentColorCode ?? string.Empty).Trim().ToUpper() +
                                             (x.GarmentColorName ?? string.Empty).Trim().ToUpper() + (x.GarmentSize ?? string.Empty).Trim().ToUpper() +
                                             (x.CustomerStyle ?? string.Empty).Trim().ToUpper() + (x.Season ?? string.Empty).Trim().ToUpper() +
                                             (x.Specify ?? string.Empty).Trim().ToUpper() ==
                                             (y.PurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (y.ItemID ?? string.Empty).Trim().ToUpper() +
                                             (y.ItemName ?? string.Empty).Trim().ToUpper() + (y.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                                             (y.ItemColorName ?? string.Empty).Trim().ToUpper() + (y.GarmentColorCode ?? string.Empty).Trim().ToUpper() +
                                             (y.GarmentColorName ?? string.Empty).Trim().ToUpper() + (y.GarmentSize ?? string.Empty).Trim().ToUpper() +
                                             (y.CustomerStyle ?? string.Empty).Trim().ToUpper() + (y.Season ?? string.Empty).Trim().ToUpper() +
                                             (y.Specify ?? string.Empty).Trim().ToUpper());

                    if (existsStorageDetail != null)
                    {
                        existsStorageDetail.OnHandQuantity += x.Quantity;
                        existsStorageDetail.Roll += x.Roll;
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
                            Season = x.Season,
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
                            LotNumber = x.LotNumber,
                            InvoiceNumber = x.InvoiceNumber,
                            DocumentNumber = x.DocumentNumber,
                            Remark = x.Remark,
                            OldImport = true
                        };
                        addStorageDetails.Add(storageDetail);
                        addMaterialTransactions.Add(x);
                    }
                });

                context.StorageDetail.UpdateRange(updateStorageDetails);
                context.StorageDetail.AddRange(addStorageDetails);
                context.MaterialTransaction.AddRange(addMaterialTransactions);

                context.SaveChanges();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Storage import event handler has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Storage import event handler has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
                return Task.CompletedTask;
            }
            //finally
            //{
            //    /// Run Hangfire
            //    var jobId = BackgroundJob.Enqueue<UpdateLSStyleStorageDetailJob>(j => j.Execute(addMaterialTransactions, customerID, storageCode));
            //}
        }
    }
}
