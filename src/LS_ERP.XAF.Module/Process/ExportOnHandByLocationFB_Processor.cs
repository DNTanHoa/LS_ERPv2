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
    public class ExportOnHandByLocationFB_Processor
    {
        private static int _maxRow = 1;
        public static Stream CreateExcelFile(List<StorageDetailsReport> storageDetailReports, List<MaterialTransaction> transactionList, Stream stream = null)
        {
            string Author = "Leading Star";
            string Title = storageDetailReports.FirstOrDefault()?.StorageCode ?? "";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                var sheetIndex = 0;
                ExcelPackage excel = excelPackage;
                excelPackage.Workbook.Properties.Author = Author;                
                var storageBinCodes = storageDetailReports.Select(s => s.StorageBinCode.ToUpper()).Distinct().ToList();
                storageBinCodes.Sort();
                foreach(var StorageBinCode in storageBinCodes)
                {
                    _maxRow = 3;
                    var exportStorageDetailReports = storageDetailReports.Where(s=>s.StorageBinCode == StorageBinCode).OrderBy(o=>o.DyeLotNumber).ToList();
                    var exportMaterialTransaction = transactionList.Where(t=>
                                                        exportStorageDetailReports.Select(s=>s.ID).ToList().Contains((long)t.StorageDetailID)).ToList();
                    CreateOnhand(excelPackage, exportStorageDetailReports, exportMaterialTransaction, Title, sheetIndex++);
                }  
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        public static void CreateOnhand(ExcelPackage excelPackage, List<StorageDetailsReport> storageDetailsReports, List<MaterialTransaction> transactionList, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            
            var sheetName = storageDetailsReports.FirstOrDefault()?.StorageBinCode ?? sheetIndex.ToString();
            excelPackage.Workbook.Worksheets.Add(sheetName);
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateHeaderOnhand(workSheet);
            FillDataOnhand(workSheet, storageDetailsReports,transactionList);

            //string modelRangeBorder = "A1:" + "J" + _maxRow;

            //using (var range = workSheet.Cells[modelRangeBorder])
            //{
            //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    range.Style.Fill.BackgroundColor.SetColor(Color.White);
            //}


            // Set page break view
            //workSheet.View.PageBreakView = true;
            
            //workSheet.Row(_maxRow).PageBreak = true;
            //workSheet.Column(10).PageBreak = true;

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
        public static void CreateHeaderOnhand(ExcelWorksheet workSheet)
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
            workSheet.Column(10).Width = 10;
            workSheet.Column(11).Width = 10;

            workSheet.Cells[1, 1].Value = "THÔNG TIN HÀNG NHẬP";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 11;
            workSheet.Cells[1, 1, 1, 7].Merge = true; // range column A -> G
            workSheet.Cells[1, 1, 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1, 1, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            

            workSheet.Cells[1, 9].Value = "THÔNG TIN HÀNG XUẤT RA";
            workSheet.Cells[1, 9].Style.Font.Bold = true;
            workSheet.Cells[1, 9].Style.Font.Size = 11;
            workSheet.Cells[1, 9, 1, 11].Merge = true; // range column I -> K
            workSheet.Cells[1, 9, 1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 9, 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            workSheet.Row(_currentRow).Height = 20;
            _currentRow++;

            workSheet.Cells[_currentRow, 1].Value = "Received Date";
            workSheet.Cells[_currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 1].Style.WrapText = true;


            workSheet.Cells[_currentRow, 2].Value = "Color - size";
            workSheet.Cells[_currentRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 2].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 3].Value = "Lot";
            workSheet.Cells[_currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 3].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 4].Value = "Roll stock";
            workSheet.Cells[_currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 4].Style.WrapText = true;

            workSheet.Cells[_currentRow, 5].Value = "AVAILABLE Stock (yard/met)";
            workSheet.Cells[_currentRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 5].Style.WrapText = true;

            workSheet.Cells[_currentRow, 6].Value = "Location";
            workSheet.Cells[_currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 6].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 7].Value = "DK Model Code";
            workSheet.Cells[_currentRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 7].Style.WrapText = true;

            workSheet.Cells[_currentRow, 8].Value = "FABRIC PO";
            workSheet.Cells[_currentRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 8].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 9].Value = "Exported rolls";
            workSheet.Cells[_currentRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 9].Style.WrapText = true;

            workSheet.Cells[_currentRow, 10].Value = "Leftover roll";
            workSheet.Cells[_currentRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 10].Style.WrapText = true;

            workSheet.Cells[_currentRow, 11].Value = "Remark";
            workSheet.Cells[_currentRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_currentRow, 11].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 1, _currentRow, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_currentRow, 1, _currentRow, 11].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 11].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //workSheet.Cells[_currentRow, 1, _currentRow, 11].Style.Fill.BackgroundColor.SetColor(Color.White);
            workSheet.Cells[_currentRow, 1, _currentRow, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

        }

        public static void FillDataOnhand(ExcelWorksheet workSheet,List<StorageDetailsReport> storageDetailsReports, List<MaterialTransaction> transactionList)
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

                workSheet.Cells[_maxRow, 4].Value = transactionList.Where(t => t.StorageDetailID == storageDetailReport.ID && t.IssuedNumber == null).Select(s=>s.Roll).Sum();
                workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 5].Value = storageDetailReport.OnHandQuantity; // roll stock
                workSheet.Cells[_maxRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 6].Value = storageDetailReport.StorageBinCode;
                workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 7].Value = storageDetailReport.ItemID;
                workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 8].Value = string.IsNullOrEmpty(storageDetailReport.Zone) ?  storageDetailReport.FabricPurchaseOrderNumber : storageDetailReport.Zone;
                workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 9].Value = ""; // export rolls
                workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 10].Value = ""; //storageDetailReport.Roll; // leftover rolls
                workSheet.Cells[_maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 11].Value = ""; // storageDetailReport.Remark; //remark
                workSheet.Cells[_maxRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Fill.BackgroundColor.SetColor(Color.White);
                //Fill list issue roll
                //var transactionsIsssue = transactionList.Where(t=>t.StorageDetailID == storageDetailReport.ID && t.IssuedNumber != null).ToList();
                //foreach(var transaction in transactionsIsssue)
                //{
                //    workSheet.Cells[_maxRow, 9].Value = transaction.Roll; // export rolls
                //    workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    _maxRow++;
                //}    
                ////
                //if(transactionsIsssue.Count ==0)
                //{
                //    _maxRow++;
                //}
                _maxRow++;

            }
        }
       
    }
}
