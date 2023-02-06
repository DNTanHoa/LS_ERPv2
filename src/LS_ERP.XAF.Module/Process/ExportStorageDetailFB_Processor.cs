using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using LS_ERP.XAF.Module.DomainComponent;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class ExportStorageDetailFB_Processor
    {
        private static int _maxRow = 2;
        public static Stream CreateExcelFile(List<StorageDetailsReport> storageDetailReports, List<MaterialTransaction> transactionList, Stream stream = null)
        {
            string Author = "Leading Star";
            string Title = storageDetailReports.FirstOrDefault()?.StorageCode ?? "";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                var sheetIndex = 0;
                _maxRow = 2;
                ExcelPackage excel = excelPackage;
                excelPackage.Workbook.Properties.Author = Author;
                CreateStorageDetail(excelPackage, storageDetailReports, transactionList, Title, sheetIndex);


                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        public static void CreateStorageDetail(ExcelPackage excelPackage, List<StorageDetailsReport> storageDetailsReports, List<MaterialTransaction> transactionList, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;

            //var sheetName = storageDetailsReports.FirstOrDefault()?.StorageBinCode ?? sheetIndex.ToString();
            var sheetName = "Sheet1";
            excelPackage.Workbook.Worksheets.Add(sheetName);
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateHeader(workSheet);
            FillData(workSheet, storageDetailsReports,transactionList);

           

            workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
            //workSheet.PrinterSettings.HorizontalCentered = true;
            workSheet.PrinterSettings.PaperSize = ePaperSize.A4;
            //workSheet.PrinterSettings.FitToHeight = true;

            workSheet.PrinterSettings.LeftMargin = 0.25M;
            workSheet.PrinterSettings.RightMargin = 0.25M;
            workSheet.PrinterSettings.TopMargin = 0.5M;
            workSheet.PrinterSettings.BottomMargin = 0.5M;

            // Set the tab color
            workSheet.TabColor = Color.FromArgb(219, 77, 255);
        }
        public static void CreateHeader(ExcelWorksheet workSheet)
        {
            int _currentRow = 1;            
            
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Column(1).Width = 10;
            workSheet.Column(2).Width = 20;
            workSheet.Column(3).Width = 20;
            workSheet.Column(4).Width = 10;
            workSheet.Column(5).Width = 10;
            workSheet.Column(6).Width = 10;
            workSheet.Column(7).Width = 15;
            workSheet.Column(8).Width = 15;
            workSheet.Column(9).Width = 10;
            workSheet.Column(10).Width = 15;
            workSheet.Column(11).Width = 15;
            workSheet.Column(12).Width = 15;
            workSheet.Column(13).Width = 15;
            workSheet.Column(14).Width = 15;
            workSheet.Column(15).Width = 15;
            workSheet.Column(16).Width = 15;

           
            

            workSheet.Row(_currentRow).Height = 20;
            

            workSheet.Cells[_currentRow, 1].Value = "Received Date";
            workSheet.Cells[_currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 1].Style.WrapText = true;


            workSheet.Cells[_currentRow, 2].Value = "Color - size";
            workSheet.Cells[_currentRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 2].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 3].Value = "LOT CARD NO";
            workSheet.Cells[_currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 3].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 4].Value = "LOT NO";
            workSheet.Cells[_currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 4].Style.WrapText = true;

            workSheet.Cells[_currentRow, 5].Value = "Roll No.";
            workSheet.Cells[_currentRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 5].Style.WrapText = true;

            workSheet.Cells[_currentRow, 6].Value = "output roll";
            workSheet.Cells[_currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 6].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 7].Value = "roll stock";
            workSheet.Cells[_currentRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 7].Style.WrapText = true;

            workSheet.Cells[_currentRow, 8].Value = "unit(YDS)";
            workSheet.Cells[_currentRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 8].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 9].Value = "INPUT qty";
            workSheet.Cells[_currentRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 9].Style.WrapText = true;

            workSheet.Cells[_currentRow, 10].Value = "OUTPUT qty";
            workSheet.Cells[_currentRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 10].Style.WrapText = true;

            workSheet.Cells[_currentRow, 11].Value = "AVAILABLE Stock";
            workSheet.Cells[_currentRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 11].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 12].Value = "LOCATION";
            workSheet.Cells[_currentRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 12].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 13].Value = "DK Model Code";
            workSheet.Cells[_currentRow, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 13].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 14].Value = "DK Model Code";
            workSheet.Cells[_currentRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 14].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 15].Value = "FABRIC PO";
            workSheet.Cells[_currentRow, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 15].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 16].Value = "Supplier";
            workSheet.Cells[_currentRow, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 16].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 1, _currentRow, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_currentRow, 1, _currentRow, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
           
            workSheet.Cells[_currentRow, 1, _currentRow, 16].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

        }

        public static void FillData(ExcelWorksheet workSheet,List<StorageDetailsReport> storageDetailsReports, List<MaterialTransaction> transactionList)
        {
            foreach(var storageDetailReport in storageDetailsReports)
            {
                workSheet.Cells[_maxRow, 1].Value = storageDetailReport.TransactionDate;
                workSheet.Cells[_maxRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[_maxRow, 1].Style.Numberformat.Format = "d-MMM-yy";

                workSheet.Cells[_maxRow, 2].Value = storageDetailReport.ItemColorName;
                workSheet.Cells[_maxRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 3].Value = storageDetailReport.DyeLotNumber;
                workSheet.Cells[_maxRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 4].Value = storageDetailReport.LotNumber;
                workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 5].Value = storageDetailReport.RollNo  ;
                workSheet.Cells[_maxRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 6].Value = storageDetailReport.RollOutput;
                workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 7].Value = storageDetailReport.Roll;
                workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 8].Value = storageDetailReport.UnitID;
                workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 9].Value = storageDetailReport.InputQuantity; 
                workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 10].Value = storageDetailReport.OutputQuantity; 
                workSheet.Cells[_maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 11].Value = storageDetailReport.OnHandQuantity; 
                workSheet.Cells[_maxRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 12].Value = storageDetailReport.StorageBinCode; 
                workSheet.Cells[_maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 13].Value = storageDetailReport.ItemID;
                workSheet.Cells[_maxRow, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 14].Value = storageDetailReport.ItemColorCode;
                workSheet.Cells[_maxRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 15].Value = string.IsNullOrEmpty(storageDetailReport.Zone) ? storageDetailReport.FabricPurchaseOrderNumber : storageDetailReport.Zone;
                workSheet.Cells[_maxRow, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 16].Value = storageDetailReport.Supplier;
                workSheet.Cells[_maxRow, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Fill.BackgroundColor.SetColor(Color.White);
           
                _maxRow++;

            }
            workSheet.Cells[_maxRow, 11].Value = storageDetailsReports.Select(s=>s.OnHandQuantity).Sum();  //total Onhand
            workSheet.Cells[_maxRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
       
    }
}
