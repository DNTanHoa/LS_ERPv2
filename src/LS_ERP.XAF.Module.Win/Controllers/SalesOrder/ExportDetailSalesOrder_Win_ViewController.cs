using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.Process;
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
    public class ExportDetailSalesOrder_Win_ViewController : ExportDetailSalesOrder_ViewController
    {
        public override void ExportDetailSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportDetailSalesOrder_Execute(sender, e);
            var export = View.CurrentObject as SalesOrder;

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = export.ID.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

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
                        case "DE":
                        case "HA":
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
                excelPackage.Workbook.Properties.Comments = "Sales Order of Leading Start";
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

    }
}
