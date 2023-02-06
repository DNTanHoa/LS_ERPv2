using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
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
    public class ExportShippingPlanGA_Processor
    {
        private static int _maxRow = 1;
        public static Stream CreateExcelFile(ShippingPlan ShippingPlan, Stream stream = null)
        {
            string Author = "Leading Star";

            if (!String.IsNullOrEmpty(ShippingPlan.CreatedBy))
            {
                Author = ShippingPlan.CreatedBy;
            }

            string Title = ShippingPlan.Title;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                var sheetIndex = 0;
                ExcelPackage excel = excelPackage;
                _maxRow = 1;
                excelPackage.Workbook.Properties.Author = Author;
                CreateShippingPlan(excelPackage, ShippingPlan, Title, sheetIndex);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        private static void CreateShippingPlan(ExcelPackage excelPackage, ShippingPlan ShippingPlan, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "ShippingPlan of Leading Star";
            excelPackage.Workbook.Worksheets.Add("XUAT");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateHeaderShippingPlan(workSheet, ShippingPlan);
            FillDataShippingPlan(workSheet, ShippingPlan);


            //string modelRangeBorder = "A1:" + "J" + _maxRow;

            //using (var range = workSheet.Cells[modelRangeBorder])
            //{
            //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    range.Style.Fill.BackgroundColor.SetColor(Color.White);
            //}


            // Set page break view
            //workSheet.View.PageBreakView = true;
            //workSheet.PrinterSettings.FitToPage = true;
            //workSheet.Row(_maxRow).PageBreak = true;
            //workSheet.Column(10).PageBreak = true;

            //// Set the tab color
            //workSheet.TabColor = Color.FromArgb(219, 77, 255);
        }



        #region ShippingPlan
        private static void CreateHeaderShippingPlan(ExcelWorksheet workSheet, ShippingPlan ShippingPlan)
        {
            var title = ShippingPlan.Company?.Name;

            workSheet.Cells[_maxRow, 1].Value = title.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 16;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Merge = true;

            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 19.8;
            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = ShippingPlan.Title?.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 16;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Merge = true;

            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 19.8;


            workSheet.Column(1).Width = 20;
            workSheet.Column(2).Width = 20;
            workSheet.Column(3).Width = 20;
            workSheet.Column(4).Width = 20;
            workSheet.Column(6).Width = 20;
            workSheet.Column(7).Width = 20;
            workSheet.Column(8).Width = 20;
            workSheet.Column(9).Width = 20;
            workSheet.Column(10).Width =20;
            workSheet.Column(11).Width = 20;
            workSheet.Column(12).Width = 20;
            workSheet.Column(13).Width = 20;
            workSheet.Column(14).Width = 20;

            _maxRow++;

            _maxRow++;

            workSheet.Cells[_maxRow, 1].Value = "PO";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
         

            workSheet.Cells[_maxRow, 2].Value = "LSSTYLE";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 3].Value = "SHEETNAME";
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = "STYLE";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
         

            workSheet.Cells[_maxRow, 5].Value = "CTNS";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
    

            workSheet.Cells[_maxRow, 6].Value = "QTY";
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
          

            workSheet.Cells[_maxRow, 7].Value = "CBM";
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;


            workSheet.Cells[_maxRow, 8].Value = "N.W";
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;
           

            workSheet.Cells[_maxRow, 9].Value = "G.W";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
           

            workSheet.Cells[_maxRow, 10].Value = "ORDER TYPE";
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
          

            workSheet.Cells[_maxRow, 11].Value = "COLOR";
            workSheet.Cells[_maxRow, 11].Style.Font.Bold = true;
          

            workSheet.Cells[_maxRow, 12].Value = "COLOR CODE";
            workSheet.Cells[_maxRow, 12].Style.Font.Bold = true;
           

            workSheet.Cells[_maxRow, 13].Value = "PLACE DELIVERY";
            workSheet.Cells[_maxRow, 13].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 14].Value = "INV NO";
            workSheet.Cells[_maxRow, 14].Style.Font.Bold = true;


            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Fill.BackgroundColor.SetColor(Color.White);

            _maxRow++;

        }
        private static void FillDataShippingPlan(ExcelWorksheet workSheet, ShippingPlan ShippingPlan)
        {
            var listInvoice = ShippingPlan.Details.Select(s => s.InvoiceNumber).Distinct().ToList();
            foreach (var invoice in listInvoice)
            {
                var listShippingPlanDetail = ShippingPlan.Details.Where(x => x.InvoiceNumber == invoice).ToList();
                foreach (var shippingPlanDetail in listShippingPlanDetail)
                {
                    fillRow(workSheet, shippingPlanDetail);
                }
                fillRowTotal(workSheet, listShippingPlanDetail);
            }
            fillGrandRowTotal(workSheet, ShippingPlan.Details);
         
        }

        public static void fillRow(ExcelWorksheet workSheet, ShippingPlanDetail shippingPlanDetail)
        {
            workSheet.Cells[_maxRow, 1].Value = shippingPlanDetail.PurchaseOrderNumber;



            workSheet.Cells[_maxRow, 2].Value = shippingPlanDetail.LSStyle;

            workSheet.Cells[_maxRow, 3].Value = shippingPlanDetail.SheetName;

            workSheet.Cells[_maxRow, 4].Value = shippingPlanDetail.CustomerStyle;


            workSheet.Cells[_maxRow, 5].Value = shippingPlanDetail.CTN;   
            
        
            workSheet.Cells[_maxRow, 6].Value = shippingPlanDetail.PCS;
       
            workSheet.Cells[_maxRow, 7].Value = shippingPlanDetail.Volume;

            workSheet.Cells[_maxRow, 8].Value = "";//shippingPlanDetail.GrossWeight;

            workSheet.Cells[_maxRow, 9].Value = shippingPlanDetail.GrossWeight;     

            workSheet.Cells[_maxRow, 10].Value = shippingPlanDetail.OrderType;     

            workSheet.Cells[_maxRow, 11].Value = shippingPlanDetail.Color;
       
            workSheet.Cells[_maxRow, 12].Value = shippingPlanDetail.GarmentColorCode;

            workSheet.Cells[_maxRow, 13].Value = shippingPlanDetail.Destination;

            workSheet.Cells[_maxRow, 14].Value = shippingPlanDetail.InvoiceNumber;

            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Fill.BackgroundColor.SetColor(Color.White);
            _maxRow++;
        }
        public static void fillRowTotal(ExcelWorksheet workSheet, List<ShippingPlanDetail> shippingPlanDetail)
        {
            workSheet.Cells[_maxRow, 5].Value = shippingPlanDetail.Select(s => s.CTN).Sum();  //CTNS
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 6].Value = shippingPlanDetail.Select(s => s.PCS).Sum();  // PCS
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true; 
           
            workSheet.Cells[_maxRow, 7].Value = shippingPlanDetail.Select(s => s.Volume).Sum(); // VOL
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 9].Value = shippingPlanDetail.Select(s => s.GrossWeight).Sum(); //G.W
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));
            _maxRow++;
        }

        public static void fillGrandRowTotal(ExcelWorksheet workSheet, List<ShippingPlanDetail> shippingPlanDetail)
        {
            workSheet.Cells[_maxRow, 5].Value = shippingPlanDetail.Select(s => s.CTN).Sum();  //CTNS
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 6].Value = shippingPlanDetail.Select(s => s.PCS).Sum();  // PCS
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 7].Value = shippingPlanDetail.Select(s => s.Volume).Sum(); // VOL
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 9].Value = shippingPlanDetail.Select(s => s.GrossWeight).Sum(); //G.W
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;


            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 14].Style.Fill.BackgroundColor.SetColor(Color.Orange);
            _maxRow++;
        }

        #endregion ShippingPlan
    }
}

    
