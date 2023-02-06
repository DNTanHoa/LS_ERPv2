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
    public class UpdateCAC_CHD_EHDProcess
    {
        public static bool UpdateMultiSalesOrder(IQueryable<SalesOrder> salesOrders, string username,
            string filePath, out List<ItemStyle> itemStyles, out string errorMessage)
        {
            errorMessage = string.Empty;
            itemStyles = new List<ItemStyle>();

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

                        var salesOrderIDs = dataTable.AsEnumerable()
                                                .Select(r => r.Field<string>("Sales Order"))
                                                .Distinct().ToList();

                        var updatedSalesOrder = salesOrders
                                                .Where(x => salesOrderIDs.Contains(x.ID)).ToList();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var salesOrderID = row["Sales Order"]?.ToString();

                            var salesOrder = updatedSalesOrder
                                .FirstOrDefault(x => x.ID == salesOrderID);

                            if (salesOrder != null)
                            {
                                var lsStyle = row["LS style"]?.ToString();
                                var orderNumner = row["Order"]?.ToString();

                                var itemStyle = salesOrder.ItemStyles
                                    .FirstOrDefault(x => x.LSStyle == lsStyle &&
                                                    x.PurchaseOrderNumber == orderNumner);

                                if (itemStyle == null)
                                {
                                    errorMessage = "Old sales order not have " + lsStyle;
                                    return false;
                                }

                                if(!string.IsNullOrEmpty(row["CAC"]?.ToString()))
                                {
                                    itemStyle.DeliveryPlace = row["CAC"]?.ToString();
                                }

                                if (!string.IsNullOrEmpty(row["CHD"]?.ToString()) &&
                                    DateTime.TryParse(row["CHD"]?.ToString(), out DateTime contractDate))
                                {
                                    itemStyle.ContractDate = contractDate;
                                }

                                if (!string.IsNullOrEmpty(row["EHD"]?.ToString()) &&
                                    DateTime.TryParse(row["EHD"]?.ToString(), out DateTime estimatedDate))
                                {
                                    itemStyle.EstimatedSupplierHandOver = estimatedDate;
                                    itemStyle.ProductionSkedDeliveryDate = estimatedDate;
                                }

                                itemStyle.SetUpdateAudit(username);

                                itemStyles.Add(itemStyle);
                            }
                            else
                            {
                                errorMessage = "Invalid sales order " + salesOrderID
                                    + ". Sales order not exist";
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        errorMessage = "File no data";
                    }
                }
            }
            else
            {
                errorMessage = "File not exist or invalid format";
            }

            return false;
        }
    }
}
