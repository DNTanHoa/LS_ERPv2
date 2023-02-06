using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Extensions;
using LS_ERP.Ultilities.Extensions;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class StorageDetailProcess
    {
        /// <summary>
        /// Update storage from transaction
        /// </summary>
        /// <param name="currentStorages"></param>
        /// <param name="transactions"></param>
        /// <param name="username"></param>
        /// <param name="storageDetails"></param>
        public static void UpdateStorageFromReceipt(List<StorageDetail> currentStorages,
            List<MaterialTransaction> transactions, string storageCode,
            string username, out List<StorageDetail> newStorageDetails)
        {
            newStorageDetails = new List<StorageDetail>();

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<MaterialTransaction, StorageDetail>()
                    .ForMember(x => x.InvoiceNumber, y => y.MapFrom(s => s.Receipt.InvoiceNumber))
                    .ForMember(x => x.InvoiceNumberNoTotal, y => y.MapFrom(s => s.Receipt.InvoiceNumberNoTotal))
                    .ForMember(x => x.ProductionMethodCode, y => y.MapFrom(s => s.Receipt.ProductionMethodCode))
                    .ForMember(x => x.DocumentNumber, y => y.MapFrom(s => s.Receipt.DocumentReferenceNumber))
                    .ForMember(x => x.LotNumber, y => y.MapFrom(s => s.ReceiptGroupLine.LotNumber))
                    .ForMember(x => x.PurchaseOrderNumber, y => y.MapFrom(s => s.Receipt.PurchaseOrderNumber))
                    .ForMember(x => x.FabricPurchaseOrderNumber, y => y.MapFrom(s => s.Receipt.FabricPurchaseOrderNumber))
                    .ForMember(x => x.CustomerID, y => y.MapFrom(s => s.Receipt.CustomerID))
                    .ForMember(x => x.DyeLotNumber, y => y.MapFrom(s => s.ReceiptGroupLine.DyeLotNumber))
                    .ForMember(x => x.StorageBinCode, y => y.MapFrom(s => s.ReceiptGroupLine.StorageBinCode))
                    .ForMember(x => x.OnHandQuantity, y => y.MapFrom(s => s.Quantity));
            });

            var mapper = config.CreateMapper();

            foreach (var transaction in transactions)
            {
                var storageDetail = currentStorages?
                    .FirstOrDefault(x => x.ItemID == transaction.ItemID &&
                                         x.ItemName == transaction.ItemName &&
                                         x.ItemColorCode == transaction.ItemColorCode &&
                                         x.ItemColorName == transaction.ItemColorName &&
                                         x.Specify == transaction.Specify &&
                                         x.LSStyle == transaction.LSStyle &&
                                         x.GarmentColorCode == transaction.GarmentColorCode &&
                                         x.GarmentColorName == transaction.GarmentColorName &&
                                         x.GarmentSize == transaction.GarmentSize &&
                                         x.LotNumber == transaction.ReceiptGroupLine?.LotNumber &&
                                         x.DyeLotNumber == transaction.ReceiptGroupLine?.DyeLotNumber &&
                                         (x.PurchaseOrderNumber == transaction.Receipt?.PurchaseOrderNumber
                                         || x.FabricPurchaseOrderNumber == transaction.Receipt?.FabricPurchaseOrderNumber) &&
                                         x.CustomerID == transaction.Receipt?.CustomerID &&
                                         x.InvoiceNumber == transaction.Receipt?.InvoiceNumber);

                if (storageDetail != null)
                {
                    storageDetail.MaterialTransactions.Add(transaction);
                    storageDetail.CalculateQuantity();
                }
                else
                {
                    ///Try search in new list
                    var existNewStorageDetail = newStorageDetails
                        .FirstOrDefault(x => x.ItemID == transaction.ItemID &&
                                         x.ItemName == transaction.ItemName &&
                                         x.ItemColorCode == transaction.ItemColorCode &&
                                         x.ItemColorName == transaction.ItemColorName &&
                                         x.Specify == transaction.Specify &&
                                         x.LSStyle == transaction.LSStyle &&
                                         x.GarmentColorCode == transaction.GarmentColorCode &&
                                         x.GarmentColorName == transaction.GarmentColorName &&
                                         x.GarmentSize == transaction.GarmentSize &&
                                         x.LotNumber == transaction.ReceiptGroupLine?.LotNumber &&
                                         x.DyeLotNumber == transaction.ReceiptGroupLine?.DyeLotNumber &&
                                         (x.PurchaseOrderNumber == transaction.Receipt?.PurchaseOrderNumber
                                         || x.FabricPurchaseOrderNumber == transaction.Receipt?.FabricPurchaseOrderNumber) &&
                                         x.CustomerID == transaction.Receipt?.CustomerID &&
                                         x.InvoiceNumber == transaction.Receipt?.InvoiceNumber);

                    if (existNewStorageDetail != null)
                    {
                        existNewStorageDetail.MaterialTransactions.Add(transaction);
                        existNewStorageDetail.CalculateQuantity();
                    }
                    else
                    {
                        var newStorageDetail = mapper.Map<StorageDetail>(transaction);

                        newStorageDetail.StorageCode = storageCode;

                        newStorageDetail.MaterialTransactions.Add(transaction);
                        newStorageDetail.CalculateQuantity();

                        newStorageDetails.Add(newStorageDetail);
                    }
                }
            }
        }

        /// <summary>
        /// Update storage from transaction for issued
        /// </summary>
        /// <param name="currentStorages"></param>
        /// <param name="transactions"></param>
        /// <param name="storageCode"></param>
        /// <param name="username"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool UpdateStorageFromIssued(List<StorageDetail> currentStorages,
            List<MaterialTransaction> transactions, string storageCode,
            string username, out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach (var transaction in transactions)
            {
                var storageDetail = currentStorages
                    .FirstOrDefault(x => (x.ItemID == transaction.ItemID || string.IsNullOrEmpty(transaction.ItemID)) &&
                                         (x.ItemName == transaction.ItemName || string.IsNullOrEmpty(transaction.ItemName)) &&
                                         (x.ItemColorCode.Replace("\n", " ") == transaction.ItemColorCode || string.IsNullOrEmpty(transaction.ItemColorCode)) &&
                                         (x.ItemColorName.Replace("\n", " ") == transaction.ItemColorName || string.IsNullOrEmpty(transaction.ItemColorName)) &&
                                         (x.Specify == transaction.Specify || string.IsNullOrEmpty(transaction.Specify)) &&
                                         (x.GarmentColorCode == transaction.GarmentColorCode || string.IsNullOrEmpty(transaction.GarmentColorCode)) &&
                                         (x.GarmentColorName == transaction.GarmentColorName || string.IsNullOrEmpty(transaction.GarmentColorName)) &&
                                         (x.GarmentSize == transaction.GarmentSize || string.IsNullOrEmpty(transaction.GarmentSize)) &&
                                         (x.LotNumber == transaction.IssuedLine?.LotNumber || string.IsNullOrEmpty(transaction.IssuedLine?.LotNumber)) &&
                                         (x.DyeLotNumber == transaction.IssuedLine?.DyeLotNumber || string.IsNullOrEmpty(transaction.IssuedLine?.DyeLotNumber)) &&
                                         (x.CustomerID == transaction.Issued?.CustomerID || string.IsNullOrEmpty(transaction.Issued?.CustomerID)));

                if (storageDetail == null)
                {
                    errorMessage = "Can't find item " + transaction.ItemID
                        + "-" + transaction.ItemName
                        + "-" + transaction.ItemColorCode
                        + " in stock";
                    return false;
                }
                else
                {
                    storageDetail.MaterialTransactions.Add(transaction);
                    storageDetail.CalculateQuantity();
                }
            }

            return true;
        }

        /// <summary>
        /// Update fabric storage from transaction for issued
        /// </summary>
        /// <param name="currentStorages"></param>
        /// <param name="transactions"></param>
        /// <param name="storageCode"></param>
        /// <param name="username"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool UpdateFabricStorageFromIssued(List<StorageDetail> currentStorages,
            List<MaterialTransaction> transactions, string storageCode,
            string username, out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach (var transaction in transactions)
            {
                if (!string.IsNullOrEmpty(transaction.FabricPurchaseOrderNumber))
                {
                    var storageDetail = currentStorages
                    .FirstOrDefault(x => x.FabricPurchaseOrderNumber == transaction.FabricPurchaseOrderNumber &&
                                         x.ID == transaction.StorageDetailID);

                    if (storageDetail == null)
                    {
                        errorMessage = "Can't find Fabric PO: " + transaction.FabricPurchaseOrderNumber
                            + "OR storageID: " + transaction.StorageDetailID
                            + " in stock";
                        return false;
                    }
                    else
                    {
                        storageDetail.MaterialTransactions.Add(transaction);
                        storageDetail.CalculateQuantity();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(transaction.PurchaseOrderNumber))
                    {
                        var storageDetail = currentStorages
                        .FirstOrDefault(x => x.PurchaseOrderNumber == transaction.PurchaseOrderNumber &&
                                             x.ID == transaction.StorageDetailID);

                        if (storageDetail == null)
                        {
                            errorMessage = "Can't find PO: " + transaction.PurchaseOrderNumber
                                + "OR storageID: " + transaction.StorageDetailID
                                + " in stock";
                            return false;
                        }
                        else
                        {
                            storageDetail.MaterialTransactions.Add(transaction);
                            storageDetail.CalculateQuantity();
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Update fabric storage from transaction for import output issued
        /// </summary>
        /// <param name="currentStorages"></param>
        /// <param name="transactions"></param>
        /// <param name="storageCode"></param>
        /// <param name="username"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool UpdateFabricStorageFromImportOutputIssued(List<StorageDetail> currentStorages,
            List<MaterialTransaction> transactions, string storageCode,
            string username, out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach (var transaction in transactions)
            {
                var storageDetail = currentStorages
                    .FirstOrDefault(x => (x.FabricPurchaseOrderNumber == transaction.FabricPurchaseOrderNumber &&
                                         (x.ID == transaction.StorageDetailID)));

                if (storageDetail == null)
                {
                    errorMessage = "Can't find item " + transaction.ItemID
                        + "-" + transaction.ItemName
                        + "-" + transaction.ItemColorCode
                        + " in stock";
                    return false;
                }
                else
                {
                    storageDetail.MaterialTransactions.Add(transaction);
                    storageDetail.CalculateQuantity();
                }
            }

            return true;
        }

        /// <summary>
        /// Import storage for clearing
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="currentStorages"></param>
        /// <param name="storageCode"></param>
        /// <param name="username"></param>
        /// <param name="CustomerID"></param>
        /// <param name="newStorageDetails"></param>
        /// <param name="updateStorageDetails"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool ImportFromExcel(string filePath, List<StorageDetail> currentStorages,
            string storageCode, string username, string CustomerID,
            out List<StorageDetail> newStorageDetails, out List<StorageDetail> updateStorageDetails,
            out string errorMessage)
        {
            newStorageDetails = new List<StorageDetail>();
            updateStorageDetails = new List<StorageDetail>();
            errorMessage = string.Empty;

            if (File.Exists(filePath) &&
                Path.GetExtension(filePath).Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets.First();
                        var dataTable = workSheet.ToDataTable();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (!string.IsNullOrEmpty(row["Model FG"]?.ToString()))
                            {
                                var storageDetail = new StorageDetail()
                                {
                                    StorageCode = storageCode,
                                    ItemID = row["Item Code"]?.ToString(),
                                    ItemColorName = row["Component"]?.ToString(),
                                    GarmentSize = row["Garment size"]?.ToString(),
                                    UnitID = row["Unit"]?.ToString(),
                                    CanReUseQuantity = decimal.Parse(row["STOCK"]?.ToString()),
                                    Specify = row["Length/ Color of acc"]?.ToString(),
                                    CustomerStyle = row["IMAN"]?.ToString(),
                                    GarmentColorCode = row["Model FG"]?.ToString(),
                                    CustomerID = CustomerID
                                };

                                var currentStorage = currentStorages
                                    .FirstOrDefault(x => x.ItemID == storageDetail.ItemID &&
                                                         x.ItemName == storageDetail.ItemName &&
                                                         x.ItemColorCode == storageDetail.ItemColorCode &&
                                                         x.ItemColorName == storageDetail.ItemColorName &&
                                                         x.GarmentSize == storageDetail.GarmentSize &&
                                                         x.GarmentColorCode == storageDetail.GarmentColorCode &&
                                                         x.GarmentColorName == storageDetail.GarmentColorName &&
                                                         x.Specify == storageDetail.Specify);

                                if (currentStorage != null)
                                {
                                    currentStorage.CanReUseQuantity += storageDetail.CanReUseQuantity;
                                    updateStorageDetails.Add(currentStorage);
                                }
                                else
                                {
                                    newStorageDetails.Add(currentStorage);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
