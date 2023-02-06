using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
using LS_ERP.XAF.Module.DomainComponent;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
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
    public class ExportSalesContract_Win_ViewController : ExportSalesContract_ViewController
    {
        public override void ExportSalesContract_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportSalesContract_Execute(sender, e);
            var export = View.CurrentObject as SalesContract;

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = export.Number.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var stream = CreateExcelFile(export);
                    var buffer = stream as MemoryStream;

                    File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                    options = Message.GetMessageOptions("Export successfully. ", "Successs",
                   InformationType.Success, null, 5000);
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

        private Stream CreateExcelFile(SalesContract salesContract, Stream stream = null)
        {
            DataTable table = SetData(salesContract);

            string Author = "Leading Star";

            if (!String.IsNullOrEmpty(salesContract.CreatedBy))
            {
                Author = salesContract.CreatedBy;
            }

            string Title = salesContract.Number;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Sales contract of Leading Start";
                excelPackage.Workbook.Worksheets.Add(Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                workSheet.DefaultColWidth = 12;

                CreateHeader(workSheet, Title);

                workSheet.Cells[3, 1].LoadFromDataTable(table);

                string modelRangeBorder = "A2:AG" + (table.Rows.Count + 2).ToString();
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
                using (var range = workSheet.Cells["A2:AG2"])
                {
                    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }

        }

        public void CreateHeader(ExcelWorksheet workSheet, string Title)
        {
            workSheet.Cells[1, 2].Value = "CONTRACT: ";
            workSheet.Cells[1, 2].Style.Font.Bold = true;

            workSheet.Cells[1, 3].Value = Title;
            workSheet.Cells[1, 3].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "Contract No";
            workSheet.Cells[2, 2].Value = "LS Style";
            workSheet.Cells[2, 3].Value = "Cust PO";
            workSheet.Cells[2, 4].Value = "Style No";
            workSheet.Cells[2, 5].Value = "Color";
            workSheet.Cells[2, 6].Value = "Qty";
            workSheet.Cells[2, 7].Value = "Unit";
            workSheet.Cells[2, 8].Value = "Country Name";
            workSheet.Cells[2, 9].Value = "Contract Date";
            workSheet.Cells[2, 10].Value = "Year";
            workSheet.Cells[2, 11].Value = "Brand";
            workSheet.Cells[2, 12].Value = "Season";
            workSheet.Cells[2, 13].Value = "Division";
            workSheet.Cells[2, 14].Value = "Product(Top)";
            workSheet.Cells[2, 15].Value = "Product(Bottom)";
            workSheet.Cells[2, 16].Value = "PO No.";
            workSheet.Cells[2, 17].Value = "MRQ Date";
            workSheet.Cells[2, 18].Value = "Factory Date";
            workSheet.Cells[2, 19].Value = "Req Ship Mth";
            workSheet.Cells[2, 20].Value = "Job Order Ready Date";
            workSheet.Cells[2, 21].Value = "No of Emb";
            workSheet.Cells[2, 22].Value = "No of Transfer Print";
            workSheet.Cells[2, 23].Value = "No of Screen Print";
            workSheet.Cells[2, 24].Value = "No of Welding";
            workSheet.Cells[2, 25].Value = "No of Pad Print";
            workSheet.Cells[2, 26].Value = "No of Laser";
            workSheet.Cells[2, 27].Value = "Gmt  Lead  Time";
            workSheet.Cells[2, 28].Value = "Mfr Lead Time";
            workSheet.Cells[2, 29].Value = "Longest Matl Lead time";
            workSheet.Cells[2, 30].Value = "Transit Lead time";
            workSheet.Cells[2, 31].Value = "Shipping Mark";
            workSheet.Cells[2, 32].Value = "Remarks";
            workSheet.Cells[2, 33].Value = "Updates";
        }

        public DataTable SetData(SalesContract salesContract)
        {
            DataTable table = new DataTable();
            for (int i = 0; i <= 32; i++)
            {
                DataColumn column = new DataColumn("Column" + i);
                table.Columns.Add(column);
            }

            foreach (var contractDetail in salesContract.ContractDetails)
            {
                DataRow row = table.NewRow();
                row["Column0"] = contractDetail.ContractNo;
                row["Column1"] = contractDetail.LSStyle;
                row["Column2"] = contractDetail.CustomerPO;
                row["Column3"] = contractDetail.CustomerStyle;
                row["Column4"] = contractDetail.GarmentColorCode;
                row["Column5"] = (int)contractDetail.Quantity;
                row["Column6"] = contractDetail.UnitID;
                row["Column7"] = contractDetail.CountryName;
                row["Column8"] = contractDetail.OrderPlacedDate?.ToShortDateString();
                row["Column9"] = contractDetail.Year;
                row["Column10"] = contractDetail.Brand;
                row["Column11"] = contractDetail.Season;
                row["Column12"] = contractDetail.Division;
                row["Column13"] = contractDetail.ProductTop;
                row["Column14"] = contractDetail.ProductBottom;
                row["Column15"] = contractDetail.PurchaseOrderNumber;
                row["Column16"] = contractDetail.MRQDate?.ToShortDateString();
                row["Column17"] = contractDetail.FactoryDate?.ToShortDateString();
                row["Column18"] = contractDetail.ReqShipMonth;
                row["Column19"] = "//";
                row["Column20"] = contractDetail.Emboss;
                row["Column21"] = contractDetail.Transfer;
                row["Column22"] = contractDetail.Screen;
                row["Column23"] = contractDetail.Bonding;
                row["Column24"] = contractDetail.Pad;
                row["Column25"] = contractDetail.Lazer;
                row["Column26"] = contractDetail.GmtLeadTime;
                row["Column27"] = contractDetail.MfrLeadTime;
                row["Column28"] = contractDetail.LongestMaterialLeadTime;
                row["Column29"] = contractDetail.TransitLeadTime;
                row["Column30"] = contractDetail.ShippingMark;
                row["Column31"] = contractDetail.Remark;
                row["Column32"] = contractDetail.Updates;

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
