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
    public class ExportInvoicePU_Processor
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
                    CreateInvoiceTSG(excelPackage, invoices[i], Title, sheetIndex);
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
            excelPackage.Workbook.Worksheets.Add("INV HQ");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 12));
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
        }

        private static void CreateInvoiceTSG(ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Star";
            excelPackage.Workbook.Worksheets.Add("INV TT");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 14));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateHeaderInvoiceTSG(workSheet, invoice);
            FillDataInvoiceTSG(workSheet, invoice);

            string modelRangeBorder = "A1:" + "H" + _maxRow;

            using (var range = workSheet.Cells[modelRangeBorder])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White);
            }


            // Set page break view
            workSheet.View.PageBreakView = true;
            workSheet.PrinterSettings.FitToPage = true;
            workSheet.Row(_maxRow).PageBreak = true;
            workSheet.Column(8).PageBreak = true;
        }

        #region INVOICE
        private static void CreateHeaderInvoice(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Column(1).Width = 12.5;
            workSheet.Column(2).Width = 12.5;
            workSheet.Column(3).Width = 12.5;
            workSheet.Column(4).Width = 21.8;
            workSheet.Column(5).Width = 32.8;
            workSheet.Column(6).Width = 9.8;
            workSheet.Column(7).Width = 10.8;
            workSheet.Column(8).Width = 10.8;
            workSheet.Column(9).Width = 14.3;
            workSheet.Column(10).Width = 14.3;

            workSheet.Cells[1, 1].Value = "COMMERCIAL INVOICE";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1, 1, 10].Merge = true; // range column A -> J
            workSheet.Cells[1, 1, 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 8].Value = "No:";
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 9].Value = invoice?.Code;

            _maxRow++;
            workSheet.Cells[_maxRow, 8].Value = "Date:";
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 9].Value = invoice?.Date;
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "MMM dd yyyy";
            workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _maxRow++;
            workSheet.Cells[_maxRow, 8].Value = "TSG INV NO:";
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 9].Value = invoice?.CustomerInvoiceNo;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SHIPPER:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = invoice?.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow++;
            var addressList = invoice?.Company?.DisplayAddress?.ToUpper().Split(",").ToList();
            var address1 = "";
            var address2 = "";
            var address3 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);
            workSheet.Cells[_maxRow, 4].Value = address1;

            _maxRow++;
            workSheet.Cells[_maxRow, 4].Value = address2 + 
                (!string.IsNullOrEmpty(address3) ? (", " + address3) : string.Empty);

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "For account"; 
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = invoice?.ShipTo?.Name;
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow += 1;
            addressList = invoice?.ShipTo?.Address?.Split(",").ToList();
            address1 = "";
            address2 = "";
            address3 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);
            workSheet.Cells[_maxRow, 1].Value = "& rick of messrs:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = address1;

            _maxRow++;
            workSheet.Cells[_maxRow, 4].Value = address2 +
                (!string.IsNullOrEmpty(address3) ? (", " + address3) : string.Empty);

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "CONSIGNEE:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = invoice?.Consignee?.Name;
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow += 1;
            addressList = invoice?.Consignee?.Address?.Split(",").ToList();
            address1 = "";
            address2 = "";
            address3 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);
            workSheet.Cells[_maxRow, 4].Value = address1;

            if(!string.IsNullOrEmpty(address2))
            {
                _maxRow++;
                workSheet.Cells[_maxRow, 4].Value = address2;
            }

            if (!string.IsNullOrEmpty(address3))
            {
                _maxRow++;
                workSheet.Cells[_maxRow, 4].Value = address3;
            }

            if (!string.IsNullOrEmpty(invoice?.Consignee?.ZipCode))
            {
                _maxRow++;
                workSheet.Cells[_maxRow, 4].Value = "Zip Code: " + invoice?.Consignee?.ZipCode;
            }

            if (!string.IsNullOrEmpty(invoice?.Consignee?.PhoneNumber))
            {
                _maxRow++;
                workSheet.Cells[_maxRow, 4].Value = "Phone: " + invoice?.Consignee?.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(invoice?.Consignee?.Fax))
            {
                _maxRow++;
                workSheet.Cells[_maxRow, 4].Value = "Fax: " + invoice?.Consignee?.Fax;
            }

            if (!string.IsNullOrEmpty(invoice?.Consignee?.Email))
            {
                _maxRow++;
                workSheet.Cells[_maxRow, 4].Value = "Email: " + invoice?.Consignee?.Email;
            }

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "PORT OF LOADING:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = invoice?.PortOfLoading?.Name?.ToUpper() +
                ((invoice?.PortOfLoading?.CountryID ?? 0) != 0 ? (", " + invoice?.PortOfLoading?.Country?.Name?.ToUpper()) : string.Empty);

            _maxRow += 1;
            var style = invoice?.PackingList?.FirstOrDefault()?.ItemStyles?.FirstOrDefault();

            workSheet.Cells[_maxRow, 1].Value = "PLACE OF DESTINATION:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = style?.DeliveryPlace;

            _maxRow += 1;
            workSheet.Cells[_maxRow, 1].Value = "DELIVERY TERM:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = "FOB HO CHI MINH PORT, VN";

            _maxRow += 1;
            workSheet.Cells[_maxRow, 1].Value = "PAYMENT TERM:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = invoice?.PaymentTerm?.Description?.ToUpper();

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "CUSTOMER CODE:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = style?.CustomerCode?.ToUpper() ?? style?.UCustomerCode?.ToUpper();

            _maxRow += 2;
        }
        private static void FillDataInvoice(ExcelWorksheet workSheet, Invoice invoice)
        {
            var itemStyles = new List<ItemStyle>();
            invoice?.PackingList?.ToList().ForEach(p =>
            {
                itemStyles.AddRange(p?.ItemStyles);
            });

            var rowBorder = _maxRow;
            var column = 1;

            workSheet.Cells[_maxRow, column].Value = "PO CUS No.";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 54;

            column++;
            workSheet.Cells[_maxRow, column].Value = "PO";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "STYLE";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "COLOUR";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "DESCRIPTION";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Q'TY @PCS".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "FOB PRICE @(USD)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "CM PRICE @(USD)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "TOTAL FOB @VALUE @(USD)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "TOTAL CM @VALUE @(USD)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow += 1;
            column = 1;
            decimal? totQty = 0;
            decimal? totCMAmt = 0;
            decimal? totFOBAmt = 0;
            foreach (var detail in invoice?.InvoiceDetails)
            {
                var itemStyle = itemStyles
                        .FirstOrDefault(x => x.ColorCode == detail.GarmentColorCode &&
                                            x.CustomerStyle == detail.CustomerStyle &&
                                            x.PurchaseOrderNumber == detail.CustomerPurchaseOrderNumber);

                workSheet.Cells[_maxRow, column].Value = itemStyle?.CustomerCodeNo;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(_maxRow).Height = 45;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail.CustomerPurchaseOrderNumber;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.CustomerStyle;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.GarmentColorCode?.ToUpper() + " " + detail?.GarmentColorName?.ToUpper();
                workSheet.Cells[_maxRow, column].Style.WrapText = true;


                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.Description?.ToUpper();
                workSheet.Cells[_maxRow, column].Style.WrapText = true;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.Quantity;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                totQty += detail?.Quantity ?? 0;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.PriceFOB;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.PriceCM;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.PriceFOB * detail?.Quantity;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";
                totFOBAmt += (detail?.PriceFOB ?? 0) * detail?.Quantity;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.PriceCM * detail?.Quantity;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";
                totCMAmt += (detail?.PriceCM ?? 0) * detail?.Quantity;

                column = 1;
                _maxRow++;
            }

            workSheet.Cells[_maxRow, 1].Value = "TOTAL";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 5].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 40;

            workSheet.Cells[_maxRow, 6].Value = totQty;
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 6].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, 9].Value = totFOBAmt;
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "#,##0.00";

            workSheet.Cells[_maxRow, 10].Value = totCMAmt;
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
            workSheet.Cells[_maxRow, 1].Value = "Say CM total:";

            workSheet.Cells[_maxRow, 2].Value = NumberToWordsHelpers.ToVerbalCurrency((double)totCMAmt);
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 2].Style.Font.Italic = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 10].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 10].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        }
        #endregion INVOICE

        #region INVOICE TSG
        private static void CreateHeaderInvoiceTSG(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Column(1).Width = 14.5;
            workSheet.Column(2).Width = 14.5;
            workSheet.Column(3).Width = 14.5;
            workSheet.Column(4).Width = 23.8;
            workSheet.Column(5).Width = 34.8;
            workSheet.Column(6).Width = 11.8;
            workSheet.Column(7).Width = 12.8;
            workSheet.Column(8).Width = 16.3;

            workSheet.Cells[1, 1].Value = "COMMERCIAL INVOICE";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 24;
            workSheet.Cells[1, 1, 1, 8].Merge = true; // range column A -> J
            workSheet.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "No:";
            workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 8].Value = invoice?.Code;

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "Date:";
            workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 8].Value = invoice?.Date;
            workSheet.Cells[_maxRow, 8].Style.Numberformat.Format = "MMM dd yyyy";
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "TSG INV:";
            workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 8].Value = invoice?.CustomerInvoiceNo;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SHIPPER:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = invoice?.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow++;
            var addressList = invoice?.Company?.DisplayAddress?.ToUpper().Split(",").ToList();
            var address1 = "";
            var address2 = "";
            var address3 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);
            workSheet.Cells[_maxRow, 4].Value = address1;

            _maxRow++;
            workSheet.Cells[_maxRow, 4].Value = address2 +
                (!string.IsNullOrEmpty(address3) ? (", " + address3) : string.Empty);

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "For account";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = invoice?.ShipTo?.Name;
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow += 1;
            addressList = invoice?.ShipTo?.Address?.Split(",").ToList();
            address1 = "";
            address2 = "";
            address3 = "";

            SplitAddress(addressList, ref address1, ref address2, ref address3);
            workSheet.Cells[_maxRow, 1].Value = "& rick of messrs:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = address1;

            _maxRow++;
            workSheet.Cells[_maxRow, 4].Value = address2 +
                (!string.IsNullOrEmpty(address3) ? (", " + address3) : string.Empty);

            _maxRow += 2;
        }
        private static void FillDataInvoiceTSG(ExcelWorksheet workSheet, Invoice invoice)
        {
            var itemStyles = new List<ItemStyle>();
            invoice?.PackingList?.ToList().ForEach(p =>
            {
                itemStyles.AddRange(p?.ItemStyles);
            });

            var rowBorder = _maxRow;
            var column = 1;

            workSheet.Cells[_maxRow, column].Value = "PO CUS No.";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 54;

            column++;
            workSheet.Cells[_maxRow, column].Value = "PO";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "STYLE";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "COLOUR";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "DESCRIPTION";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "Q'TY @PCS".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "CM PRICE @(USD)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "TOTAL CM @VALUE @(USD)".Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow += 1;
            column = 1;
            decimal? totQty = 0;
            decimal? totCMAmt = 0;
            foreach (var detail in invoice?.InvoiceDetails)
            {
                var itemStyle = itemStyles
                        .FirstOrDefault(x => x.ColorCode == detail.GarmentColorCode &&
                                            x.CustomerStyle == detail.CustomerStyle &&
                                            x.PurchaseOrderNumber == detail.CustomerPurchaseOrderNumber);

                workSheet.Cells[_maxRow, column].Value = itemStyle?.CustomerCodeNo;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(_maxRow).Height = 45;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail.CustomerPurchaseOrderNumber;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.CustomerStyle;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.GarmentColorCode?.ToUpper() + " " + detail?.GarmentColorName?.ToUpper();
                workSheet.Cells[_maxRow, column].Style.WrapText = true;


                column++;
                workSheet.Cells[_maxRow, column].Value = itemStyle?.Description?.ToUpper();
                workSheet.Cells[_maxRow, column].Style.WrapText = true;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.Quantity;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                totQty += detail?.Quantity ?? 0;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.PriceCM;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                column++;
                workSheet.Cells[_maxRow, column].Value = detail?.PriceCM * detail?.Quantity;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";
                totCMAmt += (detail?.PriceCM ?? 0) * detail?.Quantity;

                column = 1;
                _maxRow++;
            }

            workSheet.Cells[_maxRow, 1].Value = "TOTAL";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 5].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 40;

            workSheet.Cells[_maxRow, 6].Value = totQty;
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 6].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, 8].Value = totCMAmt;
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

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Say CM total:";

            workSheet.Cells[_maxRow, 2].Value = NumberToWordsHelpers.ToVerbalCurrency((double)totCMAmt);
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 2].Style.Font.Italic = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 8].Value = invoice.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        }
        #endregion INVOICE TSG

        #region General Function
        private static void SplitAddress(List<string> addressList, ref string addr1, ref string addr2, ref string addr3 )
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
