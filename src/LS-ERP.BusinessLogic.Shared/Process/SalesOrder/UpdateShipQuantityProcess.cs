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
    public class UpdateShipQuantityProcess
    {
        public static bool UpdateMultiSalesOrder(IQueryable<SalesOrder> salesOrders, string username,
            string filePath, out List<OrderDetail> orderDetails, out List<ItemStyleBarCode> itemStyleBarCodes, out string errorMessage)
        {
            errorMessage = string.Empty;
            orderDetails = new List<OrderDetail>();
            itemStyleBarCodes = new List<ItemStyleBarCode>();

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
                                                .ToList();

                        var updatedSalesOrder = salesOrders.Where(x => salesOrderIDs.Contains(x.ID))
                            .ToList();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var salesOrderID = row["Sales Order"]?.ToString();

                            var salesOrder = updatedSalesOrder
                                .FirstOrDefault(x => x.ID == salesOrderID);

                            if (salesOrder != null)
                            {
                                var lsStyle = row["LS style"]?.ToString();
                                var size = row["Size"]?.ToString();

                                var itemStyle = salesOrder.ItemStyles
                                    .FirstOrDefault(x => x.LSStyle == lsStyle);

                                if (itemStyle == null)
                                {
                                    errorMessage = "Old sales order not have " + lsStyle;
                                    return false;
                                }

                                var orderDetail = itemStyle.OrderDetails
                                    .FirstOrDefault(x =>
                                        x.Size.Replace(" ", "").Trim().ToUpper() == size.Replace(" ", "").Trim().ToUpper());

                                if (orderDetail == null)
                                {
                                    errorMessage = lsStyle + " don't have size " + size;
                                    return false;
                                }

                                if (!string.IsNullOrEmpty(row["Ship qty"]?.ToString()))
                                {
                                    orderDetail.ShipQuantity = (int)decimal.Parse(row["Ship qty"]?.ToString());
                                }

                                orderDetails.Add(orderDetail);

                                //var itemStyleBarcode = itemStyle.Barcodes
                                //    .FirstOrDefault(x =>
                                //        x.Size.Replace(" ", "").Trim().ToUpper() == size.Replace(" ", "").Trim().ToUpper());

                                //if (itemStyleBarcode != null)
                                //{
                                //    itemStyleBarcode.Quantity = (int)decimal.Parse(row["Ship qty"]?.ToString());
                                //    itemStyleBarcode.SetUpdateAudit(username);
                                //    itemStyleBarCodes.Add(itemStyleBarcode);
                                //}
                                itemStyle.SetUpdateAudit(username);
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
