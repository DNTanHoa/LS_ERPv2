using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Process;
using LS_ERP.XAF.Module.Win.Dtos;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ExportSalesOrder_Win_ViewController : ExportSalesOrder_ViewController
    {
        public override void ExportSalesOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            base.ExportSalesOrder_Execute(sender, e);
            var exportParam = e.PopupWindowViewCurrentObject as SalesOrderExportParam;

            var export = View.CurrentObject as SalesOrder;

            SaveFileDialog dialog = new SaveFileDialog();

            switch (export.CustomerID)
            {
                case "HA":
                    dialog.FileName = "SO.HADDAD." + exportParam.ShipDate?.ToString("MM") + "." + exportParam.ShipDate?.ToString("yyyy");
                    break;
                default:
                    dialog.FileName = export.ID.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');
                    break;
            }


            dialog.Filter = "Excel Files|*.xlsx;*.xls;";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (export.CustomerID)
                    {
                        case "PU":
                            {
                                var stream = CreateExcelFile_PU(export);
                                var buffer = stream as MemoryStream;

                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                        case "HA":
                            {
                                string SOID = "SO.HADDAD.";

                                //var criteria = CriteriaOperator.Parse("([SalesOrderID] LIKE '%"+SOID+"%') " +
                                //    "AND ([ShipDate] <= #" + exportParam.ShipDate?.ToString("yyyy-MM") + "-" + DateTime.DaysInMonth(exportParam.ShipDate?.Year ?? 1990, exportParam.ShipDate?.Month ?? 1) + "#) " +
                                //    "AND ([ShipDate] >= #" + exportParam.ShipDate?.ToString("yyyy-MM") + "-01#) AND [ItemStyleStatusCode] <> '3'",
                                //        exportParam.ShipDate, exportParam.ShipDate);

                                var criteria = CriteriaOperator.Parse("([SalesOrderID] LIKE '" + SOID + "%') " +
                                    "AND (GetMonth([ShipDate]) = " + exportParam.ShipDate?.ToString("MM") + ") AND [ItemStyleStatusCode] <> '3'");

                                var itemStyles = ObjectSpace.GetObjects<ItemStyle>(criteria).OrderBy(x => x.ContractNo)
                                                                                            .OrderBy(x => x.CustomerStyle)
                                                                                            .OrderBy(x => x.ColorCode)
                                                                                            .OrderBy(x => x.PurchaseOrderNumber);


                                var stream = SalesOrderProcess.CreateExcelFile(export, itemStyles?.ToList());
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                        case "DE":
                            {
                                var stream = SalesOrderProcess.CreateExcelFile(export);
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                    }

                    options = Message.GetMessageOptions("Export successfully. ", "Successs", InformationType.Success, null, 5000);
                }
                catch (Exception EE)
                {
                    options = Message.GetMessageOptions("Export failed. " + EE.Message, "Error",
                   InformationType.Error, null, 5000);

                }
                Application.ShowViewStrategy.ShowMessage(options);
                View.Refresh();
            }
        }

        private Stream CreateExcelFile_PU(SalesOrder salesOrder, Stream stream = null)
        {

            DataTable table = SetData_PU(salesOrder);

            string Author = "Leading Star";

            if (!String.IsNullOrEmpty(salesOrder.CreatedBy))
            {
                Author = salesOrder.CreatedBy;

            }

            string Title = salesOrder.ID;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {

                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Sales Order of Leading Star";
                excelPackage.Workbook.Worksheets.Add(Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                workSheet.DefaultColWidth = 12;

                CreateHeader_PU(workSheet, Title);

                workSheet.Cells[3, 1].LoadFromDataTable(table);


                string modelRangeBorder = "A2:T" + (table.Rows.Count + 2).ToString();
                using (var range = workSheet.Cells[modelRangeBorder])
                {
                    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.AutoFitColumns();
                }

                //string modelRange = "A1:T1";
                using (var range = workSheet.Cells["A2:T2"])
                {
                    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                string modelNumeric = "A3:T" + (table.Rows.Count + 2).ToString();
                using (var range = workSheet.Cells[modelNumeric])
                {
                    //range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }

        }

        public void CreateHeader_PU(ExcelWorksheet workSheet, string Title)
        {
            workSheet.Cells[1, 1].Value = "Order No: ";
            workSheet.Cells[1, 1].Style.Font.Bold = true;

            workSheet.Cells[1, 2].Value = Title;
            workSheet.Cells[1, 2].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "LS Style";
            workSheet.Cells[2, 2].Value = "Customer Style";
            workSheet.Cells[2, 3].Value = "Description";
            workSheet.Cells[2, 4].Value = "Season";
            workSheet.Cells[2, 5].Value = "Order PO";
            workSheet.Cells[2, 6].Value = "Delivery Place";
            workSheet.Cells[2, 7].Value = "Delivery Date";
            workSheet.Cells[2, 8].Value = "Contract No";
            workSheet.Cells[2, 9].Value = "Contract Date";
            workSheet.Cells[2, 10].Value = "PO Type";
            workSheet.Cells[2, 11].Value = "Ship Mode";
            workSheet.Cells[2, 12].Value = "CustCode";
            workSheet.Cells[2, 13].Value = "UCustCode";
            workSheet.Cells[2, 14].Value = "CustCoNo";
            workSheet.Cells[2, 15].Value = "UCustCoNo";
            workSheet.Cells[2, 16].Value = "Color";
            workSheet.Cells[2, 17].Value = "Color Name";
            workSheet.Cells[2, 18].Value = "Size";
            workSheet.Cells[2, 19].Value = "Qty";
            workSheet.Cells[2, 20].Value = "Price";

        }

        public DataTable SetData_PU(SalesOrder salesOrder)
        {
            DataTable table = new DataTable();
            for (int i = 0; i <= 32; i++)
            {
                DataColumn column = new DataColumn("Column" + i);
                table.Columns.Add(column);
            }

            foreach (var itemStyle in salesOrder.ItemStyles)
            {
                var orderDetails = itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex);

                foreach (var orderDetail in orderDetails)
                {
                    DataRow row = table.NewRow();
                    row["Column0"] = itemStyle.LSStyle;
                    row["Column1"] = itemStyle.CustomerStyle;
                    row["Column2"] = itemStyle.Description;
                    row["Column3"] = itemStyle.Season;
                    row["Column4"] = itemStyle.PurchaseOrderNumber;
                    row["Column5"] = itemStyle.DeliveryPlace;
                    row["Column6"] = itemStyle.DeliveryDate?.ToShortDateString();
                    row["Column7"] = itemStyle.ContractNo;
                    row["Column8"] = itemStyle.ContractDate?.ToShortDateString();
                    row["Column9"] = itemStyle.PurchaseOrderTypeCode;
                    row["Column10"] = itemStyle.ShipMode;
                    row["Column11"] = itemStyle.CustomerCode;
                    row["Column12"] = itemStyle.UCustomerCode;
                    row["Column13"] = itemStyle.CustomerCodeNo;
                    row["Column14"] = itemStyle.UCustomerCodeNo;
                    row["Column15"] = itemStyle.ColorCode;
                    row["Column16"] = itemStyle.ColorName;
                    row["Column17"] = orderDetail.Size;
                    row["Column18"] = orderDetail.Quantity?.ToString("G29");
                    row["Column19"] = float.Parse(orderDetail.Price?.ToString("G29"));
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        private Stream CreateExcelFile_DE(SalesOrder salesOrder, Stream stream = null)
        {

            int row = 2;

            List<ExportQuantityDE_Dto> list = ListData_DE(salesOrder, out row);

            string Author = "Admin LS";

            if (!String.IsNullOrEmpty(salesOrder.CreatedBy))
            {
                Author = salesOrder.CreatedBy;

            }

            string Title = salesOrder.ID;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {

                ExcelPackage excel = excelPackage;


                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Sale order of Leading Start";
                excelPackage.Workbook.Worksheets.Add("Sheet_" + Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                workSheet.DefaultColWidth = 12;

                CreateHeader_DE(workSheet, Title);

                workSheet.Cells[3, 1].LoadFromCollection(list, false);

                string modelRangeOrderDate = "B3:B" + row.ToString();
                using (var range = workSheet.Cells[modelRangeOrderDate])
                {
                    range.Style.Numberformat.Format = "dd-MMM-yyyy";
                }

                string modelRangeCHD = "E3:E" + row.ToString();
                using (var range = workSheet.Cells[modelRangeCHD])
                {
                    range.Style.Numberformat.Format = "dd-MMM-yyyy";
                }

                string modelRangeDeliveryDate = "S3:S" + row.ToString();
                using (var range = workSheet.Cells[modelRangeDeliveryDate])
                {
                    range.Style.Numberformat.Format = "dd-MMM-yyyy";
                }

                string modelRangeEHD = "T3:T" + row.ToString();
                using (var range = workSheet.Cells[modelRangeEHD])
                {
                    range.Style.Numberformat.Format = "dd-MMM-yyyy";
                }

                string modelRangeBorder = "A2:T" + row.ToString();
                using (var range = workSheet.Cells[modelRangeBorder])
                {
                    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.AutoFitColumns();
                }

                //string modelRange = "A2:T2";
                using (var range = workSheet.Cells["A2:T2"])
                {
                    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                //foreach (var item in _arrChangeRow)
                //{
                //    using (var range = workSheet.Cells["A" + item + ":T" + item])
                //    {
                //        range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                //        //range.Style.Font.Bold = true;
                //        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 182, 193));
                //        //range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //    }
                //}

                excelPackage.Save();
                return excelPackage.Stream;
            }

        }

        public void CreateHeader_DE(ExcelWorksheet workSheet, string Title)
        {
            workSheet.Cells[1, 1].Value = "Order No: ";
            workSheet.Cells[1, 1].Style.Font.Bold = true;

            workSheet.Cells[1, 2].Value = Title;
            workSheet.Cells[1, 2].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "LS Style";
            workSheet.Cells[2, 2].Value = "Order Creation Date";
            workSheet.Cells[2, 3].Value = "Order";
            workSheet.Cells[2, 4].Value = "Production Line";
            workSheet.Cells[2, 5].Value = "Contractual Supplier Handover";
            workSheet.Cells[2, 6].Value = "Order Type";
            workSheet.Cells[2, 7].Value = "Model";
            workSheet.Cells[2, 8].Value = "Designation";
            workSheet.Cells[2, 9].Value = "Item";
            workSheet.Cells[2, 10].Value = "Size";
            workSheet.Cells[2, 11].Value = "PCB";
            workSheet.Cells[2, 12].Value = "UE";
            workSheet.Cells[2, 13].Value = "Packaging";
            workSheet.Cells[2, 14].Value = "IMAN Code";
            workSheet.Cells[2, 15].Value = "Ordered Qty";
            workSheet.Cells[2, 16].Value = "Unit Price";
            workSheet.Cells[2, 17].Value = "Delivery Place";
            workSheet.Cells[2, 18].Value = "Order Status";
            workSheet.Cells[2, 19].Value = "Contractual Delivery Date";
            workSheet.Cells[2, 20].Value = "Estimated Supplier Handover";
        }

        public List<ExportQuantityDE_Dto> ListData_DE(SalesOrder salesOrder, out int row)
        {
            List<ExportQuantityDE_Dto> list = new List<ExportQuantityDE_Dto>();

            row = 2;

            foreach (ItemStyle item in salesOrder.ItemStyles)
            {

                foreach (var orderDetail in item.OrderDetails)
                {
                    ExportQuantityDE_Dto exportSaleDTO = new ExportQuantityDE_Dto();

                    exportSaleDTO.LSStyle = item.LSStyle;
                    exportSaleDTO.OrderDate = item.PurchaseOrderDate;
                    exportSaleDTO.OrderID = item.Number;
                    exportSaleDTO.ProductionLine = item.PIC;
                    exportSaleDTO.ContactSupplierHandover = item.ContractDate;
                    exportSaleDTO.OrderType = item.PurchaseOrderTypeCode;

                    exportSaleDTO.Designation = item.Description;
                    exportSaleDTO.IMan = item.CustomerStyle;
                    exportSaleDTO.Size = orderDetail.Size;

                    var barcode = item.Barcodes.FirstOrDefault(x => x.Size.ToUpper().Replace(" ", "").Trim() ==
                                                                    orderDetail.Size.ToUpper().Replace(" ", "").Trim());
                    if (barcode != null)
                    {
                        exportSaleDTO.Item = barcode.BarCode;
                        exportSaleDTO.PCB = barcode.PCB;
                        exportSaleDTO.UE = barcode.UE;
                        exportSaleDTO.Packing = barcode.Packing;
                    }


                    exportSaleDTO.Model = item.ColorCode;


                    exportSaleDTO.OrderQuantity = orderDetail.Quantity;
                    exportSaleDTO.UnitPrice = orderDetail.Price;


                    exportSaleDTO.DeliveryPlace = item.DeliveryPlace;
                    exportSaleDTO.OrderStatus = item.PurchaseOrderStatusCode;
                    exportSaleDTO.ContractualDeliveryDate = item.DeliveryDate;
                    exportSaleDTO.EstimatedSupplierHandover = item.EstimatedSupplierHandOver;

                    list.Add(exportSaleDTO);

                    row++;
                }
            }

            return list;
        }
    }
}
