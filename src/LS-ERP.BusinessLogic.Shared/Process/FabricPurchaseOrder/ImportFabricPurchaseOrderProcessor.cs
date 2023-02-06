using LS_ERP.EntityFrameworkCore.Entities;
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
    public static class ImportFabricPurchaseOrderProcessor
    {
        public static List<FabricPurchaseOrder> Import(string filePath, string subPath, string fileName,
            string userName, string customerID, string productionMethodCode,
            List<FabricPurchaseOrder> oldFabricPurchaseOrders,
            out List<FabricPurchaseOrder> updateFabricPurchaseOrders,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            var result = new List<FabricPurchaseOrder>();
            updateFabricPurchaseOrders = new List<FabricPurchaseOrder>();
            switch (customerID)
            {
                case "DE":
                    {
                        result = ReadDataDE(filePath, subPath, fileName, customerID, userName, productionMethodCode, oldFabricPurchaseOrders,
                            out updateFabricPurchaseOrders,
                            out errorMessage);
                    }
                    break;
            }

            return result;
        }

        public static List<FabricPurchaseOrder> ReadDataDE(string filePath, string subPath,
            string fileName, string customerID, string userName, string productionMethodCode,
            List<FabricPurchaseOrder> oldFabricPurchaseOrders,
            out List<FabricPurchaseOrder> updateFabricPurchaseOrders,
            out string errorMessage)
        {
            var dicOldFabricPO = oldFabricPurchaseOrders.ToDictionary(x => x.Number);
            updateFabricPurchaseOrders = new List<FabricPurchaseOrder>();
            errorMessage = string.Empty;
            var result = new List<FabricPurchaseOrder>();

            if (File.Exists(filePath) &&
                Path.GetExtension(filePath).Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets["FABRIC PO"];
                        var dataTable = workSheet.ToDataTable(3, 1, 31, 3);
                        var workSheetMaster = package.Workbook.Worksheets["MASTER DATA OF FABRIC"];
                        var dataTableMaster = workSheetMaster.ToDataTable(3, 1, 31, 3);

                        var dicFabricMaster = GetMasterDataFabric(dataTableMaster);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            string number = row["ORDER NUMBER"]?.ToString();

                            if (!String.IsNullOrEmpty(number) && (!number.ToLower().Replace(" ", "")
                                                                 .Equals("OrderNumber".ToLower().Replace(" ", "")))
                                                                  && !number.Equals("0"))
                            {

                                if (dicOldFabricPO.TryGetValue(number, out FabricPurchaseOrder rsFB))
                                {
                                    rsFB.ItemID = row["MODEL"]?.ToString();
                                    rsFB.ItemName = row["MODEL NAME"]?.ToString();
                                    rsFB.ItemColorCode = row["ITEM CODE"]?.ToString();
                                    rsFB.ItemColorName = row["ITEM NAME"]?.ToString();
                                    rsFB.ExpectedDeliveryDateWeek = row["EDD WEEK"]?.ToString();
                                    rsFB.Note = row["Note"]?.ToString();
                                    rsFB.Line = row["LINE"]?.ToString();
                                    rsFB.FabricSupplier = row["FABRIC SUPLIER"]?.ToString();
                                    rsFB.CustomerID = customerID;
                                    rsFB.UnitID = "YDS";
                                    rsFB.SupplierContactName = row["SPL NAME"]?.ToString();
                                    rsFB.FileName = fileName;
                                    rsFB.ServerFileName = subPath;
                                    rsFB.FilePath = filePath;

                                    if (!string.IsNullOrEmpty(row["ORDERRED Q.TY"]?.ToString()))
                                    {
                                        var orderedQuantityResult = decimal.TryParse(row["ORDERRED Q.TY"]?.ToString(), out decimal orderedQty);
                                        if (orderedQuantityResult && orderedQty > 0)
                                        {
                                            rsFB.OrderedQuantity = orderedQty;
                                        }
                                    }

                                    if (DateTime.TryParse(row["UPDATED ETD"]?.ToString(), out DateTime updateETD))
                                    {
                                        rsFB.UpdatedEstimatedTimeOfDeparture = updateETD;
                                    }

                                    if (DateTime.TryParse(row["ORIGINAL ETD"]?.ToString(), out DateTime ETD))
                                    {
                                        rsFB.EstimatedTimeOfDeparture = ETD;

                                    }

                                    if (DateTime.TryParse(row["EDD"]?.ToString(), out DateTime EDD))
                                    {
                                        rsFB.ExpectedDeliveryDate = EDD;
                                    }

                                    if (DateTime.TryParse(row["CDD"]?.ToString(), out DateTime CDD))
                                    {
                                        rsFB.ContractualDeliveryDate = CDD;
                                    }

                                    if (DateTime.TryParse(row["ORDER CREATION DATE"]?.ToString(), out DateTime createDate))
                                    {
                                        rsFB.OrderCreationDate = createDate;
                                    }

                                    if (DateTime.TryParse(row["Production starting date_CBA"]?.ToString(), out DateTime productionDate))
                                    {
                                        rsFB.ProductionStartDate = productionDate;
                                    }

                                    if (!string.IsNullOrEmpty(row["ORDERRED Q.TY"]?.ToString()))
                                    {
                                        var orderedQuantityResult = decimal.TryParse(row["ORDERRED Q.TY"]?.ToString(), out decimal orderedQty);
                                        if (orderedQuantityResult && orderedQty > 0)
                                        {
                                            rsFB.OrderedQuantity = orderedQty;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(row["Shipped quantity from CPT Supplier"]?.ToString()))
                                    {
                                        var shippedQtyResult = decimal.TryParse(row["Shipped quantity from CPT Supplier"]?.ToString(), out decimal shippedQty);
                                        if (shippedQtyResult && shippedQty > 0)
                                        {
                                            rsFB.ShippedQuantity = shippedQty;
                                        }
                                    }

                                    if (dicFabricMaster.TryGetValue(rsFB.ItemColorCode, out FabricPurchaseOrder rs))
                                    {
                                        rsFB.Seasons = rs.Seasons;
                                        rsFB.CustomerStyles = rs.CustomerStyles;
                                        rsFB.GarmentColorCodes = rs.GarmentColorCodes;
                                    }

                                    rsFB.SetUpdateAudit(userName);
                                    updateFabricPurchaseOrders.Add(rsFB);
                                }
                                else
                                {
                                    var fabricPurchaseOrder = new FabricPurchaseOrder()
                                    {
                                        Number = number,
                                        ItemID = row["MODEL"]?.ToString(),
                                        ItemName = row["MODEL NAME"]?.ToString(),
                                        ItemColorCode = row["ITEM CODE"]?.ToString(),
                                        ItemColorName = row["ITEM NAME"]?.ToString(),
                                        ExpectedDeliveryDateWeek = row["EDD WEEK"]?.ToString(),
                                        Note = row["Note"]?.ToString(),
                                        Line = row["LINE"]?.ToString(),
                                        FabricSupplier = row["FABRIC SUPLIER"]?.ToString(),
                                        CustomerID = customerID,
                                        UnitID = "YDS",
                                        SupplierContactName = row["SPL NAME"]?.ToString(),
                                        FileName = fileName,
                                        ServerFileName = subPath,
                                        FilePath = filePath,
                                        ProductionMethodCode = productionMethodCode
                                    };

                                    if (!string.IsNullOrEmpty(row["ORDERRED Q.TY"]?.ToString()))
                                    {
                                        var orderedQuantityResult = decimal.TryParse(row["ORDERRED Q.TY"]?.ToString(), out decimal orderedQty);
                                        if (orderedQuantityResult && orderedQty > 0)
                                        {
                                            fabricPurchaseOrder.OrderedQuantity = orderedQty;
                                        }
                                    }

                                    if (DateTime.TryParse(row["UPDATED ETD"]?.ToString(), out DateTime updateETD))
                                    {
                                        fabricPurchaseOrder.UpdatedEstimatedTimeOfDeparture = updateETD;
                                    }

                                    if (DateTime.TryParse(row["ORIGINAL ETD"]?.ToString(), out DateTime ETD))
                                    {
                                        fabricPurchaseOrder.EstimatedTimeOfDeparture = ETD;
                                    }

                                    if (DateTime.TryParse(row["EDD"]?.ToString(), out DateTime EDD))
                                    {
                                        fabricPurchaseOrder.ExpectedDeliveryDate = EDD;
                                    }

                                    if (DateTime.TryParse(row["CDD"]?.ToString(), out DateTime CDD))
                                    {
                                        fabricPurchaseOrder.ContractualDeliveryDate = CDD;
                                    }

                                    if (DateTime.TryParse(row["ORDER CREATION DATE"]?.ToString(), out DateTime createDate))
                                    {
                                        fabricPurchaseOrder.OrderCreationDate = createDate;
                                    }

                                    if (DateTime.TryParse(row["Production starting date_CBA"]?.ToString(), out DateTime productionDate))
                                    {
                                        fabricPurchaseOrder.ProductionStartDate = productionDate;
                                    }

                                    if (!string.IsNullOrEmpty(row["ORDERRED Q.TY"]?.ToString()))
                                    {
                                        var orderedQuantityResult = decimal.TryParse(row["ORDERRED Q.TY"]?.ToString(), out decimal orderedQty);
                                        if (orderedQuantityResult && orderedQty > 0)
                                        {
                                            fabricPurchaseOrder.OrderedQuantity = orderedQty;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(row["Shipped quantity from CPT Supplier"]?.ToString()))
                                    {
                                        var shippedQtyResult = decimal.TryParse(row["Shipped quantity from CPT Supplier"]?.ToString(), out decimal shippedQty);
                                        if (shippedQtyResult && shippedQty > 0)
                                        {
                                            fabricPurchaseOrder.ShippedQuantity = shippedQty;
                                        }
                                    }

                                    if (dicFabricMaster.TryGetValue(fabricPurchaseOrder.ItemColorCode, out FabricPurchaseOrder rs))
                                    {
                                        fabricPurchaseOrder.Seasons = rs.Seasons;
                                        fabricPurchaseOrder.CustomerStyles = rs.CustomerStyles;
                                        fabricPurchaseOrder.GarmentColorCodes = rs.GarmentColorCodes;
                                    }

                                    fabricPurchaseOrder.DeliveredQuantity = fabricPurchaseOrder.ShippedQuantity + fabricPurchaseOrder.ReceivedQuantity;
                                    fabricPurchaseOrder.SetCreateAudit(userName);
                                    dicOldFabricPO[fabricPurchaseOrder.Number] = fabricPurchaseOrder;
                                    fabricPurchaseOrder.UserFollow = fabricPurchaseOrder.CreatedBy;
                                    result.Add(fabricPurchaseOrder);
                                }


                            }
                        }
                    }
                }
            }
            else
            {
                errorMessage = "Invalid file";
            }

            return result;
        }

        public static Dictionary<string, FabricPurchaseOrder> GetMasterDataFabric(DataTable dataTable)
        {
            Dictionary<string, FabricPurchaseOrder> dicFabricMasterData = new Dictionary<string, FabricPurchaseOrder>();

            foreach (DataRow row in dataTable.Rows)
            {
                string number = row["ITEM CODE_FABRIC"]?.ToString();

                if (!String.IsNullOrEmpty(number) && (!number.ToLower().Replace(" ", "")
                                                     .Equals("ITEM CODE_FABRIC".ToLower().Replace(" ", "")))
                                                      && !number.Equals("0"))
                {

                    if (!dicFabricMasterData.ContainsKey(number))
                    {
                        var fabricPurchaseOrder = new FabricPurchaseOrder()
                        {
                            ItemColorCode = number,
                            GarmentColorCodes = row["MODEL"]?.ToString(),
                            CustomerStyles = row["CC"]?.ToString(),
                            Seasons = row["SEASON"]?.ToString(),
                        };

                        dicFabricMasterData[fabricPurchaseOrder.ItemColorCode] = fabricPurchaseOrder;
                    }
                }
            }

            return dicFabricMasterData;
        }

        public static bool CalculateReceiptQuantity(FabricPurchaseOrder fabricPurchaseOrder,
            List<ReceiptGroupLine> receiptGroupLines,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            if (fabricPurchaseOrder != null)
            {
                if (fabricPurchaseOrder.ReceivedQuantity == null)
                    fabricPurchaseOrder.ReceivedQuantity = 0;

                if (fabricPurchaseOrder.OnHandQuantity == null)
                {
                    fabricPurchaseOrder.OnHandQuantity = 0;
                }

                var receivedQty = receiptGroupLines
                    .Where(r => r.FabricPurchaseOrderNumber == fabricPurchaseOrder.Number)
                    .Sum(s => s.ReceiptQuantity);

                fabricPurchaseOrder.ReceivedQuantity += receivedQty;


                fabricPurchaseOrder.DeliveredQuantity = receivedQty + fabricPurchaseOrder.ShippedQuantity;
                fabricPurchaseOrder.OnHandQuantity = receivedQty + fabricPurchaseOrder.OnHandQuantity;
            }

            return true;
        }

        public static bool CalculateIssuedQuantity(List<FabricPurchaseOrder> fabricPurchaseOrders,
            List<StorageDetail> storageDetails,
            List<IssuedGroupLine> issuedGroupLines,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach (var itemFB in fabricPurchaseOrders)
            {
                if (itemFB.OnHandQuantity == null)
                    itemFB.OnHandQuantity = 0;

                if (itemFB.IssuedQuantity == null)
                    itemFB.IssuedQuantity = 0;

                itemFB.IssuedQuantity += issuedGroupLines
                    .Where(r => r.FabricPurchaseOrderNumber == itemFB.Number)
                    .Sum(s => s.IssuedQuantity);

                itemFB.OnHandQuantity = itemFB.OnHandQuantity - itemFB.IssuedQuantity;
            }

            return true;
        }
    }
}
