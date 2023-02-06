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
    public class ShippingPlanProcess
    {
        public static List<ShippingPlanDetail> ImportShippingPlan(string customerID, string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            var result = new List<ShippingPlanDetail>();

            switch (customerID)
            {
                case "DE":
                    result = ReadDataDE(filePath, out errorMessage);
                    break;
                case "HA":
                    break;
                default:
                    break;
            }

            return result;
        }

        public static List<ShippingPlanDetail> ReadDataDE(string filePath, out string errorMessage)
        {
            var invoiceNo = "";
            errorMessage = string.Empty;
            var result = new List<ShippingPlanDetail>();

            if (File.Exists(filePath) &&
                Path.GetExtension(filePath).Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets[1];

                        var dataTable = workSheet.ToDataTable(startHeader: 5, startColumn: 2,
                                                endColumn: 47, startRow: 5);

                        //var dataTable = workSheet.ToDataTable(5);

                        var shippingPlanDetails = new List<ShippingPlanDetail>();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            if(string.IsNullOrEmpty(row["PO"]?.ToString()) ||
                                string.IsNullOrEmpty(row["LS STYLE"]?.ToString()))
                            {
                                continue;
                            }
                            
                            if(!string.IsNullOrEmpty(row["INVOICE S&P"]?.ToString()))
                            {
                                invoiceNo = row["INVOICE S&P"]?.ToString();
                            }

                            var shippingPlanDetail = new ShippingPlanDetail()
                            { 
                                PurchaseOrderNumber = row["PO"]?.ToString(),
                                GarmentColorCode = row["MODEL"]?.ToString(),
                                LSStyle = row["LS STYLE"]?.ToString(),
                                CustomerStyle = row["STYLE"]?.ToString(),
                                Destination = row["DESTINATION"]?.ToString(),
                                InvoiceNumber = invoiceNo,
                            };

                            if (!string.IsNullOrEmpty(row[" PCS"]?.ToString()))
                            {
                                var pcsParseResult = int.TryParse(row[" PCS"]?.ToString(), out int pcs);
                                if (pcsParseResult)
                                {
                                    shippingPlanDetail.PCS = pcs;
                                }
                            }

                            if (!string.IsNullOrEmpty(row[" CTNS"]?.ToString()))
                            {
                                var ctnParseResult = int.TryParse(row[" CTNS"]?.ToString(), out int ctn);
                                if (ctnParseResult)
                                {
                                    shippingPlanDetail.CTN = ctn;
                                }
                            }

                            if (!string.IsNullOrEmpty(row["G.W"]?.ToString()))
                            {
                                var grossweightParseResult = decimal.TryParse(row["G.W"]?.ToString(), out decimal grossweight);
                                if (grossweightParseResult)
                                {
                                    shippingPlanDetail.GrossWeight = grossweight;
                                }
                            }

                            if (!string.IsNullOrEmpty(row["AHD"]?.ToString()))
                            {
                                var shipDateParseResult = DateTime.TryParse(row["AHD"]?.ToString(), out DateTime shipDate);
                                if (shipDateParseResult)
                                {
                                    shippingPlanDetail.ShipDate = shipDate;
                                }
                            }

                            if (!string.IsNullOrEmpty(row["VOL"]?.ToString()))
                            {
                                var volumneParseResult = decimal.TryParse(row["VOL"]?.ToString(), out decimal volume);
                                if (volumneParseResult)
                                {
                                    shippingPlanDetail.Volume = volume;
                                }
                            }

                            if (!string.IsNullOrEmpty(row["DEPT"]?.ToString()))
                            {
                                var deptParseResult = decimal.TryParse(row["DEPT"]?.ToString(), out decimal dept);
                                if (deptParseResult)
                                {
                                    shippingPlanDetail.Dept = dept;
                                }
                            }

                            result.Add(shippingPlanDetail);
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
    }
}
