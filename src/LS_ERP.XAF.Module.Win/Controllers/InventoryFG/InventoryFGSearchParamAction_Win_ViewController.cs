using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraBars.Ribbon.BackstageView.Accessible;
using DevExpress.XtraCharts;
//using DevExpress.XtraReports.Templates;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
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
using Ultils.Extensions;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class InventoryFGSearchParamAction_Win_ViewController
        : InventoryFGSearchParamAction_ViewController
    {
        public override void ExportInventoryFGAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportInventoryFGAction_Execute(sender, e);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel Files|*.xlsx;*.xls;";

            var viewObject = View.CurrentObject as InventoryFGSearchParam;

            if(viewObject != null)
            {
                dialog.FileName = "Inventory" + DateTime.Now.ToString("yyyyMMdd");
            }

            var inventories = new List<InventoryFG>();
            viewObject.Inventory.ToList().ForEach(i =>
            {
                inventories.Add(i.Inventory);
            });

            //var dataTable = CreateData(viewObject.Inventory);
            var dataTable = CreateData(inventories);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var stream = CreateInventoryStream(dataTable, null);
                var buffer = stream as MemoryStream;
                File.WriteAllBytes(dialog.FileName, buffer.ToArray());

                var message = Message
                    .GetMessageOptions("Export successfully. ", "Successs", InformationType.Success, null, 5000);

                Application.ShowViewStrategy.ShowMessage(message);
            }
        }

        #region Support function

        public DataTable CreateData(List<InventoryFG> items)
        {
            var table = new DataTable();

            table.Columns.Add("Model Code", typeof(string));
            table.Columns.Add("Model Name", typeof(string));
            table.Columns.Add("Item Code", typeof(string));
            table.Columns.Add("Item Name", typeof(string));
            table.Columns.Add("Item Nature", typeof(string));
            table.Columns.Add("On Hand Stock", typeof(int));
            table.Columns.Add("Unit Measurement", typeof(string));
            table.Columns.Add("Unit Price", typeof(decimal));
            table.Columns.Add("Local Currency", typeof(string));
            table.Columns.Add("Stock On-hand Value", typeof(decimal));
            table.Columns.Add("Consolidated Value ($)", typeof(string));
            table.Columns.Add("Stock Life Time (week 35)", typeof(string));
            table.Columns.Add("Supplier Code", typeof(string));
            table.Columns.Add("Supplier Name", typeof(string));
            table.Columns.Add("Stock Update Time", typeof(string));
            table.Columns.Add("Stock Update By", typeof(string));
            table.Columns.Add("Error", typeof(string));

            foreach(var item in items)
            {
                DataRow dr = table.NewRow();
                dr["Model Code"] = item.GarmentColorCode;
                dr["Model Name"] = item.Description;
                dr["Item Code"] = item.ItemCode;
                dr["Item Name"] = item.ItemName;
                dr["Item Nature"] = "FG";
                dr["On Hand Stock"] = item.OnHandQuantity;
                dr["Unit Measurement"] = "PIECE";
                dr["Unit Price"] = item.UnitPrice ?? 0;
                dr["Local Currency"] = "USD";
                dr["Stock On-hand Value"] = (item.OnHandQuantity * item.UnitPrice) ?? 0;

                table.Rows.Add(dr);
            }

            return table;
        }

        public Stream CreateInventoryStream(DataTable data, Stream stream = null)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = "Leading Star ERP system";
                excelPackage.Workbook.Properties.Title = "Sheet0";
                excelPackage.Workbook.Properties.Comments = "Finish good of Leading Star";
                excelPackage.Workbook.Worksheets.Add("Sheet0");
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                workSheet.DefaultColWidth = 18;

                CreateHeader(workSheet);

                workSheet.Cells[2, 1].LoadFromDataTable(data);

                using (var range = workSheet.Cells["F:F"])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                using (var range = workSheet.Cells["H:H"])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public void CreateHeader(ExcelWorksheet workSheet)
        {
            workSheet.Cells[1, 1].Value = "Model Code";
            workSheet.Cells[1, 2].Value = "Model Name";
            workSheet.Cells[1, 3].Value = "Item Code";
            workSheet.Cells[1, 4].Value = "Item Name";
            workSheet.Cells[1, 5].Value = "Item Nature";
            workSheet.Cells[1, 6].Value = "On Hand Stock";
            workSheet.Cells[1, 7].Value = "Unit Measurement";
            workSheet.Cells[1, 8].Value = "Unit Price";
            workSheet.Cells[1, 9].Value = "Local Currency";
            workSheet.Cells[1, 10].Value = "Stock On-hand Value";
            workSheet.Cells[1, 11].Value = "Consolidated Value ($)";
            workSheet.Cells[1, 12].Value = "Stock Life Time (week 35)";
            workSheet.Cells[1, 13].Value = "Supplier Code";
            workSheet.Cells[1, 14].Value = "Supplier Name";
            workSheet.Cells[1, 15].Value = "Stock Update Time";
            workSheet.Cells[1, 16].Value = "Stock Update By";
            workSheet.Cells[1, 17].Value = "Error";
        }

        #endregion
    }
}
