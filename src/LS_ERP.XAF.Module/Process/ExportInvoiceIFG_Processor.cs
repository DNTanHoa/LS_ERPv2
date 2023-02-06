using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using Size = LS_ERP.EntityFrameworkCore.Entities.Size;

namespace LS_ERP.XAF.Module.Process
{
    public class ExportInvoiceIFG_Processor
    {
        private static int _maxRow = 1;
        private static Dictionary<string, int> _dicPositionColumnPackingLine;
        private const string CATEGORY_NO = "CATEGORY NO.";
        private const string STYLE_NO = "STYLE NO.";
        private const string PO_NO = "PO. NO.";
        private const string DESCRIPTION = "DESCRIPTION";
        private const string COLOR = "COLOR";
        private const string SIZE = "SIZE";
        private const string QUANTITY = "QUANTITY";
        private const string QUANTITY_PCS_SET = "QUANTITY (PCS/SET)";
        private const string UNIT = "UNIT";
        private const string PRICE_PER_DOZEN = "PRICE PER DOZEN (USD)";
        private const string TOTAL_COST = "TOTAL COST (USD)";

        private const string FROM_NO = "From No";
        private const string TO_NO = "To No";
        private const string NO_CARTON = "No. of Ctns.";
        private const string LABEL = "LABEL";
        private const string COLOR_CODE = "COLOR CODE";
        private const string UNITS_POLYBAG = "Units Per Polybag";
        private const string CASE_PACKING = "Case packing";
        private const string UNITS_CARTON = "Units Per Ctn.";
        private const string TOTAL_UNITS_PCS = "Total Units (PCS / SET)";
        private const string TOTAL_UNITS_DZS = "Total Units (DOZENS)";
        private const string TOTAL_GW = "G.W./ (@)";
        private const string GW_CARTON = "G.W./ (@)/ CTN";
        private const string NW_CARTON = "N.W./ (@)/ CTN";
        private const string TOTAL_NW = "N.W./ (@)";
        private const string LENGTH = "LENGTH";
        private const string WIDTH = "W";
        private const string HEIGHT = "H";
        private const string CARTON_DIMENSION = "Measurement / Dimensions/ (@)";
        private const string VOLUMN_CBM = "Volume (CBM)";

        private static List<string> _sizeList;
        private static string _sizePackingList = "";
        private static int _columnSize = 0;
        private static int _columnEndSize = 0;
        private static Dictionary<int, string> _dicAlphabel;
        private static string _lengthUnit = "";
        private static string _weightUnit = "";
        private static List<ItemStyle> _styleList;

        private static string _cellPKLTotalMeasM3 = "";
        private static string _cellPKLTotalGross = "";
        private static string _cellPKLTotalCarton = "";
        private static string _cellPKLTotalPCS = "";

        public static Stream CreateExcelFile(Invoice invoice, List<Unit> units,
            List<Size> sizes, Stream stream = null)
        {
            string Author = "Leading Star";

            if (!String.IsNullOrEmpty(invoice?.CreatedBy))
            {
                Author = invoice?.CreatedBy;
            }

            string Title = invoice.Code;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                var sheetIndex = 0;
                ExcelPackage excel = excelPackage;

                GetSizeList(invoice, null);
                _dicAlphabel = new Dictionary<int, string>();
                SetDictionaryAlphabet();

                _maxRow = 1;
                excelPackage.Workbook.Properties.Author = Author;
                CreateBookingRequest(excelPackage, invoice, Title, sheetIndex);

                _maxRow = 1;
                sheetIndex++;
                CreateBookingRequestDetails(excelPackage, invoice, Title, sheetIndex);

                _maxRow = 1;
                sheetIndex++;
                CreateBookingSummary(excelPackage, invoice, Title, sizes, sheetIndex);

                _maxRow = 1;
                sheetIndex++;
                CreatePackingList(excelPackage, invoice, Title, units, sizes, sheetIndex);

                _maxRow = 1;
                sheetIndex++;
                CreateInvoice(excelPackage, invoice, Title, units, sheetIndex);

                _maxRow = 1;
                sheetIndex++;
                CreateContainerQuantity(excelPackage, invoice, Title, sheetIndex);

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        private static DataTable SetData(Invoice invoice)
        {
            DataTable data = new DataTable();

            return data;
        }
        //ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex
        public static void CreateInvoice(ExcelPackage excelPackage, Invoice invoice, string Title, List<Unit> units, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Star";
            excelPackage.Workbook.Worksheets.Add("INV");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 13));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateHeaderInvoice(workSheet, invoice);
            CreateHeaderInvoiceDetail(workSheet, invoice);
            FillDataInvoiceDetail(workSheet, invoice, units);

            using (var range = workSheet.Cells["A1:K" + _maxRow])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White);
            }

            // Set page break view
            workSheet.View.PageBreakView = true;
            workSheet.PrinterSettings.FitToPage = true;
            workSheet.Row(_maxRow).PageBreak = true;
            workSheet.Column(11).PageBreak = true;
        }

        private static void CreatePackingList(ExcelPackage excelPackage, Invoice invoice, string Title,
            List<Unit> units, List<Size> sizes, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Start";
            excelPackage.Workbook.Worksheets.Add("PKL");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 13));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            GetSizeList(invoice, null);
            _columnSize = 11;
            _columnEndSize = _columnSize + _sizeList.Count - 1;


            CreateHeaderPL(workSheet, invoice, sizes);
            FillDataPL(workSheet, invoice, units);

            string modelRangeBorder = "A1:"
                                   + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GW]]
                                   + (_maxRow);

            using (var range = workSheet.Cells[modelRangeBorder])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White);
            }

            /// SET DATA for BOOKING REQUEST
            excelPackage.Workbook.Worksheets[0].Cells[36, 7].Formula = _cellPKLTotalGross;
            excelPackage.Workbook.Worksheets[0].Cells[36, 9].Formula = _cellPKLTotalMeasM3;
            excelPackage.Workbook.Worksheets[0].Cells[41, 6].Formula = _cellPKLTotalCarton;
            excelPackage.Workbook.Worksheets[0].Cells[42, 6].Formula = _cellPKLTotalPCS;
            excelPackage.Workbook.Worksheets[0].Cells[52, 6].Formula = _cellPKLTotalMeasM3;

            // Set page break view
            workSheet.View.PageBreakView = true;
            workSheet.PrinterSettings.FitToPage = true;
            workSheet.Row(_maxRow).PageBreak = true;
            workSheet.Column(12).PageBreak = true;
        }

        private static void CreateBookingRequest(ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Start";
            excelPackage.Workbook.Worksheets.Add("BOOKING REQUEST FORM");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 12));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateBookingRequest(workSheet, invoice);
            FillDataBookingRequest(workSheet, invoice);

            using (var range = workSheet.Cells["B2:I" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }

            // Set page break view
            //workSheet.View.PageBreakView = true;
            //workSheet.PrinterSettings.FitToPage = true;
            //workSheet.Row(_maxRow).PageBreak = true;
            //workSheet.Column(12).PageBreak = true;
        }

        private static void CreateBookingRequestDetails(ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Start";
            excelPackage.Workbook.Worksheets.Add("BOOKING REQUEST DETAILS");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 9));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateBookingRequestDetails(workSheet, invoice);

            using (var range = workSheet.Cells["A1:K" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }

            ////Set page break view
            workSheet.View.PageBreakView = true;
            workSheet.PrinterSettings.FitToPage = true;
            workSheet.Row(_maxRow).PageBreak = true;
            workSheet.Column(11).PageBreak = true;
        }

        private static void CreateBookingSummary(ExcelPackage excelPackage, Invoice invoice, string Title, List<Size> sizes, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Start";
            excelPackage.Workbook.Worksheets.Add("BOOKING SUMMARY");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 9));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            GetSizeList(invoice, null);

            CreateBookingSummary(workSheet, invoice, sizes);
        }

        private static void CreateContainerQuantity(ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Start";
            excelPackage.Workbook.Worksheets.Add("CONTAINER QTY FORMAT WM");
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            workSheet.Cells.Style.Font.SetFromFont(new Font("Calibri", 12));
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            CreateContainerQuantityFormatWN(workSheet, invoice);

        }

        #region INVOICE
        private static void CreateHeaderInvoice(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Cells[1, 1].Value = "COMMERCIAL INVOICE";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Black);
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1, 1, 11].Merge = true;
            workSheet.Cells[1, 1, 1, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[1, 1, 1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 10].Value = "No:";
            workSheet.Cells[_maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 11].Value = invoice?.Code;
            workSheet.Cells[_maxRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


            _maxRow++;
            workSheet.Cells[_maxRow, 10].Value = "Date: ";
            workSheet.Cells[_maxRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 11].Value = invoice?.Date;
            workSheet.Cells[_maxRow, 11].Style.Numberformat.Format = "MMM dd yyyy";
            workSheet.Cells[_maxRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Beneficiary: ";
            workSheet.Cells[_maxRow, 2].Value = invoice?.Company?.Name;

            _maxRow++;
            var companyAddress = invoice?.Company?.DisplayAddress.Split(",");

            string address = companyAddress[0];

            for (int i = 1; i < companyAddress.Count(); i++)
            {
                if (i == 3)
                {
                    workSheet.Cells[_maxRow, 2].Value = address.Trim().ToUpper();
                    address = string.Empty;
                    address = companyAddress[i].Trim();
                }
                else if (i == companyAddress.Count() - 1)
                {
                    _maxRow++;
                    address += ", " + companyAddress[i].Trim();
                    workSheet.Cells[_maxRow, 2].Value = address.Trim().ToUpper();
                }
                else
                {
                    address += ", " + companyAddress[i].Trim();
                }
            }
            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "BENEFICIARY ACCOUNT NUMBER: " + invoice.Company?.BankAccount?.BankAccountNumber;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Port of Loading: ";
            workSheet.Cells[_maxRow, 2].Value = invoice?.PortOfLoading?.Name?.ToUpper() + 
                ((invoice?.PortOfLoading?.CountryID ?? 0) != 0 ? ("," + invoice?.PortOfLoading?.Country?.Name?.ToUpper()) : string.Empty);

            workSheet.Cells[_maxRow, 5].Value = "Vessel & Voyage: ";
            workSheet.Cells[_maxRow, 6].Value = invoice?.VesselVoyageNo;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Port of Discharge: ";
            workSheet.Cells[_maxRow, 2].Value = invoice?.PortOfDischarge?.Name?.ToUpper() +
                ((invoice?.PortOfDischarge?.CountryID ?? 0) != 0 ? ("," + invoice?.PortOfDischarge?.Country?.Name?.ToUpper()) : string.Empty); 

            workSheet.Cells[_maxRow, 5].Value = "On board date: ";
            workSheet.Cells[_maxRow, 6].Value = invoice?.OnBoardDate;
            workSheet.Cells[_maxRow, 6].Style.Numberformat.Format = "MMM dd yyyy";
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Payment Term: ";
            workSheet.Cells[_maxRow, 2].Value = invoice?.PaymentTerm?.Description?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Applicant: ";
            workSheet.Cells[_maxRow, 2].Value = invoice?.Consignee?.Name?.ToUpper();

            if (!string.IsNullOrEmpty(invoice.Consignee?.Address))
            {
                var customerAddress = invoice.Consignee?.Address?.Split(",");

                foreach (var item in customerAddress)
                {
                    _maxRow++;
                    workSheet.Cells[_maxRow, 2].Value = item.Trim().ToUpper();
                }
            }


            _maxRow++;

        }

        private static void CreateHeaderInvoiceDetail(ExcelWorksheet workSheet, Invoice invoice)
        {
            int column = 1;
            _maxRow++;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

            workSheet.Cells[_maxRow, column].Value = CATEGORY_NO;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 20;
            _dicPositionColumnPackingLine.Add(CATEGORY_NO, column); // add position column 

            column++;
            workSheet.Cells[_maxRow, column].Value = STYLE_NO;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 17;
            _dicPositionColumnPackingLine.Add(STYLE_NO, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = PO_NO;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 14;
            _dicPositionColumnPackingLine.Add(PO_NO, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = DESCRIPTION;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 35;
            _dicPositionColumnPackingLine.Add(DESCRIPTION, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = COLOR;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 20;
            _dicPositionColumnPackingLine.Add(COLOR, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = SIZE;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 18;
            _dicPositionColumnPackingLine.Add(SIZE, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = QUANTITY;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 15;
            _dicPositionColumnPackingLine.Add(QUANTITY, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = QUANTITY_PCS_SET;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 15;
            _dicPositionColumnPackingLine.Add(QUANTITY_PCS_SET, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = UNIT;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 11;
            _dicPositionColumnPackingLine.Add(UNIT, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = PRICE_PER_DOZEN;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 14;
            _dicPositionColumnPackingLine.Add(PRICE_PER_DOZEN, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = TOTAL_COST;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 17;
            _dicPositionColumnPackingLine.Add(TOTAL_COST, column);

            workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, 11].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            _maxRow++;
        }

        private static void FillDataInvoiceDetail(ExcelWorksheet workSheet, Invoice invoice, List<Unit> units)
        {
            int column = 1;

            //var packingList = invoice?.PackingList?.FirstOrDefault();
            //var itemStyle = packingList?.ItemStyles?.FirstOrDefault();

            var lastCol = _dicPositionColumnPackingLine[TOTAL_COST];

            var distinctStyles = _styleList.Select(x => x.CustomerStyle).Distinct().ToList();
            foreach (var style in distinctStyles)
            {
                var firstStyle = _styleList.FirstOrDefault(i => i.CustomerStyle == style);
                var packingList = invoice?.PackingList?.
                    Where(x => x.ItemStyles.Contains(firstStyle)).FirstOrDefault();

                GetSizeList(invoice, packingList);

                workSheet.Cells[_maxRow, column].Value = firstStyle?.Description.Trim() +
                        ", SIZE: " + _sizePackingList + ", STYLE: " + firstStyle?.CustomerStyle;
                workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                _maxRow++;
            }

            workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, column].Value = "SHIPMENT TERMS: " +
                        _styleList?.FirstOrDefault()?.SalesOrder?.PriceTermCode.ToUpper() + 
                        " " + invoice?.PortOfLoading?.Name?.ToUpper() + 
                        ((invoice?.PortOfLoading?.CountryID ?? 0) != 0 ? (", " + invoice?.PortOfLoading?.Country?.Name?.ToUpper()) : string.Empty) +
                        " " + "INCOTERM 2010.";
            workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, column].Value = "THE FULL NAME AND ADDRESS OF THE MANUFACTURER OF STYLES:";
            workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, column].Value = invoice?.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            var companyAddress = invoice?.Company?.DisplayAddress.Split(",");
            workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            string address = companyAddress[0];

            for (int i = 1; i < companyAddress.Count(); i++)
            {
                if (i == 3)
                {
                    workSheet.Cells[_maxRow, column].Value = address.Trim().ToUpper();
                    address = string.Empty;
                    address = companyAddress[i].Trim();
                    workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
                else if (i == companyAddress.Count() - 1)
                {
                    _maxRow++;
                    address += ", " + companyAddress[i].Trim();
                    workSheet.Cells[_maxRow, column].Value = address.Trim().ToUpper();
                    workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
                else
                {
                    address += ", " + companyAddress[i].Trim();
                }
            }
            _maxRow++;
            workSheet.Cells[_maxRow, column].Value = "BENEFICIARY ACCOUNT NUMBER: " + invoice?.Company?.BankAccount?.BankAccountNumber;
            workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, lastCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            var rowBorder = _maxRow;
            decimal totCost = 0;
            decimal totPCS = 0;
            decimal totDZS = 0;

            foreach (var data in invoice?.InvoiceDetails
                .OrderBy(x => x.CustomerStyle + x.CustomerPurchaseOrderNumber + x.GarmentColorCode))
            {
                var itemStyle = _styleList?
                    .FirstOrDefault(i => i.CustomerStyle == data.CustomerStyle &&
                                        i.PurchaseOrderNumber == data.CustomerPurchaseOrderNumber &&
                                        i.ColorCode == data.GarmentColorCode);
                var unit = new Unit();
                if(string.IsNullOrEmpty(itemStyle.UnitID))
                {
                    unit = units.FirstOrDefault(x => x.ID == "DZ");
                }
                else
                {
                    unit = units.FirstOrDefault(x => x.ID == itemStyle.UnitID);
                }

                var packingList = invoice?.PackingList?.FirstOrDefault(p => p.ItemStyles.Contains(itemStyle));
                GetSizeList(invoice, packingList);

                column = 1;
                workSheet.Cells[_maxRow, column].Value = "";

                column++;
                workSheet.Cells[_maxRow, column].Value = data?.CustomerStyle;

                column++;
                workSheet.Cells[_maxRow, column].Value = data?.CustomerPurchaseOrderNumber;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = data?.Description;

                column++;
                workSheet.Cells[_maxRow, column].Value = data?.GarmentColorName;

                column++;
                workSheet.Cells[_maxRow, column].Value = _sizePackingList;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = data?.Quantity;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat
                    .Format = decimal.Parse(data?.Quantity.ToString().Split(".")[1]) == 0 ? "#,##0" : "#,##0.00";


                column++;
                workSheet.Cells[_maxRow, column].Value = data?.Quantity * unit.Factor;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0";

                column++;
                workSheet.Cells[_maxRow, column].Value = unit?.FullName?.ToUpper();
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                column++;
                workSheet.Cells[_maxRow, column].Value = data?.UnitPrice;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.000";

                column++;
                workSheet.Cells[_maxRow, column].Value = data?.Amount;
                workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.000";

                totCost += (decimal)(data?.Amount ?? 0);
                totPCS += (decimal)(data?.Quantity * unit.Factor); 
                totDZS += (decimal)data?.Quantity;

                _maxRow++;
            }

            workSheet.Cells[rowBorder, 1, _maxRow, column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[rowBorder, 1, _maxRow, column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[rowBorder, 1, _maxRow, column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[rowBorder, 1, _maxRow, column].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            _maxRow++;

            workSheet.Cells[_maxRow, 1].Value = "TOTAL:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[SIZE]].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[SIZE]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Value = totDZS;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Style.Numberformat.Format = (totPCS % 12 == 0) ? "#,##0" : "#,##0.00";

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY_PCS_SET]].Value = totPCS;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY_PCS_SET]].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY_PCS_SET]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY_PCS_SET]].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Value = totCost;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Style.Numberformat.Format = "#,##0.000";

            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SAY:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 2].Value = NumberToWordsHelpers.ToVerbalCurrency((double)totCost); ;
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            _maxRow += 3;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = invoice?.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR], _maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Merge = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR], _maxRow, _dicPositionColumnPackingLine[TOTAL_COST]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        }
        #endregion INVOICE

        #region PACKINGLIST
        private static void CreateHeaderPL(ExcelWorksheet workSheet, Invoice invoice, List<Size> sizes)
        {
            _dicPositionColumnPackingLine = new Dictionary<string, int>();
            var packingUnit = invoice?.PackingList?.FirstOrDefault().PackingUnit;
            if (packingUnit != null)
            {
                _lengthUnit = packingUnit?.LengthUnit;
                _weightUnit = packingUnit?.WeightUnit;
            }
            else
            {
                _lengthUnit = "Cm";
                _weightUnit = "Kgs";
            }

            workSheet.Cells[_maxRow, 1].Value = invoice?.Company?.Name.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 18;
            workSheet.Cells[_maxRow, 1, _maxRow, _columnEndSize + 13].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _columnEndSize + 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice?.Company?.DisplayAddress.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 18;
            workSheet.Cells[_maxRow, 1, _maxRow, _columnEndSize + 13].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _columnEndSize + 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "PACKING LIST";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 20;
            workSheet.Cells[_maxRow, 1, _maxRow, _columnEndSize + 13].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _columnEndSize + 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Applicant:";
            workSheet.Cells[_maxRow, 4].Value = invoice?.Consignee?.Name?.ToUpper();

            workSheet.Cells[_maxRow, _columnEndSize + 1].Value = "Invoice No. :";
            workSheet.Cells[_maxRow, _columnEndSize + 3].Value = invoice?.Code;

            var customerAddress = invoice?.Consignee?.Address.Split(",");
            int i = 1;
            foreach (var item in customerAddress)
            {
                workSheet.Cells[_maxRow + i, 4].Value = item.Trim().ToUpper();

                i++;
            }

            _maxRow++;
            workSheet.Cells[_maxRow, _columnEndSize + 1].Value = "Invoice Date :";
            workSheet.Cells[_maxRow, _columnEndSize + 3].Value = invoice?.Date;
            workSheet.Cells[_maxRow, _columnEndSize + 3].Style.Numberformat.Format = "MMM dd yyyy";
            workSheet.Cells[_maxRow, _columnEndSize + 3, _maxRow, _columnEndSize + 4].Merge = true;
            workSheet.Cells[_maxRow, _columnEndSize + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


            _maxRow++;
            workSheet.Cells[_maxRow, _columnEndSize + 1].Value = "Payment Term :";
            workSheet.Cells[_maxRow, _columnEndSize + 3].Value = invoice?.PaymentTerm?.Description?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, _columnEndSize + 1].Value = "Port of Loading :";
            workSheet.Cells[_maxRow, _columnEndSize + 3].Value = invoice?.PortOfLoading?.Name?.ToUpper() +
                ((invoice?.PortOfLoading?.CountryID ?? 0) != 0 ? (", " + invoice?.PortOfLoading?.Country?.Name?.ToUpper()) : string.Empty);

            _maxRow++;
            workSheet.Cells[_maxRow, _columnEndSize + 1].Value = "Port of Discharge :";
            workSheet.Cells[_maxRow, _columnEndSize + 3].Value = invoice?.PortOfDischarge?.Name?.ToUpper() +
                ((invoice?.PortOfDischarge?.CountryID ?? 0) != 0 ? ("," + invoice?.PortOfDischarge?.Country?.Name?.ToUpper()) : string.Empty);

            _maxRow++;
            workSheet.Cells[_maxRow, _columnEndSize + 1].Value = "Vessel & Voyage :";
            workSheet.Cells[_maxRow, _columnEndSize + 3].Value = invoice?.VesselVoyageNo?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, _columnEndSize + 1].Value = "On board date:";
            workSheet.Cells[_maxRow, _columnEndSize + 3].Value = invoice?.OnBoardDate;
            workSheet.Cells[_maxRow, _columnEndSize + 3].Style.Numberformat.Format = "MMM dd yyyy";
            workSheet.Cells[_maxRow, _columnEndSize + 3, _maxRow, _columnEndSize + 4].Merge = true;
            workSheet.Cells[_maxRow, _columnEndSize + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            /// CREATE HEADER PACKING LIST
            int column = 1;
            _maxRow++;

            string range = "A" + _maxRow + ":C" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = "Ctn. No.";
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(FROM_NO, 1); // add position column 
            _dicPositionColumnPackingLine.Add(TO_NO, 3);
            workSheet.Column(column).Width = 7.9;
            workSheet.Column(column + 1).Width = 2.5;
            workSheet.Column(column + 2).Width = 7.9;

            column += 3;
            range = "D" + _maxRow + ":D" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = NO_CARTON;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(NO_CARTON, column);
            workSheet.Column(column).Width = 13.9;

            column++;
            range = "E" + _maxRow + ":E" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = STYLE_NO;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(STYLE_NO, column);
            workSheet.Column(column).Width = 16.5;

            column++;
            range = "F" + _maxRow + ":F" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = PO_NO;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(PO_NO, column);
            workSheet.Column(column).Width = 11.9;

            column++;
            range = "G" + _maxRow + ":G" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = DESCRIPTION;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(DESCRIPTION, column);
            workSheet.Column(column).Width = 41.8;

            column++;
            range = "H" + _maxRow + ":H" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(COLOR, column);
            workSheet.Column(column).Width = 19.9;

            column++;
            range = "I" + _maxRow + ":I" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR_CODE;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(COLOR_CODE, column);
            workSheet.Column(column).Width = 13.6;

            column++;
            range = "J" + _maxRow + ":J" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = LABEL;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(LABEL, column);
            workSheet.Column(column).Width = 11.4;

            column++;
            range = "K" + _maxRow + ":" + _dicAlphabel[_columnEndSize] + _maxRow;
            workSheet.Cells[_maxRow, column].Value = SIZE;
            workSheet.Cells[range].Merge = true;
            workSheet.Row(_maxRow).Height = 50;

            var invoiceSizes = sizes.Where(x => _sizeList.Contains(x.Code))
                                .OrderBy(x => x.SequeneceNumber).ToList();
            foreach (var size in invoiceSizes)
            {
                if (!string.IsNullOrEmpty(size.Code))
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size.Code;
                    workSheet.Cells[_maxRow + 1, column].Style.WrapText = true;
                    _dicPositionColumnPackingLine.Add(size.Code, column);
                    workSheet.Column(column).Width = 8.5;

                    column++;
                }
            }

            string colChar = "";
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = UNITS_POLYBAG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(UNITS_POLYBAG, column);
                workSheet.Column(column).Width = 12.5;
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = CASE_PACKING;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(CASE_PACKING, column);
                workSheet.Column(column).Width = 8.5;
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = UNITS_CARTON;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(UNITS_CARTON, column);
                workSheet.Column(column).Width = 10.5;
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS_PCS;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(TOTAL_UNITS_PCS, column);
                workSheet.Column(column).Width = 17.2;
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS_DZS;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(TOTAL_UNITS_DZS, column);
                workSheet.Column(column).Width = 19.2;
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NW_CARTON.Replace("@", _weightUnit);
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(NW_CARTON, column);
                workSheet.Column(column).Width = 9;
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = GW_CARTON.Replace("@", _weightUnit);
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(GW_CARTON, column);
                workSheet.Column(column).Width = 9;
            }


            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar) &&
                _dicAlphabel.TryGetValue(column + 2, out string colEndChar))
            {
                range = colChar + _maxRow + ":" + colEndChar + _maxRow;
                workSheet.Cells[_maxRow, column].Value = CARTON_DIMENSION.Replace("@", _lengthUnit);
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;


                workSheet.Cells[_maxRow + 1, column].Value = "L";
                _dicPositionColumnPackingLine.Add(LENGTH, column);
                workSheet.Column(column).Width = 7.9;
                workSheet.Row(_maxRow + 1).Height = 50;

                column++;
                workSheet.Cells[_maxRow + 1, column].Value = WIDTH;
                _dicPositionColumnPackingLine.Add(WIDTH, column);
                workSheet.Column(column).Width = 7.9;

                column++;
                workSheet.Cells[_maxRow + 1, column].Value = HEIGHT;
                _dicPositionColumnPackingLine.Add(HEIGHT, column);
                workSheet.Column(column).Width = 7.9;

            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = VOLUMN_CBM;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(VOLUMN_CBM, column);
                workSheet.Column(column).Width = 10.1;
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_NW.Replace("@", _weightUnit);
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(TOTAL_NW, column);
                workSheet.Column(column).Width = 19;
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + _maxRow + ":" + colChar + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_GW.Replace("@", _weightUnit);
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(TOTAL_GW, column);
                workSheet.Column(column).Width = 19;
            }

            workSheet.Cells[_maxRow, 1, _maxRow + 1, column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow + 1, column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow + 1, column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow + 1, column].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow += 2;
        }
        private static void FillDataPL(ExcelWorksheet workSheet, Invoice invoice, List<Unit> units)
        {
            var inchConvertValue = (decimal)(1728 * 35.31);
            var lbConvertValue = (decimal)2.2046;
            var unit = units.FirstOrDefault(x => x.ID == "DZ");
            decimal grandTotCarton = 0;
            decimal grandTotPCS = 0;
            decimal grandTotDZS = 0;
            decimal grandTotMeasM3 = 0;
            decimal grandTotNW = 0;
            decimal grandTotGW = 0;

            var rowBorder = _maxRow;
            foreach (var packinglist in invoice?.PackingList
                .OrderBy(x => x?.ItemStyles?.FirstOrDefault()?.CustomerStyle +
                              x?.ItemStyles?.FirstOrDefault()?.PurchaseOrderNumber +
                              x?.ItemStyles?.FirstOrDefault()?.ColorCode))
            {
                decimal netWeight = 0;
                decimal grossWeight = 0;
                decimal measM3 = 0;
                bool firstRow = true;
                int fromNo = 0;
                int summaryCarton = 0;
                string styleIFG = "";
                string colorDescrIFG = "";
                string descriptionIFG = "";
                bool isSolidSize = false;
                string lsStyle = "";
                var itemStyle = new ItemStyle();
                var mergeCell = new List<int>();
                var styles = packinglist.ItemStyles.OrderBy(x => x.LSStyle).ToList();
                var totalStyle = packinglist.PackingLines.Select(x => x.LSStyle).Distinct().Count();
                decimal totCarton = 0;
                decimal totPCS = 0;
                decimal totDZS = 0;
                decimal totMeasM3 = 0;
                decimal totNW = 0;
                decimal totGW = 0;

                if (packinglist?.ItemStyles.Count == 1)
                {
                    var style = packinglist.ItemStyles.FirstOrDefault();
                    var sortPackingLines = packinglist.PackingLines.OrderBy(x => x.SequenceNo).ToList();

                    // Fill Data 
                    foreach (var packingLine in sortPackingLines)
                    {
                        if (packingLine.PrePack == "Assorted Size - Solid Color")
                        {
                            if (firstRow || fromNo != packingLine.FromNo)
                            {
                                if (fromNo != packingLine.FromNo && !firstRow)
                                {
                                    _maxRow++;
                                }
                                if (styleIFG.Length == 0)
                                {
                                    styleIFG = style.CustomerStyle;
                                }
                                if (colorDescrIFG.Length == 0)
                                {
                                    colorDescrIFG = style.ColorName;
                                }

                                /// Convert packing unit
                                if (_lengthUnit.Trim().ToUpper() == "INCH")
                                {
                                    measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                        (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                                }
                                else
                                {
                                    measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                                  packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                                }

                                if (_weightUnit.Trim().ToUpper() == "LBS")
                                {
                                    netWeight = (decimal)packingLine.NetWeight * (decimal)lbConvertValue;
                                    grossWeight = (decimal)packingLine.GrossWeight * (decimal)lbConvertValue;
                                }
                                else
                                {
                                    netWeight = (decimal)packingLine.NetWeight;
                                    grossWeight = (decimal)packingLine.GrossWeight;
                                }

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Value = packingLine.TotalCarton;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_NO]].Value = style.CustomerStyle;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PO_NO]].Value = style.PurchaseOrderNumber;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION]].Value = style.Description;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = style.ColorName;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE]]
                                    .Value = style.Barcodes.Where(x => x.Size == packingLine.Size).ToList().Count > 0 ?
                                        style.Barcodes.FirstOrDefault(x => x.Size == packingLine.Size).Color : style.ColorCode;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LABEL]].Value = style.LabelCode;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Value = packingLine.QuantityPerPackage;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CASE_PACKING]].Value = packingLine.PackagesPerBox;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CASE_PACKING]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Style.Numberformat.Format = "#,##0";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Value
                                        = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Numberformat.Format = "#,##0";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_DZS]].Value
                                        = packingLine.QuantityPerCarton * packingLine.TotalCarton / unit?.Factor ?? 1;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_DZS]].Style.Numberformat.Format 
                                        = (packingLine.QuantityPerCarton * packingLine.TotalCarton) % (unit?.Factor ?? 1) == 0 ? "#,##0" : "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Value
                                        = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Style.Numberformat.Format = "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Value
                                        = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Style.Numberformat.Format = "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = Math.Round((decimal)packingLine.Length, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = Math.Round((decimal)packingLine.Width, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = Math.Round((decimal)packingLine.Height, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[VOLUMN_CBM]].Value = Math.Round(measM3, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[VOLUMN_CBM]].Style.Numberformat.Format = "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Value
                                        = Math.Round(netWeight, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Style.Numberformat.Format = "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Value
                                        = Math.Round(grossWeight, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Style.Numberformat.Format = "#,##0.00";

                                totCarton += (decimal)packingLine.TotalCarton;
                                totPCS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                totDZS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton / unit?.Factor ?? 1);
                                totMeasM3 += Math.Round(measM3, 2);
                                totNW += Math.Round(netWeight, 3);
                                totGW += Math.Round(grossWeight, 3);

                                firstRow = false;
                                fromNo = (int)packingLine.FromNo;
                            }
                            else
                            {
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                totPCS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                totDZS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton / unit?.Factor ?? 1);
                                fromNo = (int)packingLine.FromNo;
                            }
                        }
                        else
                        {
                            if (packingLine.TotalCarton == 0)
                            {
                                continue;
                            }
                            if (firstRow)
                            {
                                if (styleIFG.Length == 0)
                                {
                                    styleIFG = style.CustomerStyle;
                                }
                                if (colorDescrIFG.Length == 0)
                                {
                                    colorDescrIFG = style.ColorName;
                                }

                                firstRow = false;
                            }

                            /// Convert packing unit
                            if (_lengthUnit.Trim().ToUpper() == "INCH")
                            {
                                measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                        (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                            }
                            else
                            {
                                measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                              packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                            }

                            if (_weightUnit.Trim().ToUpper() == "LBS")
                            {
                                netWeight = (decimal)packingLine.NetWeight * (decimal)lbConvertValue;
                                grossWeight = (decimal)packingLine.GrossWeight * (decimal)lbConvertValue;
                            }
                            else
                            {
                                netWeight = (decimal)packingLine.NetWeight;
                                grossWeight = (decimal)packingLine.GrossWeight;
                            }

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Style.Numberformat.Format = "#,##0";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Style.Numberformat.Format = "#,##0";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Value = packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Style.Numberformat.Format = "#,##0";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_NO]].Value = style.CustomerStyle;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PO_NO]].Value = style.PurchaseOrderNumber;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION]].Value = style.Description;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = style.ColorName;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE]]
                                .Value = style.Barcodes.Where(x => x.Size == packingLine.Size).ToList().Count > 0 ?
                                    style.Barcodes.FirstOrDefault(x => x.Size == packingLine.Size).Color : style.ColorCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LABEL]].Value = style.LabelCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Value
                                        = packingLine.QuantityPerPackage == 0 ? packingLine.QuantityPerCarton : packingLine.QuantityPerPackage;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Style.Numberformat.Format = "#,##0";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CASE_PACKING]].Value
                                        = packingLine.PackagesPerBox == 0 ? 1 : packingLine.PackagesPerBox;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CASE_PACKING]].Style.Numberformat.Format = "#,##0";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Style.Numberformat.Format = "#,##0";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Value
                                        = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Numberformat.Format = "#,##0";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_DZS]].Value
                                    = packingLine.QuantityPerCarton * packingLine.TotalCarton / unit?.Factor ?? 1;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_DZS]].Style.Numberformat.Format //= "#,##0";
                                    = (packingLine.QuantityPerCarton * packingLine.TotalCarton) % (unit?.Factor ?? 1) == 0 ? "#,##0" : "#,##0.00";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Value
                                    = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Style.Numberformat.Format = "#,##0.00";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Value
                                    = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Style.Numberformat.Format = "#,##0.00";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = Math.Round((decimal)packingLine.Length, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = Math.Round((decimal)packingLine.Width, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = Math.Round((decimal)packingLine.Height, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[VOLUMN_CBM]].Value = Math.Round(measM3, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[VOLUMN_CBM]].Style.Numberformat.Format = "#,##0.00";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Value
                                    = Math.Round(netWeight, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Style.Numberformat.Format = "#,##0.00";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Value
                                    = Math.Round(grossWeight, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Style.Numberformat.Format = "#,##0.00";

                            totCarton += (decimal)packingLine.TotalCarton;
                            totPCS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                            totDZS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton / unit?.Factor ?? 1);
                            totMeasM3 += Math.Round(measM3, 2);
                            totNW += Math.Round(netWeight, 3);
                            totGW += Math.Round(grossWeight, 3);

                            isSolidSize = true;

                            _maxRow++;
                        }
                    }

                    _maxRow++;
                }
                else
                {
                    foreach (var style in styles)
                    {
                        if (styleIFG.Length == 0)
                        {
                            styleIFG = style.CustomerStyle.Trim();
                        }
                        else if (!styleIFG.Contains(style.CustomerStyle.Trim()))
                        {
                            var customerStyles = style.CustomerStyle.Trim().ToCharArray();
                            var index = 0;
                            for (int i = customerStyles.Length - 1; i >= 0; i--)
                            {
                                if (!int.TryParse(customerStyles[i].ToString(), out int value))
                                {
                                    index = i;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            styleIFG += "/" + style.CustomerStyle.Substring(index, style.CustomerStyle.Length - index);
                        }

                        if (colorDescrIFG.Length == 0)
                        {
                            colorDescrIFG = style.ColorName.Trim();
                        }
                        else if (!colorDescrIFG.Contains(style.ColorName.Trim()))
                        {
                            colorDescrIFG += "/ " + style.ColorName.Trim();
                        }

                        if (descriptionIFG.Length == 0)
                        {
                            descriptionIFG = style.Description.Trim();
                        }
                        else
                        {
                            descriptionIFG += "/ " + style.Description.Trim();
                        }

                        if (_weightUnit.Trim().ToUpper() == "LBS")
                        {
                            netWeight += (decimal)packinglist?.PackingLines?.
                                FirstOrDefault(x => x.LSStyle == style.LSStyle)?.NetWeight * (decimal)lbConvertValue;
                        }
                        else
                        {
                            netWeight += (decimal)packinglist?.PackingLines?.
                                FirstOrDefault(x => x.LSStyle == style.LSStyle)?.NetWeight;
                        }
                    }

                    var sortPackingLines = packinglist.PackingLines.OrderBy(x => x.SequenceNo)
                                                .OrderByDescending(x => x.TotalCarton).ToList();
                    summaryCarton = (int)sortPackingLines[0].TotalCarton;
                    lsStyle = sortPackingLines[0].LSStyle;
                    foreach (var packingLine in sortPackingLines)
                    {
                        itemStyle = packinglist.ItemStyles.FirstOrDefault(x => x.LSStyle == packingLine.LSStyle);
                        if (packingLine.TotalCarton != summaryCarton)
                        {
                            firstRow = true;
                            lsStyle = packingLine.LSStyle;
                            _maxRow++;
                        }
                        if (firstRow || packingLine.LSStyle == lsStyle)
                        {
                            if (firstRow)
                            {
                                /// Convert packing unit
                                if (_lengthUnit.Trim().ToUpper() == "INCH")
                                {
                                    measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                        (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                                }
                                else
                                {
                                    measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                                  packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                                }

                                if (_weightUnit.Trim().ToUpper() == "LBS")
                                {
                                    grossWeight = netWeight + (decimal)((packingLine?.GrossWeight - packingLine?.NetWeight) * (decimal)lbConvertValue);
                                }
                                else
                                {
                                    grossWeight = (decimal)(netWeight + packingLine?.GrossWeight - packingLine?.NetWeight);
                                }

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Value = packingLine.TotalCarton;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_CARTON]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_NO]].Value = itemStyle.CustomerStyle;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PO_NO]].Value = itemStyle.PurchaseOrderNumber;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION]].Value = itemStyle.Description;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE]]
                                    .Value = itemStyle.Barcodes.Where(x => x.Size == packingLine.Size).ToList().Count > 0 ?
                                        itemStyle.Barcodes.FirstOrDefault(x => x.Size == packingLine.Size).Color : itemStyle.ColorCode;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LABEL]].Value = itemStyle.LabelCode;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Value = packingLine.QuantityPerPackage;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CASE_PACKING]].Value = packingLine.PackagesPerBox;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CASE_PACKING]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Style.Numberformat.Format = "#,##0";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Value
                                        = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Numberformat.Format = "#,##0";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_DZS]].Value
                                        = packingLine.QuantityPerCarton * packingLine.TotalCarton / unit?.Factor ?? 1;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_DZS]].Style.Numberformat.Format //= "#,##0";
                                        = (packingLine.QuantityPerCarton * packingLine.TotalCarton) % (unit?.Factor ?? 1) == 0 ? "#,##0" : "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Value
                                        = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Style.Numberformat.Format = "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Value
                                        = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Style.Numberformat.Format = "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = Math.Round((decimal)packingLine.Length, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = Math.Round((decimal)packingLine.Width, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = Math.Round((decimal)packingLine.Height, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[VOLUMN_CBM]].Value = Math.Round(measM3, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[VOLUMN_CBM]].Style.Numberformat.Format = "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Value
                                        = Math.Round(netWeight, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Style.Numberformat.Format = "#,##0.00";

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Value
                                        = Math.Round(grossWeight, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Style.Numberformat.Format = "#,##0.00";

                                totCarton += (decimal)packingLine.TotalCarton;
                                totPCS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                totDZS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton / unit?.Factor ?? 1);
                                totMeasM3 += Math.Round(measM3, 2);
                                totNW += Math.Round(netWeight, 3);
                                totGW += Math.Round(grossWeight, 3);

                                mergeCell.Add(_maxRow);
                                firstRow = false;
                                summaryCarton = (int)packingLine.TotalCarton;
                                lsStyle = packingLine.LSStyle;
                            }
                            else
                            {
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                                decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                                totPCS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                totDZS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton / unit?.Factor ?? 1);

                                summaryCarton = (int)packingLine.TotalCarton;
                                lsStyle = packingLine.LSStyle;
                            }

                        }
                        else
                        {
                            _maxRow++;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_NO]].Value = itemStyle.CustomerStyle;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PO_NO]].Value = itemStyle.PurchaseOrderNumber;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION]].Value = itemStyle.Description;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LABEL]].Value = itemStyle.LabelCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Value = packingLine.QuantityPerPackage;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Style.Numberformat.Format = "#,##0";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CASE_PACKING]].Value = packingLine.PackagesPerBox;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CASE_PACKING]].Style.Numberformat.Format = "#,##0";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Style.Numberformat.Format = "#,##0";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Value
                                        = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Numberformat.Format = "#,##0";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_DZS]].Value
                                    = packingLine.QuantityPerCarton * packingLine.TotalCarton / unit?.Factor ?? 1;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_DZS]].Style.Numberformat.Format // = "#,##0";
                                = (packingLine.QuantityPerCarton * packingLine.TotalCarton) % (unit?.Factor ?? 1) == 0 ? "#,##0" : "#,##0.00";

                            totPCS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                            totDZS += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton / unit?.Factor ?? 1);

                            summaryCarton = (int)packingLine.TotalCarton;
                            lsStyle = packingLine.LSStyle;
                        }
                    }

                    _maxRow++;
                }

                if (isSolidSize)
                {
                    _maxRow--;
                }

                foreach (var merge in mergeCell)
                {
                    workSheet.Cells["A" + merge + ":" + "A" + (merge + (totalStyle - 1))].Merge = true;
                    workSheet.Cells["B" + merge + ":" + "B" + (merge + (totalStyle - 1))].Merge = true;
                    workSheet.Cells["C" + merge + ":" + "C" + (merge + (totalStyle - 1))].Merge = true;
                    workSheet.Cells["D" + merge + ":" + "D" + (merge + (totalStyle - 1))].Merge = true;
                    workSheet.Cells["I" + merge + ":" + "I" + (merge + (totalStyle - 1))].Merge = true;
                    workSheet.Cells["J" + merge + ":" + "J" + (merge + (totalStyle - 1))].Merge = true;

                    workSheet.Cells[merge, _dicPositionColumnPackingLine[NW_CARTON],
                                   (merge + (totalStyle - 1)), _dicPositionColumnPackingLine[NW_CARTON]].Merge = true;
                    workSheet.Cells[merge, _dicPositionColumnPackingLine[GW_CARTON],
                                   (merge + (totalStyle - 1)), _dicPositionColumnPackingLine[GW_CARTON]].Merge = true;
                    workSheet.Cells[merge, _dicPositionColumnPackingLine[LENGTH],
                                   (merge + (totalStyle - 1)), _dicPositionColumnPackingLine[LENGTH]].Merge = true;
                    workSheet.Cells[merge, _dicPositionColumnPackingLine[WIDTH],
                                   (merge + (totalStyle - 1)), _dicPositionColumnPackingLine[WIDTH]].Merge = true;
                    workSheet.Cells[merge, _dicPositionColumnPackingLine[HEIGHT],
                                   (merge + (totalStyle - 1)), _dicPositionColumnPackingLine[HEIGHT]].Merge = true;
                    workSheet.Cells[merge, _dicPositionColumnPackingLine[VOLUMN_CBM],
                                   (merge + (totalStyle - 1)), _dicPositionColumnPackingLine[VOLUMN_CBM]].Merge = true;
                    workSheet.Cells[merge, _dicPositionColumnPackingLine[TOTAL_NW],
                                   (merge + (totalStyle - 1)), _dicPositionColumnPackingLine[TOTAL_NW]].Merge = true;
                    workSheet.Cells[merge, _dicPositionColumnPackingLine[TOTAL_GW],
                                   (merge + (totalStyle - 1)), _dicPositionColumnPackingLine[TOTAL_GW]].Merge = true;
                }



                ///SET DATA TOTAL PACKING LINE
                workSheet.Cells[_maxRow, 1].Value = "TOTAL:";
                workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, 1, _maxRow, 3].Merge = true;
                workSheet.Cells[_maxRow, 1, _maxRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, 4].Value = String.Format("{0:n0}", totCarton) + " CTNS";
                workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                grandTotCarton += totCarton;

                workSheet.Cells[_maxRow, 7].Value = "CONTAINER NUMBER: " + invoice?.ContainerNo;
                workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, 7, _maxRow, 8].Merge = true;
                workSheet.Cells[_maxRow, 7, _maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                workSheet.Cells[_maxRow, _columnEndSize + 4].Value = String.Format("{0:n0}", totPCS) + " PCS";
                workSheet.Cells[_maxRow, _columnEndSize + 4].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, _columnEndSize + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                grandTotPCS += totPCS;

                workSheet.Cells[_maxRow, _columnEndSize + 5].Value 
                        = (totPCS % 12 == 0 ? String.Format("{0:n0}", totDZS) : String.Format("{0:n}", totDZS)) + " DOZENS";
                workSheet.Cells[_maxRow, _columnEndSize + 5].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, _columnEndSize + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                grandTotDZS += totDZS;

                workSheet.Cells[_maxRow, _columnEndSize + 11].Value = totMeasM3;
                workSheet.Cells[_maxRow, _columnEndSize + 11].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, _columnEndSize + 11].Style.Numberformat.Format = "#,##0.00";
                workSheet.Cells[_maxRow, _columnEndSize + 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                grandTotMeasM3 += totMeasM3;

                workSheet.Cells[_maxRow, _columnEndSize + 12].Value = String.Format("{0:n}", totNW) + " " + _weightUnit;
                workSheet.Cells[_maxRow, _columnEndSize + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, _columnEndSize + 12].Style.Font.Bold = true;
                grandTotNW += totNW;

                workSheet.Cells[_maxRow, _columnEndSize + 13].Value = String.Format("{0:n}", totGW) + " " + _weightUnit;
                workSheet.Cells[_maxRow, _columnEndSize + 13].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, _columnEndSize + 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                grandTotGW += totGW;

                _maxRow++;
            }

            ///SET DATA GRAND TOTAL PACKING LINE
            workSheet.Cells[_maxRow, 1].Value = "GRAND TOTAL:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 3].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow, 4].Value = String.Format("{0:n0}", grandTotCarton) + " CTNS";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            _cellPKLTotalCarton = "PKL!" + _dicAlphabel[_dicPositionColumnPackingLine[NO_CARTON]] + _maxRow;

            workSheet.Cells[_maxRow, _columnEndSize + 4].Value = String.Format("{0:n0}", grandTotPCS) + " PCS";
            workSheet.Cells[_maxRow, _columnEndSize + 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _columnEndSize + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            _cellPKLTotalPCS = "PKL!" + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_UNITS_PCS]] + _maxRow;

            workSheet.Cells[_maxRow, _columnEndSize + 5].Value //= String.Format("{0:n0}", grandTotDZS) + " DOZENS";
                = (grandTotPCS % 12 == 0 ? String.Format("{0:n0}", grandTotDZS) : String.Format("{0:n}", grandTotDZS)) + " DOZENS";
            workSheet.Cells[_maxRow, _columnEndSize + 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _columnEndSize + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, _columnEndSize + 11].Value = grandTotMeasM3;
            workSheet.Cells[_maxRow, _columnEndSize + 11].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _columnEndSize + 11].Style.Numberformat.Format = "#,##0.00";
            workSheet.Cells[_maxRow, _columnEndSize + 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            _cellPKLTotalMeasM3 = "PKL!" + _dicAlphabel[_dicPositionColumnPackingLine[VOLUMN_CBM]] + _maxRow;

            workSheet.Cells[_maxRow, _columnEndSize + 12].Value = String.Format("{0:n}", grandTotNW) + " " + _weightUnit;
            workSheet.Cells[_maxRow, _columnEndSize + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _columnEndSize + 12].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, _columnEndSize + 13].Value = String.Format("{0:n}", grandTotGW) + " " + _weightUnit;
            workSheet.Cells[_maxRow, _columnEndSize + 13].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _columnEndSize + 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ///  SET BODER 
            string rangBoder = "A" + rowBorder + ":"
                                + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GW]] + _maxRow;

            using (var range = workSheet.Cells[rangBoder])
            {
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 3].Value = "SAY TOTAL:";
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 5].Value = NumberToWordsHelpers.ToVerbalCurrency((double)grandTotCarton, "CARTONS");
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, _columnEndSize + 1].Value = invoice?.Company?.Name?.ToUpper();
            workSheet.Cells[_maxRow, _columnEndSize + 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _columnEndSize + 1, _maxRow, _columnEndSize + 10].Merge = true;
            workSheet.Cells[_maxRow, _columnEndSize + 1, _maxRow, _columnEndSize + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow + 1, _columnEndSize + 13].Value = grandTotGW;
            _cellPKLTotalGross = "PKL!" + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GW]] + (_maxRow + 1);
            workSheet.Row(_maxRow + 1).Hidden = true;
        }
        #endregion PACKINGLIST

        #region BOOKING REQUEST
        private static void CreateBookingRequest(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Column(1).Width = 3.5;
            workSheet.Column(2).Width = 14.5;
            workSheet.Column(3).Width = 21;
            workSheet.Column(4).Width = 22.3;
            workSheet.Column(5).Width = 3.5;
            workSheet.Column(6).Width = 14.9;
            workSheet.Column(7).Width = 17.8;
            workSheet.Column(8).Width = 16.8;
            workSheet.Column(9).Width = 21.3;

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "SHIPPER (Full Name & Address)";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 5].Value = "SHIPPING ORDER";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 8].Value = "S/O No.";

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = invoice?.Company?.Name?.ToUpper();

            workSheet.Cells[_maxRow, 8].Value = "Consol Code No.";

            workSheet.Cells["H2:I3"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["H2:I3"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["H2:I3"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["H2:I3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            var companyAddress = invoice?.Company?.DisplayAddress.Split(",");
            string address = companyAddress[0];

            for (int i = 1; i < companyAddress.Count(); i++)
            {
                if (i == 3)
                {
                    workSheet.Cells[_maxRow, 2].Value = address.Trim().ToUpper();
                    address = string.Empty;
                    address = companyAddress[i].Trim();
                }
                else if (i == companyAddress.Count() - 1)
                {
                    _maxRow++;
                    address += ", " + companyAddress[i].Trim();
                    workSheet.Cells[_maxRow, 2].Value = address.Trim().ToUpper();
                }
                else
                {
                    address += ", " + companyAddress[i].Trim();
                }
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "BENEFICIARY ACCOUNT NUMBER: " + invoice.Company?.BankAccount?.BankAccountNumber;

            _maxRow += 3;
            using (var range = workSheet.Cells["B2:D" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "CONSIGNEE (If Order, please state Notify Party)";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            workSheet.Cells[_maxRow + 1, 2].Value = invoice?.Consignee?.Name?.ToUpper();

            var customerAddress = invoice?.Consignee?.Address.Split(",");
            var j = 2;
            foreach (var item in customerAddress)
            {
                workSheet.Cells[_maxRow + j, 2].Value = item.Trim().ToUpper();

                j++;
            }

            var checkBox1 = workSheet.Drawings.AddCheckBoxControl("checkBox1");
            checkBox1.Text = "";
            checkBox1.SetPosition(190, 450);
            checkBox1.SetSize(50);
            checkBox1.Checked = OfficeOpenXml.Drawing.Controls.eCheckState.Checked;
            checkBox1.Print = true;

            workSheet.Cells[_maxRow, 6].Value = "    Hochiminh";
            workSheet.Cells[_maxRow, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 6].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            var checkBox2 = workSheet.Drawings.AddCheckBoxControl("checkBox2");
            checkBox2.Text = "";
            checkBox2.SetPosition(190, 555);
            checkBox2.SetSize(50);
            checkBox2.Print = true;

            workSheet.Cells[_maxRow, 7].Value = "     Guangzhou";

            var checkBox3 = workSheet.Drawings.AddCheckBoxControl("checkBox3");
            checkBox3.Text = "";
            checkBox3.SetPosition(190, 679);
            checkBox3.SetSize(50);
            checkBox3.Print = true;

            workSheet.Cells[_maxRow, 8].Value = "    Zhongshan";

            var checkBox4 = workSheet.Drawings.AddCheckBoxControl("checkBox4");
            checkBox4.Text = "";
            checkBox4.SetPosition(190, 798);
            checkBox4.SetSize(50);
            checkBox4.Print = true;

            workSheet.Cells[_maxRow, 9].Value = "    Xiamen";

            _maxRow++;

            var checkBox5 = workSheet.Drawings.AddCheckBoxControl("checkBox5");
            checkBox5.Text = "";
            checkBox5.SetPosition(212, 450);
            checkBox5.SetSize(50);
            checkBox5.Print = true;

            workSheet.Cells[_maxRow, 6].Value = "    Shanghai";

            var checkBox6 = workSheet.Drawings.AddCheckBoxControl("checkBox6");
            checkBox6.Text = "";
            checkBox6.SetPosition(212, 555);
            checkBox6.SetSize(50);
            checkBox6.Print = true;

            workSheet.Cells[_maxRow, 7].Value = "     Ningbo";

            var checkBox7 = workSheet.Drawings.AddCheckBoxControl("checkBox7");
            checkBox7.Text = "";
            checkBox7.SetPosition(212, 679);
            checkBox7.SetSize(50);
            checkBox7.Print = true;

            workSheet.Cells[_maxRow, 8].Value = "    Qingdao";

            var checkBox8 = workSheet.Drawings.AddCheckBoxControl("checkBox8");
            checkBox8.Text = "";
            checkBox8.SetPosition(212, 798);
            checkBox8.SetSize(50);
            checkBox8.Print = true;

            workSheet.Cells[_maxRow, 9].Value = "    Tianjin";

            _maxRow++;

            var checkBox9 = workSheet.Drawings.AddCheckBoxControl("checkBox9");
            checkBox9.Text = "";
            checkBox9.SetPosition(232, 450);
            checkBox9.SetSize(50);
            checkBox9.Print = true;

            workSheet.Cells[_maxRow, 6].Value = "    Dalian";

            var checkBox10 = workSheet.Drawings.AddCheckBoxControl("checkBox10");
            checkBox10.Text = "";
            checkBox10.SetPosition(232, 555);
            checkBox10.SetSize(50);
            checkBox10.Print = true;

            workSheet.Cells[_maxRow, 7].Value = "     Shenzhen";

            _maxRow += 4;
            using (var range = workSheet.Cells["B10:D" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            using (var range = workSheet.Cells["F13:I" + _maxRow])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "NOTIFY PARTY ";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            var notify = invoice?.NotifyParties?.ToList().Count == 0 ? invoice?.Consignee
                                            : invoice?.NotifyParties?.FirstOrDefault();

            workSheet.Cells[_maxRow + 1, 2].Value = notify?.Name?.ToUpper();

            customerAddress = notify?.Address.Split(",");
            j = 2;
            foreach (var item in customerAddress)
            {
                workSheet.Cells[_maxRow + j, 2].Value = item.Trim().ToUpper();

                j++;
            }

            var checkBox11 = workSheet.Drawings.AddCheckBoxControl("checkBox11");
            checkBox11.Text = "";
            checkBox11.SetPosition(338, 450);
            checkBox11.SetSize(50);
            checkBox11.Print = true;

            workSheet.Cells[_maxRow, 6].Value = "     LCL";

            var checkBox12 = workSheet.Drawings.AddCheckBoxControl("checkBox12");
            checkBox12.Text = "";
            checkBox12.SetPosition(338, 555);
            checkBox12.SetSize(50);
            checkBox12.Checked = OfficeOpenXml.Drawing.Controls.eCheckState.Checked;
            checkBox12.Print = true;

            workSheet.Cells[_maxRow, 7].Value = "     FCL";
            workSheet.Cells[_maxRow, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 7].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            workSheet.Cells[_maxRow, 9].Value = "NOS. ORIGINAL B/L";

            _maxRow++;
            workSheet.Cells[_maxRow, 6].Value = " IF FCL:  ___ X20' ; ___ X 40' ; 1_X 40'HQ ; __ X 45' ";
            workSheet.Cells[_maxRow, 6, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 6, _maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 9].Value = " ORIGINAL BL";

            _maxRow++;
            workSheet.Cells[_maxRow, 5].Value = "FREIGHT TERM:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 7].Value = "       FOB           CIF           EXWORK";
            workSheet.Cells[_maxRow, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 7].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            var checkBox13 = workSheet.Drawings.AddCheckBoxControl("checkBox13");
            checkBox13.Text = "";
            checkBox13.SetPosition(400, 568);
            checkBox13.SetSize(50);
            checkBox13.Checked = OfficeOpenXml.Drawing.Controls.eCheckState.Checked;
            checkBox13.Print = true;

            var checkBox14 = workSheet.Drawings.AddCheckBoxControl("checkBox14");
            checkBox14.Text = "";
            checkBox14.SetPosition(400, 642);
            checkBox14.SetSize(50);
            checkBox14.Print = true;

            var checkBox15 = workSheet.Drawings.AddCheckBoxControl("checkBox15");
            checkBox15.Text = "";
            checkBox15.SetPosition(400, 707);
            checkBox15.SetSize(50);
            checkBox15.Print = true;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 5].Value = " TRAFFIC MODE:";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 7].Value = "       CY-CY           CY-CFS           CFS-CY          CFS-CFS";

            var checkBox16 = workSheet.Drawings.AddCheckBoxControl("checkBox16");
            checkBox16.Text = "";
            checkBox16.SetPosition(441, 568);
            checkBox16.SetSize(50);
            checkBox16.Print = true;

            var checkBox17 = workSheet.Drawings.AddCheckBoxControl("checkBox17");
            checkBox17.Text = "";
            checkBox17.SetPosition(441, 660);
            checkBox17.SetSize(50);
            checkBox17.Print = true;

            var checkBox18 = workSheet.Drawings.AddCheckBoxControl("checkBox18");
            checkBox18.Text = "";
            checkBox18.SetPosition(441, 756);
            checkBox18.SetSize(50);
            checkBox18.Print = true;

            var checkBox19 = workSheet.Drawings.AddCheckBoxControl("checkBox19");
            checkBox19.Text = "";
            checkBox19.SetPosition(441, 849);
            checkBox19.SetSize(50);
            checkBox19.Print = true;

            _maxRow++;
            using (var range = workSheet.Cells["B17:D" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            using (var range = workSheet.Cells["E17:I" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "PRE-CARRIGE BY ";

            workSheet.Cells[_maxRow, 4].Value = "CLOSING DATE / CARGO READY DATE";

            workSheet.Cells[_maxRow, 5].Value = "DANGEROUS CARGO";

            _maxRow++;
            workSheet.Cells[_maxRow, 5].Value = "IMCO:";

            workSheet.Cells[_maxRow, 7].Value = "UNNO:";

            workSheet.Cells[_maxRow, 9].Value = "PAGE:";

            _maxRow++;

            using (var range = workSheet.Cells["B24:C" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            using (var range = workSheet.Cells["D24:D" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "VESSEL/VOY";

            workSheet.Cells[_maxRow, 4].Value = "PORT OF LOADING";

            workSheet.Cells[_maxRow, 5].Value = "MES:";

            workSheet.Cells[_maxRow, 7].Value = "MFAC:";

            workSheet.Cells[_maxRow, 9].Value = "HKCAT:";

            _maxRow++;
            using (var range = workSheet.Cells["E24:I" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            _maxRow++;
            using (var range = workSheet.Cells["B27:C" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            using (var range = workSheet.Cells["D27:D" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            workSheet.Cells[_maxRow, 5].Value = "FREIGHT & CHARGES";

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "PORT OF DISCHARGE";

            workSheet.Cells[_maxRow, 4].Value = "PLACE OF DELIVERY";

            _maxRow++;
            workSheet.Cells[_maxRow, 6].Value = "     PREPAID";

            workSheet.Cells[_maxRow, 7].Value = "     COLLECT";
            workSheet.Cells[_maxRow, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 7].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            var checkBox20 = workSheet.Drawings.AddCheckBoxControl("checkBox20");
            checkBox20.Text = "";
            checkBox20.SetPosition(630, 450);
            checkBox20.SetSize(50);
            checkBox20.Print = true;

            var checkBox21 = workSheet.Drawings.AddCheckBoxControl("checkBox21");
            checkBox21.Text = "";
            checkBox21.SetPosition(630, 555);
            checkBox21.SetSize(50);
            checkBox21.Checked = OfficeOpenXml.Drawing.Controls.eCheckState.Checked;
            checkBox21.Print = true;

            _maxRow++;
            using (var range = workSheet.Cells["B30:C" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            using (var range = workSheet.Cells["D30:D" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            using (var range = workSheet.Cells["E29:I" + _maxRow])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            workSheet.Cells["I29:I30"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            workSheet.Cells["I31:I32"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            _maxRow++;
        }

        private static void FillDataBookingRequest(ExcelWorksheet workSheet, Invoice invoice)
        {
            var packingUnit = invoice?.PackingList?.FirstOrDefault().PackingUnit;
            if (packingUnit != null)
            {
                _lengthUnit = packingUnit?.LengthUnit;
                _weightUnit = packingUnit?.WeightUnit;
            }
            else
            {
                _lengthUnit = "Cm";
                _weightUnit = "Kgs";
            }

            var style = "";
            var poNumber = "";
            var curContractNo = "";
            foreach (var packingList in invoice?.PackingList
                .OrderBy(x => x?.ItemStyles?.FirstOrDefault().ContractNo +
                         x?.ItemStyles?.FirstOrDefault().CustomerStyle).ToList())
            {
                foreach (var data in packingList?.ItemStyles)
                {
                    if (string.IsNullOrEmpty(style))
                        style += "STYLE: @" + data.CustomerStyle;
                    else if (curContractNo != data.ContractNo)
                        style += "@" + data.CustomerStyle;
                    else if (!string.IsNullOrEmpty(data.ContractNo) && 
                        data.CustomerStyle.Contains(data.ContractNo) && 
                        data.CustomerStyle != data.ContractNo)
                        style += "/" + data.CustomerStyle?.Trim().Replace(data.ContractNo, "");
                    else if (!string.IsNullOrEmpty(data.ContractNo) && 
                        !data.CustomerStyle.Contains(data.ContractNo) && 
                        data.CustomerStyle != data.ContractNo)
                        style += "/" + data.CustomerStyle?.Trim();

                    if (string.IsNullOrEmpty(poNumber))
                        poNumber += "PO#: " + data.PurchaseOrderNumber;
                    else if (!poNumber.Contains(data.PurchaseOrderNumber))
                        poNumber += "/" + data.PurchaseOrderNumber;

                    curContractNo = data.ContractNo;
                }
            }


            workSheet.Cells[_maxRow, 2].Value = "     MARKS & NOS.";

            workSheet.Cells[_maxRow, 4].Value = "NUMBER AND KIND OF PACKAGES & DESCRIPTION";

            workSheet.Cells[_maxRow, 7].Value = "GROSS WEIGHT";

            workSheet.Cells[_maxRow, 9].Value = "MEASURMENT";

            _maxRow++;
            workSheet.Cells[_maxRow, 4].Value = "OF GOODS";

            workSheet.Cells[_maxRow, 9].Value = "(CBM)";
            workSheet.Cells[_maxRow, 9].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells["B33:C34"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["D33:F34"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["G33:H34"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["I33:I34"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            _maxRow++;
            using (var range = workSheet.Cells["B35:I52"])
            {
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                range.Style.Font.SetFromFont(new Font("Arial", 10));
            }
            workSheet.Cells[_maxRow, 2].Value = "SHIPPING MARK";

            workSheet.Cells[_maxRow, 4].Value = "GARMENTS";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4, _maxRow + 1, 6].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = invoice?.Consignee?.Name?.ToUpper();

            //workSheet.Cells[_maxRow, 7].Formula = _cellPKLTotalGross;
            workSheet.Cells[_maxRow, 7].Style.Font.SetFromFont(new Font("Comic Sans MS", 12));
            workSheet.Cells[_maxRow, 7].Style.Numberformat.Format = "#,##0.00";
            workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 8].Value = _weightUnit?.ToUpper();

            //workSheet.Cells[_maxRow, 7].Formula = _cellPKLTotalMeasM3;
            workSheet.Cells[_maxRow, 9].Style.Font.SetFromFont(new Font("Comic Sans MS", 12));
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "#,##0.00";
            workSheet.Cells[_maxRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            _maxRow += 3;
            workSheet.Cells[_maxRow, 2].Value = "DESTINATION:";
            workSheet.Cells[_maxRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
            workSheet.Row(_maxRow).Height = 55;

            workSheet.Cells[_maxRow, 4].Value = style.Replace('@', '\n');
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Style.Font.Color.SetColor(Color.Red);
            workSheet.Cells[_maxRow, 4].Style.WrapText = true;
            workSheet.Cells[_maxRow, 4, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 4, _maxRow, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            _maxRow += 1;
            workSheet.Cells[_maxRow, 2].Value = "MASTER STYLE NO:";
            workSheet.Cells[_maxRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
            workSheet.Row(_maxRow).Height = 30;

            workSheet.Cells[_maxRow, 4].Value = poNumber;
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Style.Font.Color.SetColor(Color.Red);
            workSheet.Cells[_maxRow, 4].Style.WrapText = true;
            workSheet.Cells[_maxRow, 4, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 4, _maxRow, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "LABEL CODE:";

            workSheet.Cells[_maxRow, 4].Value = "CARTON:";

            //workSheet.Cells[_maxRow, 6].Formula = _cellPKLTotalCarton;
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "STYLE NO:";

            workSheet.Cells[_maxRow, 4].Value = "QUANTITY:";

            workSheet.Cells[_maxRow, 6].Formula = _cellPKLTotalPCS;
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "IFG PO NO:";

            _maxRow += 9;
            workSheet.Cells[_maxRow, 2].Value = "TOTAL CONTAINER";
            //workSheet.Cells[_maxRow, 6].Formula = _cellPKLTotalMeasM3;
            workSheet.Cells[_maxRow, 6].Style.Font.SetFromFont(new Font("Comic Sans MS", 14));
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 6].Style.Font.Color.SetColor(Color.Red);
            workSheet.Cells[_maxRow, 6].Style.Numberformat.Format = "#,##0.00";
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 7].Value = "CBM";
            workSheet.Cells[_maxRow, 7].Style.Font.SetFromFont(new Font("Times New Roman", 12));
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7].Style.Font.Color.SetColor(Color.Red);

            workSheet.Cells["B35:C52"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["D35:F52"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["G35:H52"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["I35:I52"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "SOLAS Regulations - Verified Gross Mass (VGM)";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "Mandatory Weight Verification for all Export Containers valid globally with effective July 1, 2016";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = "** No VGM, No LOAD.";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 2].Value = "TRUCKING SERVICE";
            workSheet.Cells[_maxRow, 2, _maxRow, 5].Merge = true;
            workSheet.Cells[_maxRow, 2, _maxRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            workSheet.Cells[_maxRow, 6].Value = "CUSTOMS CLEARANCE SERVICE";
            workSheet.Cells[_maxRow, 6, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 6, _maxRow, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            _maxRow++;
            workSheet.Cells[_maxRow, 2, _maxRow, 5].Merge = true;
            workSheet.Cells[_maxRow, 2, _maxRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            workSheet.Cells[_maxRow, 6, _maxRow, 9].Merge = true;
            workSheet.Cells[_maxRow, 6, _maxRow, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            var checkBox22 = workSheet.Drawings.AddCheckBoxControl("checkBox22");
            checkBox22.Text = "";
            checkBox22.SetPosition(1267, 30);
            checkBox22.SetSize(50);
            checkBox22.Print = true;

            var checkBox23 = workSheet.Drawings.AddCheckBoxControl("checkBox23");
            checkBox23.Text = "";
            checkBox23.SetPosition(1267, 455);
            checkBox23.SetSize(50);
            checkBox23.Print = true;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 4].Value = "\" Hereby certify that this shipment does not contain any solid wood packing materials\"";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            workSheet.Cells["B59:I61"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            _maxRow += 3;
            workSheet.Cells[_maxRow, 2].Value = "       ABOVE DETAILS DECLARED BY SHIPPER";
            workSheet.Cells[_maxRow, 2, _maxRow, 9].Merge = true;

            workSheet.Cells["B62:I65"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            _maxRow += 2;

            var shapeText = "ALWAYS FILL IN IMPORTANT DETAILS HIGHLIGHTED IN RED FONT / YELLOW COLOR @PLEASE MENTION IF THE STYLE IS UNDER WALMART OR NONWALMART";
            var shape1 = workSheet.Drawings.AddShape("shape1", OfficeOpenXml.Drawing.eShapeStyle.Rect);
            shape1.SetSize(320, 150);
            shape1.SetPosition(820, 590);
            shape1.TextAlignment = OfficeOpenXml.Drawing.eTextAlignment.Center;
            shape1.Fill.Style = OfficeOpenXml.Drawing.eFillStyle.SolidFill;
            shape1.Fill.SolidFill.Color.SetRgbColor(Color.White);
            shape1.Text = shapeText.Replace('@', '\n');
            shape1.Font.Bold = true;
            shape1.Font.SetFromFont(new Font("Calibri", 14));
            shape1.Font.Color = Color.Red;

            var shape2 = workSheet.Drawings.AddShape("shape2", OfficeOpenXml.Drawing.eShapeStyle.RightArrow);
            shape2.SetSize(80, 20);
            shape2.SetPosition(1305, 150);
            shape2.Fill.Style = OfficeOpenXml.Drawing.eFillStyle.SolidFill;
            shape2.Fill.SolidFill.Color.SetRgbColor(Color.Red);
        }

        #endregion BOOKING REQUEST

        #region BOOKING REQUEST DETAILS
        private static void CreateBookingRequestDetails(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Column(1).Width = 23.6;
            workSheet.Column(2).Width = 18;
            workSheet.Column(3).Width = 25.9;
            workSheet.Column(4).Width = 33.4;
            workSheet.Column(5).Width = 13.9;
            workSheet.Column(6).Width = 23;
            workSheet.Column(7).Width = 12.8;
            workSheet.Column(8).Width = 8.8;
            workSheet.Column(9).Width = 12.3;
            workSheet.Column(10).Width = 11.6;
            workSheet.Column(11).Width = 16;

            workSheet.Cells[_maxRow, 2].Value = "Booking Form S/NO:";
            workSheet.Cells[_maxRow, 2].Style.Font.Size = 22;
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 2, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 2, _maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 43.8;

            workSheet.Cells[_maxRow, 7].Value = "VENDOR NAME:";
            workSheet.Cells[_maxRow, 7].Style.Font.Size = 12;
            workSheet.Cells[_maxRow, 7].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 7, _maxRow, 8].Style.Border.BorderAround(ExcelBorderStyle.Double);

            workSheet.Cells[_maxRow, 9, _maxRow, 11].Merge = true;
            workSheet.Cells[_maxRow, 9, _maxRow, 11].Style.Border.BorderAround(ExcelBorderStyle.Double);
            workSheet.Cells[_maxRow, 9, _maxRow, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, 9, _maxRow, 11].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            _maxRow++;
            workSheet.Row(_maxRow).Height = 9.4;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Vessel / Airline Name:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Row(_maxRow).Height = 39.6;

            workSheet.Cells[_maxRow, 3].Value = "Departure Port:";
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 6].Value = "Arrival Port:";
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 8].Value = "ALLWAYS JOB #";
            workSheet.Cells[_maxRow, 8].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 8, _maxRow + 1, 9].Merge = true;
            workSheet.Cells[_maxRow, 8, _maxRow + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow, 10, _maxRow + 1, 11].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Voyage / Flight:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Row(_maxRow).Height = 29.4;

            workSheet.Cells[_maxRow, 3].Value = "ETD Date:";
            workSheet.Cells[_maxRow, 3].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 6].Value = "ETA Date:";
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;

            using (var range = workSheet.Cells["A3:K" + _maxRow])
            {
                range.Style.Border.Left.Style = ExcelBorderStyle.Double;
                range.Style.Border.Right.Style = ExcelBorderStyle.Double;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                range.Style.Border.Top.Style = ExcelBorderStyle.Double;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            _maxRow++;
            workSheet.Row(_maxRow).Height = 9.4;

            _maxRow++;
            var rowBorder = _maxRow;
            int column = 1;

            workSheet.Cells[_maxRow, column].Value = "STYLE NO";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "PO NUMBER";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "DESCRIPTION";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "COLOR NAME";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "COLOR CODE";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "LABEL CODE";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "QUANTITY";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow, column + 2].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow, column + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column += 3;
            workSheet.Cells[_maxRow, column].Value = "CARGO MEASURMENTS";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow, column + 1].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow, column + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            column = 7;
            workSheet.Cells[_maxRow, column].Value = "TOTAL QTY";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "PCS PER CARTON";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "TOTAL CTNS";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "CBM";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = _weightUnit.ToUpper();
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            column = 1;
            decimal totQty = 0;
            decimal totCtns = 0;
            decimal totCBM = 0;
            decimal totKgs = 0;
            var inchConvertValue = (decimal)(1728 * 35.31);
            var lbConvertValue = (decimal)2.2046;

            foreach (var packingList in invoice?.PackingList)
            {
                var firstStyle = true;
               
                if(packingList?.ItemStyles?.Count() > 1)
                {
                    foreach(var itemStyle in packingList?.ItemStyles)
                    {
                        column = 1;
                        if (firstStyle)
                        {
                            decimal measM3 = 0;
                            decimal grossWeight = 0;
                            var line = packingList?.PackingLines?.FirstOrDefault(x => x.LSStyle == itemStyle.LSStyle);
                            var weightLines = packingList?.PackingLines?.Where(x => x.Size == line.Size).ToList();
                            
                            /// Convert packing unit
                            if (_lengthUnit.Trim().ToUpper() == "INCH")
                            {
                                measM3 = (decimal)(line.Length * line.Width * line.Height /
                                    (decimal)inchConvertValue * (decimal)line.TotalCarton);
                            }
                            else
                            {
                                measM3 = (decimal)(line.Length / 100 * line.Width / 100 *
                                              line.Height / 100 * (decimal)line.TotalCarton);
                            }

                            if (_weightUnit.Trim().ToUpper() == "LBS")
                            {
                                grossWeight = (decimal)((line.GrossWeight - line.NetWeight 
                                        + weightLines.Sum(x => x.NetWeight)) * (decimal)lbConvertValue);
                            }
                            else
                            {
                                grossWeight = (decimal)(line.GrossWeight - line.NetWeight
                                        + weightLines.Sum(x => x.NetWeight));
                            }

                            workSheet.Cells[_maxRow, column].Value = itemStyle.CustomerStyle;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.PurchaseOrderNumber;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.Description;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.ColorName;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.ColorCode;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.LabelCode;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingList?.
                                        PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.QuantitySize * x.TotalCarton);
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingList?.
                                        PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.Quantity);
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = line.TotalCarton;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = measM3;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";

                            column++;
                            workSheet.Cells[_maxRow, column].Value = grossWeight;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";

                            totCtns += (decimal)line.TotalCarton;
                            totQty += (decimal)packingList.TotalQuantity;
                            totKgs += grossWeight;
                            totCBM += measM3;

                            firstStyle = false;
                            _maxRow++;
                        }
                        else
                        {
                            var line = packingList?.PackingLines?.FirstOrDefault(x => x.LSStyle == itemStyle.LSStyle);

                            workSheet.Cells[_maxRow, column].Value = itemStyle.CustomerStyle;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.PurchaseOrderNumber;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.Description;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.ColorName;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.ColorCode;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.LabelCode;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingList?.
                                        PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.QuantitySize * x.TotalCarton);
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingList?.
                                        PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.Quantity);
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            _maxRow++;
                        }
                    }
                    
                }
                else
                {
                    var itemStyle = packingList?.ItemStyles?.FirstOrDefault();
                    if(packingList?.PackingLines?.FirstOrDefault().PrePack == "Assorted Size - Solid Color")
                    {
                        column = 1;
                        decimal measM3 = 0;
                        decimal grossWeight = 0;
                        var line = packingList?.PackingLines?.FirstOrDefault();
                        /// Convert packing unit
                        if (_lengthUnit.Trim().ToUpper() == "INCH")
                        {
                            measM3 = (decimal)(line.Length * line.Width * line.Height /
                                (decimal)inchConvertValue * (decimal)line.TotalCarton);
                        }
                        else
                        {
                            measM3 = (decimal)(line.Length / 100 * line.Width / 100 *
                                          line.Height / 100 * (decimal)line.TotalCarton);
                        }

                        if (_weightUnit.Trim().ToUpper() == "LBS")
                        {
                            grossWeight = (decimal)line.GrossWeight * (decimal)lbConvertValue;
                        }
                        else
                        {
                            grossWeight = (decimal)line.GrossWeight;
                        }

                        workSheet.Cells[_maxRow, column].Value = itemStyle.CustomerStyle;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle.PurchaseOrderNumber;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle.Description;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle.ColorName;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle.ColorCode;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = itemStyle.LabelCode;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = packingList.TotalQuantity;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = packingList?.PackingLines?.Sum(x => x.Quantity);
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = line.TotalCarton;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = measM3;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";

                        column++;
                        workSheet.Cells[_maxRow, column].Value = grossWeight;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";

                        totCtns += (decimal)line.TotalCarton;
                        totQty += (decimal)packingList.TotalQuantity;
                        totKgs += grossWeight;
                        totCBM += measM3;

                        _maxRow++;
                    }
                    else
                    {
                        packingList?.PackingLines?.ToList().ForEach(x =>
                        {
                            column = 1;
                            decimal measM3 = 0;
                            decimal grossWeight = 0;
                            /// Convert packing unit
                            if (_lengthUnit.Trim().ToUpper() == "INCH")
                            {
                                measM3 = (decimal)(x.Length * x.Width * x.Height /
                                    (decimal)inchConvertValue * (decimal)x.TotalCarton);
                            }
                            else
                            {
                                measM3 = (decimal)(x.Length / 100 * x.Width / 100 *
                                              x.Height / 100 * (decimal)x.TotalCarton);
                            }

                            if (_weightUnit.Trim().ToUpper() == "LBS")
                            {
                                grossWeight = (decimal)x.GrossWeight * (decimal)lbConvertValue;
                            }
                            else
                            {
                                grossWeight = (decimal)x.GrossWeight;
                            }

                            workSheet.Cells[_maxRow, column].Value = itemStyle.CustomerStyle;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.PurchaseOrderNumber;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.Description;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.ColorName;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.ColorCode;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = itemStyle.LabelCode;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = x.TotalQuantity;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = x.QuantityPerCarton;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = x.TotalCarton;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = measM3;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";

                            column++;
                            workSheet.Cells[_maxRow, column].Value = grossWeight;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";

                            totCtns += (decimal)x.TotalCarton;
                            totQty += (decimal)x.TotalQuantity;
                            totKgs += grossWeight;
                            totCBM += measM3;

                            _maxRow++;

                        });
                    }
                }
            }

            column = 7;
            workSheet.Cells[_maxRow, column].Value = totQty;
            workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0";
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            column += 2;
            workSheet.Cells[_maxRow, column].Value = totCtns;
            workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0";
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            column++;
            workSheet.Cells[_maxRow, column].Value = totCBM;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";

            column++;
            workSheet.Cells[_maxRow, column].Value = totKgs;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, column].Style.Numberformat.Format = "#,##0.00";

            using (var range = workSheet.Cells["A" + _maxRow + ":K" + _maxRow])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
            }

            using (var range = workSheet.Cells["A" + rowBorder + ":K" + _maxRow])
            {
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

        }
        #endregion BOOKING REQUEST DETAILS

        #region BOOKING SUMMARY
        private static void CreateBookingSummary(ExcelWorksheet workSheet, Invoice invoice, List<Size> sizes)
        {
            workSheet.Column(1).Width = 11.3;
            workSheet.Column(2).Width = 38.4;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();
            
            int column = 1;
            int columnEndSize = 3 + _sizeList.Count() - 1;

            workSheet.Cells[_maxRow, column].Value = "PO";
            workSheet.Cells[_maxRow, column].Style.Font.Size = 12;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            _dicPositionColumnPackingLine.Add(PO_NO, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = "COLOR";
            workSheet.Cells[_maxRow, column].Style.Font.Size = 12;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            _dicPositionColumnPackingLine.Add(COLOR, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = "SIZE";
            workSheet.Cells[_maxRow, column].Style.Font.Size = 12;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow, columnEndSize].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow, columnEndSize].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var invoiceSizes = sizes.Where(x => _sizeList.Contains(x.Code))
                                .OrderBy(x => x.SequeneceNumber).ToList();
            foreach (var size in invoiceSizes)
            {
                if (!string.IsNullOrEmpty(size.Code))
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size.Code;
                    workSheet.Cells[_maxRow + 1, column].Style.WrapText = true;
                    workSheet.Cells[_maxRow + 1, column].Style.Font.Size = 12;
                    workSheet.Cells[_maxRow + 1, column].Style.Font.Bold = true;
                    workSheet.Cells[_maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    _dicPositionColumnPackingLine.Add(size.Code, column);
                    workSheet.Column(column).Width = 8.5;

                    column++;
                }
            }

            workSheet.Cells[_maxRow, column].Value = "TOTAL QUANTITY";
            workSheet.Cells[_maxRow, column].Style.Font.Size = 12;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Merge = true;
            workSheet.Cells[_maxRow, column, _maxRow + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            _dicPositionColumnPackingLine.Add(TOTAL_UNITS_PCS, column);
            workSheet.Column(column).Width = 13.4;

            using (var range = workSheet.Cells["A1:" + _dicAlphabel[columnEndSize+1] + "2"])
            {
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 217, 196));
            }

            _maxRow += 1;
            var rowBorder = _maxRow + 1;
            decimal totQty = 0;

            foreach (var packingList in invoice?.PackingList)
            {
                if (packingList?.PackingLines?.FirstOrDefault()?.PrePack == "Assorted Size - Solid Color")
                {
                    foreach (var itemStyle in packingList?.ItemStyles)
                    {
                        _maxRow++;

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PO_NO]].Value = itemStyle.PurchaseOrderNumber;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PO_NO]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        foreach (var line in packingList?.PackingLines
                            .Where(x => x.LSStyle == itemStyle.LSStyle).ToList())
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[line.Size]].Value = line.QuantitySize * line.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[line.Size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[line.Size]].Style.Numberformat.Format = "#,##0";
                        }

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Value = packingList?.
                                PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.QuantitySize * x.TotalCarton);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Numberformat.Format = "#,##0";
                    }
                    totQty += (decimal)packingList?.TotalQuantity;
                }
                else
                {
                    foreach (var itemStyle in packingList?.ItemStyles)
                    {
                        foreach (var line in packingList?.PackingLines
                            .Where(x => x.LSStyle == itemStyle.LSStyle)
                            .OrderBy(x => x.SequenceNo).ToList())
                        {
                            _maxRow++;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PO_NO]].Value = itemStyle.PurchaseOrderNumber;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PO_NO]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[line.Size]].Value = line.QuantitySize * line.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[line.Size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[line.Size]].Style.Numberformat.Format = "#,##0";

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Value = packingList?.
                                    PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle && x.Size == line.Size).Sum(x => x.QuantitySize * x.TotalCarton);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Numberformat.Format = "#,##0";
                        }

                    }
                    totQty += (decimal)packingList?.TotalQuantity;
                }
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "GRAND TOTAL";
            workSheet.Cells[_maxRow, 1].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 1, _maxRow, columnEndSize].Merge = true;
            workSheet.Cells[_maxRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Value = totQty;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Font.Color.SetColor(Color.Red);
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS_PCS]].Style.Numberformat.Format = "#,##0";

            using (var range = workSheet.Cells["A" + _maxRow + ":" +_dicAlphabel[_dicPositionColumnPackingLine[TOTAL_UNITS_PCS]] + _maxRow])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            using (var range = workSheet.Cells["A" + rowBorder + ":" + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_UNITS_PCS]] + _maxRow])
            {
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

        }
        #endregion BOOKING SUMMARY

        #region CONTAINER QTY FORMAT WM
        private static void CreateContainerQuantityFormatWN(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Column(1).Width = 26.1;
            workSheet.Column(2).Width = 28.5;
            workSheet.Column(3).Width = 14.1;
            workSheet.Column(4).Width = 11.4;
            workSheet.Column(5).Width = 22.1;
            workSheet.Column(6).Width = 11.4;
            workSheet.Column(7).Width = 50.7;
            workSheet.Column(8).Width = 26.8;

            using (var range = workSheet.Cells["A1:D1"])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            var packingUnit = invoice?.PackingList?.FirstOrDefault().PackingUnit;
            if (packingUnit != null)
            {
                _lengthUnit = packingUnit?.LengthUnit?.ToUpper() == "INCH" ? "\"" : packingUnit?.LengthUnit?.ToLower();
            }
            else
            {
                _lengthUnit = "cm";
            }

            GetSizeList(invoice, null);

            var seasonList = "";
            var styleList = "";
            var descList = "";
            var factoryList = "";

            foreach (var style in _styleList)
            {
                if (string.IsNullOrEmpty(seasonList))
                    seasonList = style?.Season.Trim();
                else if(!seasonList.Contains(style?.Season?.Trim()))
                    seasonList += "/" + style?.Season.Trim();

                if (string.IsNullOrEmpty(styleList))
                    styleList = style?.CustomerStyle?.Trim();
                else if (!styleList.Contains(style?.CustomerStyle?.Trim()))
                    styleList += "/" + style?.CustomerStyle?.Trim();

                if (string.IsNullOrEmpty(descList))
                    descList = style?.Description?.Trim();
                else if (!descList.Contains(style?.Description.Trim()))
                    descList += "/" + style?.Description.Trim();
            }


            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "CARTON DIMS : NO.OF BOX IN CONTAINER";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SEASON";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 2].Value = seasonList;
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "COUNTRY";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 2].Value = "VIETNAM";
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "FACTORY NAME";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            var rowFactoty = _maxRow;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "STYLE NUMBER:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 2].Value = styleList;
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 2].Style.Font.Color.SetColor(Color.Red);

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "DESCRIPTION";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 2].Value = descList;
            workSheet.Cells[_maxRow, 2].Style.Font.Color.SetColor(Color.Red);

            int column = 1;
            _maxRow++;

            workSheet.Cells[_maxRow, column].Value = "ASSORTMENT(PCS)";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;

            column++;
            workSheet.Cells[_maxRow, column].Value = "L(" + _lengthUnit + ")";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "W(" + _lengthUnit + ")";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "H(" + _lengthUnit + ")";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "NO OF CARTONS";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "total pcs";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "container type#";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;
            workSheet.Cells[_maxRow, column].Value = "REMARKS";
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_maxRow, column].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            _maxRow++;
            column = 1;
            decimal totQty = 0;
            decimal totCtns = 0;

            bool firstPackingList = true;
            var ppcBook = "";
            var rowPPCBook = 0;

            foreach (var packingList in invoice?.PackingList)
            {
                if (string.IsNullOrEmpty(factoryList))
                    factoryList = packingList?.Factory?.ToUpper();
                else if (!string.IsNullOrEmpty(packingList?.Factory) &&
                        !factoryList.Contains(packingList?.Factory?.ToUpper() ?? string.Empty))
                    factoryList += "/" + packingList?.Factory?.ToUpper();

                if(string.IsNullOrEmpty(ppcBook) &&
                    DateTime.TryParse(packingList?.PPCBookDate?.ToString(), out DateTime bookDate))
                {
                    ppcBook = "CRD: " + bookDate.ToString("dd MMM yyyy");
                }

                var firstStyle = true;
                if (packingList?.PackingLines?.FirstOrDefault().PrePack == "Assorted Size - Solid Color")
                {
                    foreach (var itemStyle in packingList?.ItemStyles)
                    {
                        column = 1;
                        var packingLine = packingList?.PackingLines?.FirstOrDefault(x => x.LSStyle == itemStyle.LSStyle);

                        if (firstStyle)
                        {
                            workSheet.Cells[_maxRow, column].Value = packingList?.
                                        PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.Quantity);
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingLine?.Length;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingLine?.Width;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingLine?.Height;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingLine?.TotalCarton;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingList?.
                                        PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.QuantitySize * x.TotalCarton);
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column += 2;
                            if(firstPackingList)
                            {
                                rowPPCBook = _maxRow;
                                firstPackingList = false;
                            }
                                

                            totCtns += (decimal)packingLine?.TotalCarton;
                            totQty += (decimal)packingList?.TotalQuantity;

                            firstStyle = false;
                            _maxRow++;
                        }
                        else
                        {
                            workSheet.Cells[_maxRow, column].Value = packingList?.
                                        PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.Quantity);
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = 0;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = 0;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = 0;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = 0;
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            column++;
                            workSheet.Cells[_maxRow, column].Value = packingList?.
                                        PackingLines?.Where(x => x.LSStyle == itemStyle.LSStyle).Sum(x => x.QuantitySize * x.TotalCarton);
                            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            _maxRow++;
                        }
                    }
                }
                else
                {
                    packingList?.PackingLines?.ToList().ForEach(x =>
                    {
                        column = 1;

                        workSheet.Cells[_maxRow, column].Value = x?.Quantity;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = x?.Length;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = x?.Width;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = x?.Height;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = x?.TotalCarton;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column++;
                        workSheet.Cells[_maxRow, column].Value = x?.TotalQuantity;
                        workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        column += 2;
                        if (firstPackingList)
                        {
                            rowPPCBook = _maxRow;
                            firstPackingList = false;
                        }


                        totCtns += (decimal)x?.TotalCarton;
                        totQty += (decimal)x?.TotalQuantity;

                        _maxRow++;

                    });
                }
            }

            workSheet.Cells[rowFactoty, 2].Value = factoryList;
            workSheet.Cells[rowFactoty, 2].Style.Font.Bold = true;

            workSheet.Cells[rowPPCBook, 8].Value = ppcBook;
            workSheet.Cells[rowPPCBook, 8].Style.Font.Bold = true;
            workSheet.Cells[rowPPCBook, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow, 5].Value = totCtns;
            workSheet.Cells[_maxRow, 5].Style.Numberformat.Format = "#,##0";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Style.Font.Color.SetColor(Color.Red);
            workSheet.Cells[_maxRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column += 2;
            workSheet.Cells[_maxRow, 6].Value = totQty;
            workSheet.Cells[_maxRow, 6].Style.Numberformat.Format = "#,##0";
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 6].Style.Font.Color.SetColor(Color.Red);
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            using (var range = workSheet.Cells["A" + _maxRow + ":H" + _maxRow])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }

            using (var range = workSheet.Cells["A1:H" + _maxRow])
            {
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;                
            }
        }
        #endregion CONTAINER QTY FORMAT WM


        #region General Function
        public static void GetSizeList(Invoice invoice, PackingList packing)
        {
            if(packing == null)
            {
                _sizeList = new List<string>();
                _styleList = new List<ItemStyle>();
                //var packingList = invoice?.PackingList?.FirstOrDefault();
                //foreach (var data in invoice?.PackingList?.Where(x => x.ID != packingList.ID))
                //{
                //    if (data?.PackingLines?.Select(x => x.Size).Distinct().Count() >
                //        packingList?.PackingLines?.Select(x => x.Size).Distinct().Count())
                //    {
                //        packingList = data;
                //    }
                //}

                //var itemStyle = packingList?.ItemStyles?.FirstOrDefault();

                //packingList.PackingLines.Where(x => x.LSStyle == itemStyle.LSStyle)
                //    .OrderBy(x => x.SequenceNo).ToList().ForEach(x =>
                //    {
                //        _sizeList.Add(x.Size);
                //    });

                foreach (var data in invoice?.PackingList)
                {
                    _styleList.AddRange(data?.ItemStyles);

                    data?.PackingLines.Where(x => x.LSStyle == data?.ItemStyles?.FirstOrDefault()?.LSStyle)
                        .OrderBy(x => x.SequenceNo).ToList().ForEach(s =>
                        {
                            if (!_sizeList.Contains(s.Size))
                            {
                                _sizeList.Add(s.Size);
                            }
                        });
                }
            }
            else
            {
                _sizePackingList = string.Empty;
                var packingLines = packing?.PackingLines
                    .Where(x => x.LSStyle == packing?.ItemStyles?.FirstOrDefault()?.LSStyle).ToList();

                _sizePackingList = packingLines.OrderBy(x => x.SequenceNo)?.FirstOrDefault()?.Size?.ToUpper() + "-" 
                                    + packingLines.OrderByDescending(x => x.SequenceNo)?.FirstOrDefault()?.Size?.ToUpper();
            }
        }

        private static void SetDictionaryAlphabet(int range = 1)
        {
            _dicAlphabel = new Dictionary<int, string>();
            int colNumber = 1; // start = 1
            int rangeCol = range; // A,B,C, ..., AA, AB,... BA, BB,... CA, CB... default 26 character
            string[] strAlpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                  "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            for (int i = 0; i <= rangeCol; i++)
            {
                if (i == 0) // range 1 character excel
                {
                    for (int j = 0; j < strAlpha.Count(); j++)
                    {
                        _dicAlphabel.Add(colNumber++, strAlpha[j]);
                    }
                }
                else if (i <= 26) // range 2 character excel
                {
                    for (int j = 0; j < strAlpha.Count(); j++)
                    {
                        _dicAlphabel.Add(colNumber++, strAlpha[i - 1] + strAlpha[j]);
                    }
                }
            }
        }

        #endregion
    }
}