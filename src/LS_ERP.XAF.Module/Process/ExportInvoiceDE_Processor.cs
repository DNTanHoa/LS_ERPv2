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
    public class ExportInvoiceDE_Processor
    {
        private static int _maxRow = 1;
        public static Stream CreateExcelFile(List<Invoice> invoices, Stream stream = null)
        {
            string Author = "Leading Star";

            if (!String.IsNullOrEmpty(invoices.FirstOrDefault().CreatedBy))
            {
                Author = invoices.FirstOrDefault().CreatedBy;
            }

            string Title = invoices.FirstOrDefault().Code;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                var sheetIndex = 0;
                ExcelPackage excel = excelPackage;
                // Param List<Invoice>() => Item => Length List<Invoice>()
                for (int i = 0; i < invoices.Count; i++)
                {
                    _maxRow = 1;
                    excelPackage.Workbook.Properties.Author = Author;
                    CreateInvoice(excelPackage, invoices[i], Title, sheetIndex);

                    _maxRow = 1;
                    sheetIndex++;
                    CreatePackingList(excelPackage, invoices[i], Title, sheetIndex);
                    sheetIndex++;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public static Stream CreatePaymentExcelFile(List<Invoice> invoices, List<ProductionDept> productionDepts, Stream stream = null)
        {
            string Author = "Leading Star";

            if (!String.IsNullOrEmpty(invoices.FirstOrDefault().CreatedBy))
            {
                Author = invoices.FirstOrDefault().CreatedBy;
            }

            string Title = invoices.FirstOrDefault().Code + "-PAYMENT";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                var sheetIndex = 0;
                ExcelPackage excel = excelPackage;
                // Param List<Invoice>() => Item => Length List<Invoice>()
                for (int i = 0; i < invoices.Count; i++)
                {
                    _maxRow = 1;
                    excelPackage.Workbook.Properties.Author = Author;
                    CreateInvoicePayment(excelPackage, invoices[i], productionDepts, Title, sheetIndex);
                    sheetIndex++;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        private static void CreateInvoice(ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Star";
            excelPackage.Workbook.Worksheets.Add("INV");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateHeaderInvoice(workSheet, invoice);
            FillDataInvoice(workSheet, invoice);

            string modelRangeBorder = "A1:" + "J" + _maxRow;

            using (var range = workSheet.Cells[modelRangeBorder])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White);
            }


            // Set page break view
            workSheet.View.PageBreakView = true;
            workSheet.PrinterSettings.FitToPage = true;
            workSheet.Row(_maxRow).PageBreak = true;
            workSheet.Column(10).PageBreak = true;

            // Set the tab color
            workSheet.TabColor = Color.FromArgb(219, 77, 255);
        }
        private static void CreatePackingList(ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Start";
            excelPackage.Workbook.Worksheets.Add("PL");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            //workSheet.DefaultRowHeight = 15.6;
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            CreateHeaderPL(workSheet, invoice);
            FillDataPL(workSheet, invoice);

            string modelRangeBorder = "A1:L" + _maxRow;

            using (var range = workSheet.Cells[modelRangeBorder])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White);
            }

            // Set the tab color
            // Set page break view
            workSheet.View.PageBreakView = true;
            workSheet.PrinterSettings.FitToPage = true;
            workSheet.Row(_maxRow).PageBreak = true;
            workSheet.Column(12).PageBreak = true;

            // Set the tab color
            workSheet.TabColor = Color.FromArgb(77, 255, 255);
        }
        private static void CreateInvoicePayment(ExcelPackage excelPackage, Invoice invoice, List<ProductionDept> productionDepts, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Star";
            //excelPackage.Workbook.Worksheets.Add("INV-PAYMENT");

            //var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            //workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            //workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //CreateHeaderInvoicePayment(workSheet, invoice);
            //FillDataInvoicePayment(workSheet, invoice);

            //string modelRangeBorder = "A1:" + "H" + _maxRow;

            //using (var range = workSheet.Cells[modelRangeBorder])
            //{
            //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    range.Style.Fill.BackgroundColor.SetColor(Color.White);
            //}


            //// Set page break view
            //workSheet.View.PageBreakView = true;
            //workSheet.PrinterSettings.FitToPage = true;
            //workSheet.Row(_maxRow).PageBreak = true;
            //workSheet.Column(8).PageBreak = true;

            //// Set the tab color
            //workSheet.TabColor = Color.FromArgb(219, 77, 255);

            // Split invoice payment

            var ListDept = new List<int>();
            foreach (var packinglist in invoice.PackingList)
            {
                var itemStyle = packinglist.ItemStyles.FirstOrDefault();
                var productionDept = productionDepts.Where(p => p.ProductionDescription == itemStyle?.ProductionDescription).FirstOrDefault();
                if (productionDept != null)
                {
                    ListDept.Add(productionDept.Dept);
                }
            }
            var Depts = ListDept.Distinct().ToList();
            var paymentIndex = 1;
            foreach (var dept in Depts.OrderBy(o=>o))
            {
                var pro = productionDepts.Where(p => p.Dept == dept).FirstOrDefault();

                var packingLists = getPackingListByDept(invoice.PackingList.ToList(),productionDepts,dept);
                if (packingLists.Count > 0)
                {
                    
                    var countPayment = (packingLists.Count -1) / 5;
                    //
                    int index = 0;
                    if (countPayment > 0)
                        index = 1;
                    //
                    for (int i = 0; i <= countPayment; i++)
                    {
                        int startIdx = i * 5;
                        int count = (startIdx + 4) <= (packingLists.Count - 1) ? 5 : ((packingLists.Count) - startIdx);
                        var excelPackingLists = packingLists.GetRange(startIdx, count);
                        
                        CreateHeaderInvoicePaymentSplit(excelPackage, invoice, dept, index++, sheetIndex);
                        FillDataInvoicePaymentSplit(excelPackage, invoice, excelPackingLists, sheetIndex);
                        sheetIndex++;
                    }
                }
            }
        }

        #region INVOICE
        private static void CreateHeaderInvoice(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Column(1).Width = 4;
            workSheet.Column(2).Width = 11.5;
            workSheet.Column(4).Width = 40;
            workSheet.Column(6).Width = 12.5;
            workSheet.Column(7).Width = 14;
            workSheet.Column(8).Width = 14;
            workSheet.Column(9).Width = 19;
            workSheet.Column(10).Width = 19;

            var addressList = invoice?.Company?.DisplayAddress?.ToUpper().Split(",").ToList();
            var address1 = "";
            var address2 = "";
            var address3 = "";
            var address4 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);

            if (!string.IsNullOrEmpty(invoice.Company?.DisplayPhone))
                address4 = "Tel: " + invoice.Company?.DisplayPhone + ", ";
            if (!string.IsNullOrEmpty(invoice.Company?.FaxNumber))
                address4 += "Fax: " + invoice.Company?.FaxNumber;

            var shipAddr = invoice?.ShipTo?.Address?.ToUpper().Split(",").ToList();
            var shipAddr1 = "";
            var shipAddr2 = "";
            var shipAddr3 = "";

            SplitAddress(shipAddr, ref shipAddr1, ref shipAddr2, ref shipAddr3);

            workSheet.Cells[1, 1].Value = "COMMERCIAL INVOICE";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 16;
            workSheet.Cells[1, 1, 1, 10].Merge = true; // range column A -> J
            workSheet.Cells[1, 1, 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1, 1, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            workSheet.Row(_maxRow).Height = 27;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SHIPPER/EXPORTER";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "For account & rick of messrs:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 9].Value = "Invoice No:";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 10].Value = invoice.Code?.ToUpper();
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = invoice.ShipTo?.Name?.ToUpper();
            //workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 9].Value = "Date:";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 10].Value = invoice.Date;
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address1;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = shipAddr1;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 9].Value = "Sailing on or about:";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address2 + (!string.IsNullOrEmpty(address3) ? ", " + address3 : string.Empty);
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = shipAddr2;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 9].Value = invoice.OnBoardDate;
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address4;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = shipAddr3;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 9].Value = "Port of Loading :";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            shipAddr = invoice?.Consignee?.Address?.ToUpper().Split(",").ToList();
            shipAddr1 = "";
            shipAddr2 = "";
            shipAddr3 = "";
            address4 = "";
            SplitAddress(shipAddr, ref shipAddr1, ref shipAddr2, ref shipAddr3);

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "CONSIGNEE";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "REMARKS:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;

            workSheet.Cells[_maxRow, 7].Value = "Payment terms by TTR";
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 9].Value = invoice.PortOfLoading?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Merge = true;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.Consignee?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "TERM:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;

            workSheet.Cells[_maxRow, 7].Value = invoice.IncoTerm?.Name?.ToUpper() + " INCOTERM 2000";
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 9].Value = "Final Destination :";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr1;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "SHIP MODE:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;

            workSheet.Cells[_maxRow, 7].Value = invoice.ShipmentCode?.ToUpper();
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 9].Value = invoice.FinalDestination?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Merge = true;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr2;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 9].Value = "Cont No:";
            workSheet.Cells[_maxRow, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 10].Value = invoice.ContainerNo;
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr3;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "Carrier / Vessel No:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_maxRow, 9].Value = "Seal No:";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 10].Value = invoice.SealNumber;
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            if (!string.IsNullOrEmpty(invoice.Consignee?.PhoneNumber))
                address4 = "INN " + invoice.Consignee?.PhoneNumber + ", ";
            if (!string.IsNullOrEmpty(invoice.Consignee?.Fax))
                address4 += "KPP " + invoice.Consignee?.Fax;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address4;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = invoice.VesselVoyageNo?.ToUpper();
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_maxRow, 9].Value = "LCL";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Merge = true;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
        }
        private static void FillDataInvoice(ExcelWorksheet workSheet, Invoice invoice)
        {            
            var rowBorder = _maxRow;
            var column = 1;
            var dicPositionColumnPackingLine = new Dictionary<string, int>();

            workSheet.Cells[_maxRow, column].Value = "NO.";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Order no.";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Model";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "DESCRIPTION";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Iman Code";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "QUANTITY @(PCS, SETS)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "U/P USD";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow, column + 1].Merge = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 15;

            workSheet.Cells[_maxRow + 1, column].Value = "CMQ";
            workSheet.Cells[_maxRow + 1, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dicPositionColumnPackingLine.Add("PriceCMQ", column);
            workSheet.Row(_maxRow + 1).Height = 34.5;

            column++;
            workSheet.Cells[_maxRow + 1, column].Value = "FCA";
            workSheet.Cells[_maxRow + 1, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dicPositionColumnPackingLine.Add("PriceFCA", column);

            column++;
            workSheet.Cells[_maxRow, column].Value = "AMOUNT USD";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow, column + 1].Merge = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow + 1, column].Value = "CMQ";
            workSheet.Cells[_maxRow + 1, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dicPositionColumnPackingLine.Add("AmountCMQ", column);

            column++;
            workSheet.Cells[_maxRow + 1, column].Value = "FCA";
            workSheet.Cells[_maxRow + 1, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dicPositionColumnPackingLine.Add("AmountFCA", column);

            _maxRow += 2;
            column = 1;
            int idx = 1;
            decimal totQty = 0;
            decimal totCMTAmt = 0;
            decimal totFCAAmt = 0;
            foreach (var packinglist in invoice.PackingList)
            {
                var itemStyle = packinglist.ItemStyles.FirstOrDefault();
                var invoiceDetail = invoice.InvoiceDetails.Where(x => x.GarmentColorCode == itemStyle.ColorCode
                                                                && x.GarmentColorName == itemStyle.ColorName
                                                                && x.CustomerPurchaseOrderNumber == itemStyle.PurchaseOrderNumber).FirstOrDefault();
                decimal priceCM = invoiceDetail?.PriceCM ?? 0;
                decimal priceFOB = invoiceDetail?.PriceFOB ?? 0;

                workSheet.Row(_maxRow).Height = 16.8;

                workSheet.Cells[_maxRow, column].Value = idx++;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.PurchaseOrderNumber;

                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.ColorCode;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.Description;

                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.CustomerStyle;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = packinglist.TotalQuantity;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                totQty += (decimal)packinglist.TotalQuantity;

             
                
                //CMQ
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Value = priceCM == 0 ? null : Math.Round(priceCM, 3);
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Style.Numberformat.Format = "#,##0.000";

                //FCA
                
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Value = priceFOB == 0 ? null : Math.Round(priceFOB, 3);
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Style.Numberformat.Format = "#,##0.000";

                //total CMQ
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountCMQ"]].Value = priceCM == 0 ? null : Math.Round(priceCM * (decimal)packinglist.TotalQuantity, 2);
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountCMQ"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountCMQ"]].Style.Numberformat.Format = "#,##0.00";
                totCMTAmt +=  Math.Round(priceCM * (decimal)packinglist.TotalQuantity, 2);
                //total FCA
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Value = priceFOB == 0 ? null : Math.Round(priceFOB * (decimal)packinglist.TotalQuantity, 2);
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Style.Numberformat.Format = "#,##0.00";
                totFCAAmt +=  Math.Round(priceFOB * (decimal)packinglist.TotalQuantity, 2);
               
                


                //if (itemStyle?.SalesOrder?.PriceTermCode == "CMT" || itemStyle?.SalesOrder?.PriceTermCode == "CM")
                //{
                //    //CMQ
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Value = Math.Round(price, 3);
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Style.Numberformat.Format = "#,##0.000";

                //    //FCA
                //    var partPrice = partPrices.Where(p=>p.Season == itemStyle.Season
                //                                    && p.GarmentColorCode == itemStyle.ColorCode                                                    
                //                                    && p.ProductionType == "FOB"
                //                                    ).FirstOrDefault();                    
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Value = partPrice == null ? null : Math.Round((decimal)partPrice?.Price, 3);
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Style.Numberformat.Format = "#,##0.000";

                //    //total CMQ
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountCMQ"]].Value = Math.Round(price * (decimal)packinglist.TotalQuantity, 3);
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountCMQ"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountCMQ"]].Style.Numberformat.Format = "#,##0.000";
                //    totCMTAmt += Math.Round(price * (decimal)packinglist.TotalQuantity, 3);
                //    //total FCA
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Value = partPrice == null ? null : Math.Round((decimal)partPrice.Price * (decimal)packinglist.TotalQuantity, 3);
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Style.Numberformat.Format = "#,##0.000";
                //    totFCAAmt += partPrice == null ? 0 : Math.Round((decimal)partPrice.Price * (decimal)packinglist.TotalQuantity, 3);
                //}
                //else 
                //{
                //    //FCA
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Value = Math.Round(price, 3);
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceFCA"]].Style.Numberformat.Format = "#,##0.000";
                //    //CMQ
                //    var partPrice = partPrices.Where(p => p.Season == itemStyle.Season
                //                                    && p.GarmentColorCode == itemStyle.ColorCode                                                   
                //                                    && p.ProductionType == "CMT"
                //                                    ).FirstOrDefault();
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Value = partPrice == null ? null : Math.Round((decimal)partPrice?.Price, 3);
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Style.Numberformat.Format = "#,##0.000";

                //    //total FCA
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Value = Math.Round(price * (decimal)packinglist.TotalQuantity, 3);
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["AmountFCA"]].Style.Numberformat.Format = "#,##0.000";
                //    totFCAAmt += Math.Round(price * (decimal)packinglist.TotalQuantity, 3);
                //    //total CMQ
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Value = partPrice == null ? null : Math.Round((decimal)partPrice.Price * (decimal)packinglist.TotalQuantity, 3);
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //    workSheet.Cells[_maxRow, dicPositionColumnPackingLine["PriceCMQ"]].Style.Numberformat.Format = "#,##0.000";
                //    totCMTAmt +=partPrice == null ? 0 : Math.Round((decimal)partPrice.Price * (decimal)packinglist.TotalQuantity, 3);
                //}

                column = 1;
                _maxRow++;
            }

            workSheet.Cells[_maxRow, 4].Value = "TOTAL";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow + 1).Height = 19.8;

            workSheet.Cells[_maxRow, 6].Value = totQty;
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 6].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, 9].Value = totCMTAmt == 0 ? null : totCMTAmt;
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "#,##0.00";

            workSheet.Cells[_maxRow, 10].Value = totFCAAmt == 0 ? null : totFCAAmt;
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 10].Style.Numberformat.Format = "#,##0.00";

            /// BORDER 
            using (var range = workSheet.Cells["A" + rowBorder + ":J" + _maxRow])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 5].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 10].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow + 1).Height = 34.4;

        }
        #endregion INVOICE

        #region PACKING LIST
        private static void CreateHeaderPL(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Column(1).Width = 4;
            workSheet.Column(2).Width = 10.9;
            workSheet.Column(3).Width = 11.5;
            workSheet.Column(4).Width = 9.8;
            workSheet.Column(5).Width = 37;
            workSheet.Column(6).Width = 26;
            workSheet.Column(7).Width = 9.5;
            workSheet.Column(8).Width = 13.5;
            workSheet.Column(9).Width = 22.5;
            workSheet.Column(10).Width = 10.8;
            workSheet.Column(11).Width = 11.5;
            workSheet.Column(12).Width = 7.5;

            var addressList = invoice?.Company?.DisplayAddress?.ToUpper().Split(",").ToList();
            var address1 = "";
            var address2 = "";
            var address3 = "";
            var address4 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);

            if (!string.IsNullOrEmpty(invoice.Company?.DisplayPhone))
                address4 = "Tel: " + invoice.Company?.DisplayPhone + ", ";
            if (!string.IsNullOrEmpty(invoice.Company?.FaxNumber))
                address4 += "Fax: " + invoice.Company?.FaxNumber;

            var shipAddr = invoice?.ShipTo?.Address?.ToUpper().Split(",").ToList();
            var shipAddr1 = "";
            var shipAddr2 = "";
            var shipAddr3 = "";

            SplitAddress(shipAddr, ref shipAddr1, ref shipAddr2, ref shipAddr3);

            workSheet.Cells[1, 1].Value = "PACKING LIST";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 16;
            workSheet.Cells[1, 1, 1, 12].Merge = true; // range column A -> L
            workSheet.Cells[1, 1, 1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            workSheet.Row(_maxRow).Height = 27;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SHIPPER/EXPORTER";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Value = "For account & rick of messrs:";
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 10].Value = "Invoice No:";
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 11].Value = invoice.Code?.ToUpper();
            workSheet.Cells[_maxRow, 11].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Value = invoice.ShipTo?.Name?.ToUpper();
            //workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 10].Value = "Date:";
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 11].Value = invoice.Date;
            workSheet.Cells[_maxRow, 11].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 11].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address1;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Value = shipAddr1;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 10].Value = "Sailing on or about:";
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address2 + (!string.IsNullOrEmpty(address3) ? ", " + address3 : string.Empty);
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Value = shipAddr2;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 10].Value = invoice.OnBoardDate;
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address4;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Value = shipAddr3;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 10].Value = "Port of Loading :";
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            shipAddr = invoice?.Consignee?.Address?.ToUpper().Split(",").ToList();
            shipAddr1 = "";
            shipAddr2 = "";
            shipAddr3 = "";
            address4 = "";
            SplitAddress(shipAddr, ref shipAddr1, ref shipAddr2, ref shipAddr3);

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "DELIVERY TO:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Value = "REMARKS:";
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7].Style.Font.UnderLine = true;


            workSheet.Cells[_maxRow, 10].Value = invoice.PortOfLoading?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.Consignee?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;


            workSheet.Cells[_maxRow, 10].Value = "Final Destination :";
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr1;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 10].Value = invoice.FinalDestination?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr2;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 10].Value = "Cont No:";
            workSheet.Cells[_maxRow, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 11].Value = invoice.ContainerNo;
            workSheet.Cells[_maxRow, 11].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr3;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Value = "Carrier / Vessel No:";
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_maxRow, 10].Value = "Seal No:";
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 11].Value = invoice.SealNumber;
            workSheet.Cells[_maxRow, 11].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 11, _maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            if (!string.IsNullOrEmpty(invoice.Consignee?.PhoneNumber))
                address4 = "INN " + invoice.Consignee?.PhoneNumber + ", ";
            if (!string.IsNullOrEmpty(invoice.Consignee?.Fax))
                address4 += "KPP " + invoice.Consignee?.Fax;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address4;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Value = invoice.VesselVoyageNo?.ToUpper();
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 7, _maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_maxRow, 10].Value = "LCL";
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 10, _maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
        }

        private static void FillDataPL(ExcelWorksheet workSheet, Invoice invoice)
        {
            var rowBorder = _maxRow;
            var column = 1;

            workSheet.Cells[_maxRow, column].Value = "NO.";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow + 1).Height = 35.8;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Order no.";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Model";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "ART NO.:";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "DESCRIPTION";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "SIZE";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Iman Code";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "QUANTITY @(PCS, SETS)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "CTNS";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "TOTAL N.W @(Kgs)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "TOTAL G.W @(Kgs)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "CBM";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            column = 1;
            int idx = 1;
            decimal totQty = 0;
            decimal totCarton = 0;
            decimal totNW = 0;
            decimal totGW = 0;
            decimal totCBM = 0;

            foreach (var packinglist in invoice.PackingList)
            {
                var itemStyle = packinglist.ItemStyles.FirstOrDefault();
                foreach (var data in itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex))
                {
                    var packingLines = packinglist.PackingLines
                        .Where(x => x.Size.Trim().Replace(" ", "").ToUpper() == data.Size.Trim().Replace(" ", "").ToUpper()
                                    && x.QuantitySize * x.TotalCarton > 0).ToList();
                    if (packingLines.Count > 0)
                    {
                        decimal cBM = 0;
                        foreach (var packingline in packingLines)
                        {
                            cBM += (decimal)(packingline.Length * packingline.Width * packingline.Height * packingline.TotalCarton / 1000000);
                        }

                        workSheet.Row(_maxRow).Height = 16.8;

                        workSheet.Cells[_maxRow, column].Value = idx++;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle?.PurchaseOrderNumber;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle?.ColorCode;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle?.Barcodes
                            .FirstOrDefault(x => x.Size.Trim().Replace(" ", "").ToUpper()
                                            == data.Size.Trim().Replace(" ", "").ToUpper()).BarCode;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle?.Description;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = data.Size.Trim().ToUpper();

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle?.CustomerStyle;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = packingLines.Sum(x => x.QuantitySize * x.TotalCarton);
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0";
                        totQty += (decimal)packingLines.Sum(x => x.QuantitySize * x.TotalCarton);

                        column++;
                        workSheet.Cells[_maxRow, column].Value = packingLines.Sum(x => x.TotalCarton);
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0";
                        totCarton += (decimal)packingLines.Sum(x => x.TotalCarton);

                        column++;
                        workSheet.Cells[_maxRow, column].Value = Math.Round((decimal)packingLines.Sum(x => x.NetWeight), 2);
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";
                        totNW += Math.Round((decimal)packingLines.Sum(x => x.NetWeight), 2);

                        column++;
                        workSheet.Cells[_maxRow, column].Value = Math.Round((decimal)packingLines.Sum(x => x.GrossWeight), 2);
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";
                        totGW += Math.Round((decimal)packingLines.Sum(x => x.GrossWeight), 2);

                        column++;
                        workSheet.Cells[_maxRow, column].Value = Math.Round(cBM, 3);
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.000";
                        totCBM += Math.Round(cBM, 3);

                        column = 1;
                        _maxRow++;
                    }
                }
            }

            workSheet.Cells[_maxRow, 5].Value = "TOTAL";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 8].Value = totQty;
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 8].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, 9].Value = totCarton;
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, 10].Value = totNW;
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 10].Style.Numberformat.Format = "#,##0.00";

            workSheet.Cells[_maxRow, 11].Value = totGW;
            workSheet.Cells[_maxRow, 11].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 11].Style.Numberformat.Format = "#,##0.00";

            workSheet.Cells[_maxRow, 12].Value = totCBM;
            workSheet.Cells[_maxRow, 12].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 12].Style.Numberformat.Format = "#,##0.000";

            /// BORDER 
            using (var range = workSheet.Cells["A" + rowBorder + ":L" + _maxRow])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            _maxRow += 2;
            workSheet.Cells[_maxRow, 2].Value = "TOTAL:";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 3].Value = totCarton + " CTNS";
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 3].Style.Numberformat.Format = "#,##0";

            _maxRow++;
            workSheet.Cells[_maxRow, 3].Value = totNW + " KGS";
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 3].Style.Numberformat.Format = "#,##0.00";

            _maxRow++;
            workSheet.Cells[_maxRow, 3].Value = totGW + " KGS";
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 3].Style.Numberformat.Format = "#,##0.00";

            workSheet.Cells[_maxRow, 8].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 3].Value = totCBM + " CBM";
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 3].Style.Numberformat.Format = "#,##0.000";


            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "SHIPPING MARKS:";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 2].Style.Font.UnderLine = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "PO#";

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "ART#";

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "Gross Weight:";

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "Volumn:";

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "Made in";

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "Size:";

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "Color:";

        }
        #endregion PACKING LIST

        #region INVOICE PAYMENT
        private static void CreateHeaderInvoicePayment(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Column(1).Width = 4;
            workSheet.Column(2).Width = 11.5;
            workSheet.Column(3).Width = 8.5;
            workSheet.Column(4).Width = 38.5;
            workSheet.Column(5).Width = 17;
            workSheet.Column(6).Width = 19;
            workSheet.Column(7).Width = 19.5;
            workSheet.Column(8).Width = 21;

            var addressList = invoice?.Company?.DisplayAddress?.ToUpper().Split(",").ToList();
            var address1 = "";
            var address2 = "";
            var address3 = "";
            var address4 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);

            if (!string.IsNullOrEmpty(invoice.Company?.DisplayPhone))
                address4 = "Tel: " + invoice.Company?.DisplayPhone + ", ";
            if (!string.IsNullOrEmpty(invoice.Company?.FaxNumber))
                address4 += "Fax: " + invoice.Company?.FaxNumber;

            var shipAddr = invoice?.ShipTo?.Address?.ToUpper().Split(",").ToList();
            var shipAddr1 = "";
            var shipAddr2 = "";
            var shipAddr3 = "";

            SplitAddress(shipAddr, ref shipAddr1, ref shipAddr2, ref shipAddr3);

            workSheet.Cells[1, 1].Value = "COMMERCIAL INVOICE";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 16;
            workSheet.Cells[1, 1, 1, 8].Merge = true; // range column A -> H
            workSheet.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1, 1, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            workSheet.Row(_maxRow).Height = 27;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "1) SHIPPER/EXPORTER";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "4) Port of loading:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 7].Value = "8) Invoice No:";
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 8].Value = invoice.Code?.ToUpper();
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 8].Style.Font.Color.SetColor(Color.Purple);
            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = invoice.PortOfLoading?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 5, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Value = invoice.Date;
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 8].Style.Numberformat.Format = "dd/MM/yyyy";
            workSheet.Cells[_maxRow, 8].Style.Font.Color.SetColor(Color.Purple);
            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address1;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "5) For transportation to:";
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 7].Value = "9) No & date of L/C: ";
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address2 + (!string.IsNullOrEmpty(address3) ? ", " + address3 : string.Empty);
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = invoice.ShipTo?.Country?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 5, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address4;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "2) For account & rick of messrs:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "6) Carrier:";
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 6].Value = "7) Sailing on:";
            workSheet.Cells[_maxRow, 6].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 7].Value = "10) Dilivery Term:";
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Value = "CMQ"; ;
            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.ShipTo?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = invoice.ShipmentCode?.ToUpper();

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Value = "INCOTERM 2000";
            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr1;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Value = "Payment terms by TTR";
            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr2 + (!string.IsNullOrEmpty(shipAddr3) ? ", " + shipAddr3.Trim() : string.Empty);
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            shipAddr = invoice?.Consignee?.Address?.ToUpper().Split(",").ToList();
            shipAddr1 = "";
            shipAddr2 = "";
            shipAddr3 = "";
            address4 = "";
            SplitAddress(shipAddr, ref shipAddr1, ref shipAddr2, ref shipAddr3);

            if (!string.IsNullOrEmpty(invoice.Consignee?.PhoneNumber))
                address4 = "Tel: " + invoice.Consignee?.PhoneNumber + ", ";
            if (!string.IsNullOrEmpty(invoice.Consignee?.Fax))
                address4 += "Fax: " + invoice.Consignee?.Fax;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "3) CONSIGNEE";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 5].Value = "11) No of PKGS:";
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            //workSheet.Cells[_maxRow, 7].Value = "Customs Invoice No:";
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Value = invoice?.CustomerInvoiceNo?.ToUpper();
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice?.Consignee?.Name;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr1;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_maxRow, 5].Value = "Range Size:";
            workSheet.Cells[_maxRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr2.Trim();
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = shipAddr3.Trim();
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_maxRow).Height = 19.8;

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = address4;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_maxRow, 5].Value = "Country of Origin:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 6].Value = "VIET NAM";

            workSheet.Cells[_maxRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            _maxRow++;
        }
        private static void FillDataInvoicePayment(ExcelWorksheet workSheet, Invoice invoice)
        {
            var rowBorder = _maxRow;
            var column = 1;

            workSheet.Cells[_maxRow, column].Value = "NO.";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 51;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Order no.";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Model";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "DESCRIPTION";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Iman Code";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "QUANTITY @(PCS)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            var title = "UNIT PRICE @(USD / PCS) @CMQ";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            title = "AMOUNT US$ @CMQ";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++; ;
            column = 1;
            int idx = 1;
            decimal totQty = 0;
            decimal totAmt = 0;
            foreach (var packinglist in invoice.PackingList)
            {
                var itemStyle = packinglist.ItemStyles.FirstOrDefault();
                var invoiceDetail = invoice.InvoiceDetails.Where(x => x.GarmentColorCode == itemStyle.ColorCode
                                                                && x.GarmentColorName == itemStyle.ColorName
                                                                && x.CustomerPurchaseOrderNumber == itemStyle.PurchaseOrderNumber).FirstOrDefault();
                decimal priceCM = invoiceDetail?.PriceCM ?? 0;
                //var price = (decimal)itemStyle.OrderDetails.FirstOrDefault().Price;
                workSheet.Row(_maxRow).Height = 16.8;

                workSheet.Cells[_maxRow, column].Value = idx++;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.PurchaseOrderNumber;

                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.ColorCode;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.Description;

                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.CustomerStyle;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = packinglist.TotalQuantity;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                totQty += (decimal)packinglist.TotalQuantity;

                column++;
                workSheet.Cells[_maxRow, column].Value = Math.Round(priceCM, 3);
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.000";

                column++;
                workSheet.Cells[_maxRow, column].Value = Math.Round(priceCM * (decimal)packinglist.TotalQuantity, 3);
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.000";
                totAmt += Math.Round(priceCM * (decimal)packinglist.TotalQuantity, 2);

                column = 1;
                _maxRow++;
            }

            workSheet.Cells[_maxRow, 4].Value = "TOTAL";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow + 1).Height = 19.8;

            workSheet.Cells[_maxRow, 6].Value = totQty;
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 6].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, 8].Value = totAmt == 0 ? null : totAmt;
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 8].Style.Numberformat.Format = "#,##0.00";

            /// BORDER 
            using (var range = workSheet.Cells["A" + rowBorder + ":H" + _maxRow])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "SAY TOTAL:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Color.SetColor(Color.Red);

            workSheet.Cells[_maxRow, 3].Value = NumberToWordsHelpers.ToVerbalCurrency((double)totAmt);
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 3].Style.Font.Color.SetColor(Color.Red);

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "The exporter/supplier of the products covered by this invoice hereby declares that, except where otherwise clearly indicated, these goods are of Vietnam";

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "orrigin within the meaning of the rules in force.";

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "(By virtue of the clause of property reserve, the googds relative to the present orders will remain LEADING STAR'S property up to their complete payment)";

            _maxRow += 3;
            workSheet.Cells[_maxRow, 5].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow + 1).Height = 34.4;

        }
        #endregion INVOICE PAYMENT

        #region INVOICE PAYMENT SPLIT
        public static List<PackingList> getPackingListByDept(List<PackingList> packingLists,List<ProductionDept> productionDepts,int dept)
        {
            var list = new List<PackingList>();
            foreach(var packingList in packingLists)
            {
                var itemStyle = packingList.ItemStyles.FirstOrDefault();
                if(itemStyle != null)
                {
                    var pro = productionDepts.Where(p=>p.ProductionDescription == itemStyle.ProductionDescription).FirstOrDefault();
                    if(pro!=null)
                    {
                        if(pro.Dept== dept)
                        {
                            list.Add(packingList);
                        }    
                    }    
                }    
            }    
            return list;
        }
        public static string getStringInvoiceNo(string invoicNO)
        {
            int a = 0;
            int idx = 0;
            var chars = invoicNO.ToCharArray();
            foreach(char c in chars)
            {
                
                if(!int.TryParse(c.ToString(), out a))
                {
                    return invoicNO.Substring(idx, invoicNO.Length - idx);
                }
                idx++;
            }
            return invoicNO;
        }
        private static void CreateHeaderInvoicePaymentSplit(ExcelPackage excelPackage, Invoice invoice,int dept,int index,int sheetIndex)
        {
            int _currentRow = 1;
            //excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            var sheetName = dept.ToString() + getStringInvoiceNo(invoice.Code) + (index > 0 ? index.ToString() : "");
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Star";
            excelPackage.Workbook.Worksheets.Add(sheetName);

            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Column(1).Width = 4;
            workSheet.Column(2).Width = 11.5;
            workSheet.Column(3).Width = 8.5;
            workSheet.Column(4).Width = 38.5;
            workSheet.Column(5).Width = 17;
            workSheet.Column(6).Width = 19;
            workSheet.Column(7).Width = 19.5;
            workSheet.Column(8).Width = 21;

            var addressList = invoice?.Company?.DisplayAddress?.ToUpper().Split(",").ToList();
            var address1 = "";
            var address2 = "";
            var address3 = "";
            var address4 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);

            if (!string.IsNullOrEmpty(invoice.Company?.DisplayPhone))
                address4 = "Tel: " + invoice.Company?.DisplayPhone + ", ";
            if (!string.IsNullOrEmpty(invoice.Company?.FaxNumber))
                address4 += "Fax: " + invoice.Company?.FaxNumber;

            var shipAddr = invoice?.ShipTo?.Address?.ToUpper().Split(",").ToList();
            var shipAddr1 = "";
            var shipAddr2 = "";
            var shipAddr3 = "";

            SplitAddress(shipAddr, ref shipAddr1, ref shipAddr2, ref shipAddr3);

            workSheet.Cells[1, 1].Value = "COMMERCIAL INVOICE";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 16;
            workSheet.Cells[1, 1, 1, 8].Merge = true; // range column A -> H
            workSheet.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1, 1, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            workSheet.Row(_currentRow).Height = 27;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = "1) SHIPPER/EXPORTER";
            workSheet.Cells[_currentRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 5].Value = "4) Port of loading:";
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_currentRow, 5, _currentRow, 6].Merge = true;
            workSheet.Cells[_currentRow, 5, _currentRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 7].Value = "8) Invoice No:";
            workSheet.Cells[_currentRow, 7].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 8].Value = sheetName; //invoice.Code?.ToUpper();
            workSheet.Cells[_currentRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 8].Style.Font.Color.SetColor(Color.Purple);
            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 5].Value = invoice.PortOfLoading?.Name?.ToUpper();
            workSheet.Cells[_currentRow, 5, _currentRow, 6].Merge = true;
            workSheet.Cells[_currentRow, 5, _currentRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Value = invoice.Date;
            workSheet.Cells[_currentRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 8].Style.Numberformat.Format = "dd/MM/yyyy";
            workSheet.Cells[_currentRow, 8].Style.Font.Color.SetColor(Color.Purple);
            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = address1;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 5].Value = "5) For transportation to:";
            workSheet.Cells[_currentRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 5, _currentRow, 6].Merge = true;
            workSheet.Cells[_currentRow, 5, _currentRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 7].Value = "9) No & date of L/C: ";
            workSheet.Cells[_currentRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = address2 + (!string.IsNullOrEmpty(address3) ? ", " + address3 : string.Empty);
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 5].Value = invoice.ShipTo?.Country?.Name?.ToUpper();
            workSheet.Cells[_currentRow, 5, _currentRow, 6].Merge = true;
            workSheet.Cells[_currentRow, 5, _currentRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = address4;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = "2) For account & rick of messrs:";
            workSheet.Cells[_currentRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 5].Value = "6) Carrier:";
            workSheet.Cells[_currentRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 6].Value = "7) Sailing on:";
            workSheet.Cells[_currentRow, 6].Style.Font.UnderLine = true;
            workSheet.Cells[_currentRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 7].Value = "10) Dilivery Term:";
            workSheet.Cells[_currentRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Value = "CMQ"; ;
            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = invoice.ShipTo?.Name?.ToUpper();
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 5].Value = invoice.ShipmentCode?.ToUpper();

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Value = "INCOTERM 2000";
            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = shipAddr1;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Value = "Payment terms by TTR";
            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = shipAddr2 + (!string.IsNullOrEmpty(shipAddr3) ? ", " + shipAddr3.Trim() : string.Empty);
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            shipAddr = invoice?.Consignee?.Address?.ToUpper().Split(",").ToList();
            shipAddr1 = "";
            shipAddr2 = "";
            shipAddr3 = "";
            address4 = "";
            SplitAddress(shipAddr, ref shipAddr1, ref shipAddr2, ref shipAddr3);

            if (!string.IsNullOrEmpty(invoice.Consignee?.PhoneNumber))
                address4 = "Tel: " + invoice.Consignee?.PhoneNumber + ", ";
            if (!string.IsNullOrEmpty(invoice.Consignee?.Fax))
                address4 += "Fax: " + invoice.Consignee?.Fax;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = "3) CONSIGNEE";
            workSheet.Cells[_currentRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 5].Value = "11) No of PKGS:";
            workSheet.Cells[_currentRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            //workSheet.Cells[_currentRow, 7].Value = "Customs Invoice No:";
            workSheet.Cells[_currentRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Value = invoice?.CustomerInvoiceNo?.ToUpper();
            workSheet.Cells[_currentRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = invoice?.Consignee?.Name;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = shipAddr1;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_currentRow, 5].Value = "Range Size:";
            workSheet.Cells[_currentRow, 5].Style.Font.UnderLine = true;
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = shipAddr2.Trim();
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = shipAddr3.Trim();
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Row(_currentRow).Height = 19.8;

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = address4;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 1, _currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_currentRow, 5].Value = "Country of Origin:";
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_currentRow, 6].Value = "VIET NAM";

            workSheet.Cells[_currentRow, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_currentRow, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_currentRow, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            _currentRow++;
        }
        private static void FillDataInvoicePaymentSplit(ExcelPackage excelPackage, Invoice invoice,List<PackingList> packingLists,int sheetIndex)
        {
            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            int _currentRow = 17;
            var rowBorder = _currentRow;
            var column = 1;

            workSheet.Cells[_currentRow, column].Value = "NO.";
            workSheet.Cells[_currentRow, column].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_currentRow).Height = 51;

            column++;
            workSheet.Cells[_currentRow, column].Value = "Order no.";
            workSheet.Cells[_currentRow, column].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_currentRow, column].Value = "Model";
            workSheet.Cells[_currentRow, column].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_currentRow, column].Value = "DESCRIPTION";
            workSheet.Cells[_currentRow, column].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_currentRow, column].Value = "Iman Code";
            workSheet.Cells[_currentRow, column].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, column].Style.WrapText = true;
            workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_currentRow, column].Value = "QUANTITY @(PCS)".Replace('@', '\n');
            workSheet.Cells[_currentRow, column].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, column].Style.WrapText = true;
            workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            var title = "UNIT PRICE @(USD / PCS) @CMQ";
            workSheet.Cells[_currentRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_currentRow, column].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, column].Style.WrapText = true;
            workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            title = "AMOUNT US$ @CMQ";
            workSheet.Cells[_currentRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_currentRow, column].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, column].Style.WrapText = true;
            workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _currentRow++; ;
            column = 1;
            int idx = 1;
            decimal totQty = 0;
            decimal totAmt = 0;
            foreach (var packinglist in packingLists)
            {
                var itemStyle = packinglist.ItemStyles.FirstOrDefault();
                var invoiceDetail = invoice.InvoiceDetails.Where(x => x.GarmentColorCode == itemStyle.ColorCode
                                                                && x.GarmentColorName == itemStyle.ColorName
                                                                && x.CustomerPurchaseOrderNumber == itemStyle.PurchaseOrderNumber).FirstOrDefault();
                decimal priceCM = invoiceDetail?.PriceCM ?? 0;
                //var price = (decimal)itemStyle.OrderDetails.FirstOrDefault().Price;
                workSheet.Row(_currentRow).Height = 16.8;

                workSheet.Cells[_currentRow, column].Value = idx++;
                workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_currentRow, column].Value = itemStyle?.PurchaseOrderNumber;

                column++;
                workSheet.Cells[_currentRow, column].Value = itemStyle?.ColorCode;
                workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_currentRow, column].Value = itemStyle?.Description;

                column++;
                workSheet.Cells[_currentRow, column].Value = itemStyle?.CustomerStyle;
                workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_currentRow, column].Value = packinglist.TotalQuantity;
                workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                totQty += (decimal)packinglist.TotalQuantity;

                column++;
                workSheet.Cells[_currentRow, column].Value = priceCM == 0 ? null : Math.Round(priceCM, 3);
                workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_currentRow, column].Style.Numberformat.Format = "#,##0.000";

                column++;
                workSheet.Cells[_currentRow, column].Value = priceCM == 0 ? null : Math.Round(priceCM * (decimal)packinglist.TotalQuantity, 2);
                workSheet.Cells[_currentRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_currentRow, column].Style.Numberformat.Format = "#,##0.00";
                totAmt += Math.Round(priceCM * (decimal)packinglist.TotalQuantity, 2);

                column = 1;
                _currentRow++;
            }

            workSheet.Cells[_currentRow, 4].Value = "TOTAL";
            workSheet.Cells[_currentRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_currentRow + 1).Height = 19.8;

            workSheet.Cells[_currentRow, 6].Value = totQty;
            workSheet.Cells[_currentRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_currentRow, 6].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_currentRow, 8].Value = totAmt == 0 ? null : totAmt ;
            workSheet.Cells[_currentRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_currentRow, 8].Style.Numberformat.Format = "#,##0.00";

            /// BORDER 
            using (var range = workSheet.Cells["A" + rowBorder + ":H" + _currentRow])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            _currentRow += 2;
            workSheet.Cells[_currentRow, 1].Value = "SAY TOTAL:";
            workSheet.Cells[_currentRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 1].Style.Font.Color.SetColor(Color.Red);

            workSheet.Cells[_currentRow, 3].Value = NumberToWordsHelpers.ToVerbalCurrency((double)totAmt);
            workSheet.Cells[_currentRow, 3].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 3].Style.Font.Color.SetColor(Color.Red);

            _currentRow += 2;
            workSheet.Cells[_currentRow, 1].Value = "The exporter/supplier of the products covered by this invoice hereby declares that, except where otherwise clearly indicated, these goods are of Vietnam";

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = "orrigin within the meaning of the rules in force.";

            _currentRow++;
            workSheet.Cells[_currentRow, 1].Value = "(By virtue of the clause of property reserve, the googds relative to the present orders will remain LEADING STAR'S property up to their complete payment)";

            _currentRow += 3;
            workSheet.Cells[_currentRow, 5].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_currentRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_currentRow, 5, _currentRow, 8].Merge = true;
            workSheet.Cells[_currentRow, 5, _currentRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_currentRow + 1).Height = 34.4;

        }
       


        #endregion

        #region General Function
        private static void SplitAddress(List<string> addressList, ref string addr1, ref string addr2, ref string addr3)
        {
            for (var i = 0; i < addressList?.Count; i++)
            {
                if (i < 3)
                    addr1 += string.IsNullOrEmpty(addr1) ? addressList[i].Trim() : ", " + addressList[i];
                else if (i < 6)
                    addr2 += string.IsNullOrEmpty(addr2) ? addressList[i].Trim() : ", " + addressList[i];
                else
                    addr3 += string.IsNullOrEmpty(addr3) ? addressList[i].Trim() : ", " + addressList[i];
            }
        }

        #endregion
    }
}
