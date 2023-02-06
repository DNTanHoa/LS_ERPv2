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
    public class ImportShipmentProcessor
    {
        public static Shipment Import(string filePath, string subPath, string fileName,
            string userName, string customerID,
            List<Shipment> shipments,
            out string errorMessage)
        {
            Shipment newShipment = new Shipment();
            errorMessage = string.Empty;

            if (!String.IsNullOrEmpty(customerID))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                List<ShipmentDetail> details = new List<ShipmentDetail>();
                List<ShipmentSummary> summaries = new List<ShipmentSummary>();

                if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
                {
                    using (var package = new ExcelPackage(fileInfo))
                    {
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            switch (customerID)
                            {
                                case "PU":
                                    {
                                        var workSheet = package.Workbook.Worksheets["iNet"];
                                        details = ReadDataInet(workSheet, userName, out errorMessage);
                                        var workSheetSummary = package.Workbook.Worksheets["Summary"];
                                        summaries = ReadDataSummary(workSheetSummary, userName, out errorMessage);

                                        newShipment.FilePath = filePath;
                                        newShipment.FileName = fileName;
                                        newShipment.FileNameServer = subPath;
                                        newShipment.CustomerID = customerID;
                                        newShipment.Details = details;
                                        newShipment.Summaries = summaries;
                                        newShipment.SetCreateAudit(userName);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            errorMessage = "Not found sheet";
                        }
                    }
                }
            }
            return newShipment;
        }

        private static List<ShipmentDetail> ReadDataInet(ExcelWorksheet workSheet, string userName, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<ShipmentDetail> details = new List<ShipmentDetail>();

            var dataTable = workSheet.ToDataTable();

            foreach (DataRow row in dataTable.Rows)
            {
                string PONumber = row["P/O No"]?.ToString();

                if (!String.IsNullOrEmpty(PONumber))
                {
                    var shipmentDetail = new ShipmentDetail()
                    {
                        CustomerPurchaseOrderNumber = PONumber,
                        ContractNo = row["Order No"]?.ToString(),
                        DeliveryNo = row["Inv/DelNo"]?.ToString(),
                        MaterialClass = row["Matr Class"]?.ToString(),
                        MaterialCode = row["Material Code"]?.ToString(),
                        Size = row["Size"]?.ToString(),
                        Color = row["Color"]?.ToString(),
                    };

                    if (DateTime.TryParse(row["Trx Date"].ToString(), out DateTime rsTrxDate))
                    {
                        shipmentDetail.TrxDate = rsTrxDate;
                    }

                    if (DateTime.TryParse(row["MRQ"].ToString(), out DateTime MRQ))
                    {
                        shipmentDetail.MRQ = MRQ;
                    }

                    if (decimal.TryParse(row["Received Qty"].ToString().Trim(), out decimal rsReceivedQty))
                    {
                        shipmentDetail.ReceivedQuantity = rsReceivedQty;
                    }

                    if (decimal.TryParse(row["Alc Qty"].ToString().Trim(), out decimal rsAlcQty))
                    {
                        shipmentDetail.AllocQuantity = rsAlcQty;
                    }

                    if (decimal.TryParse(row["AlcRcv"].ToString().Trim(), out decimal rsAlcRcvQty))
                    {
                        shipmentDetail.AllocReceivedQuantity = rsAlcRcvQty;
                    }

                    shipmentDetail.SetCreateAudit(userName);
                    details.Add(shipmentDetail);
                }
            }


            return details;

        }
        private static List<ShipmentSummary> ReadDataSummary(ExcelWorksheet workSheet, string userName, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<ShipmentSummary> summaries = new List<ShipmentSummary>();

            var dataTable = workSheet.ToDataTable(9, 1, 24, 9);

            foreach (DataRow row in dataTable.Rows)
            {
                string PONumber = row["P.O."]?.ToString();

                if (!String.IsNullOrEmpty(PONumber))
                {
                    var shipmentSummary = new ShipmentSummary()
                    {
                        CustomerPurchaseOrderNumber = PONumber,
                        DeliveryNo = row["Inv & Del. No."]?.ToString(),
                        Description = row["Declaration"]?.ToString(),
                        MaterialCode = row["Item"]?.ToString(),
                        Size = row["Size"]?.ToString(),
                        Color = row["Color"]?.ToString(),
                        VendorID = row["Vendor"]?.ToString(),
                        UnitID = row["Uom"]?.ToString()?.ToUpper()
                    };

                    if (DateTime.TryParse(row["WHS D/D"].ToString(), out DateTime rsTrxDate))
                    {
                        shipmentSummary.TrxDate = rsTrxDate;
                    }

                    if (DateTime.TryParse(row["MRQ"].ToString(), out DateTime MRQ))
                    {
                        shipmentSummary.MRQ = MRQ;
                    }

                    if (decimal.TryParse(row["Quantity"].ToString().Trim(), out decimal rsReceivedQty))
                    {
                        shipmentSummary.Quantity = rsReceivedQty;
                    }

                    shipmentSummary.SetCreateAudit(userName);
                    summaries.Add(shipmentSummary);
                }
            }

            return summaries;
        }
    }
}
