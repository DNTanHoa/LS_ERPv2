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
    public class ExportMultiInvoiceGA_Processor
    {
        private static int _maxRow = 1;
        private static int _rowMBL = 0;
        private static Dictionary<string, int> _dicPositionColumnPackingLine;
        private const string MARK_AND_NO = "Marks and Nos. ";
        private const string DESCRIPTION_OF_GOODS = "Description  of Goods";
        private const string QUANTITY = "Quantity";
        private const string UNIT_PRICE_EACH = "Unit Price/Each";
        private const string TOTAL_AMOUNT = "TOTAL AMOUNT";
        private const string SAY_TOTAL = "SAY TOTAL";
        private const double PERCENT = 0.5;
        private const string PERCENT_DEFECTIVE = " PERCENT DEFECTIVE ALLOWANCE DEDUCTED FROM THE TOTAL GROSS AMOUNT";
        private const string LESS_DEFECTIVE = "LESS DEFECTIVE ALLOWNCE @";
        private const string FROM_NO = "From No";
        private const string TO_NO = "To No";
        private const string CNTS = "CNTS";
        private const string DESCRIPTION = "DESCRIPTION";
        private const string PO_NO = "PO. NO.:";
        private const string SIZE = "SIZE";
        private const string COLOR = "COLOR";
        private const string COLOR_NO = "COLOR#:";
        private const string UNITS_CARTON = "Q'TY @PCS @/CTN";
        private const string TOTAL_UNITS = "TTL @PCS";
        private const string GW_CARTON = "G.W. @/CTN @KGS";
        private const string TOTAL_GW = "TTL @G.W. @KGS";
        private const string NW_CARTON = "N.W. @/CTN @KGS";
        private const string TOTAL_NW = "TTL @N.W. @KGS";
        private const string LENGTH = "L";
        private const string WIDTH = "W";
        private const string HEIGHT = "H";
        private const string CARTON_DIMENSION = "MEASUREMENT @(INCH)";
        private const string STYLE = "Garan @Style # @(e.g @Q61150)";
        private const string PACK = "Garan @pack @(e.g. @002, @012, @036, @etc.";
        private const string UNIT = "Unit of @Meas. @(e.g. @pieces, @dozens, @cases)";
        private const string ASSORTED = "ASSORTED";
        private static int _colSize = 0;
        private static int _countSize = 0;
        private static int _colEndSize = 0;
        private static Dictionary<int, string> _dicAlphabel;
        private static Dictionary<string, decimal> _dicTotalQuantitySize;
        private static decimal _totalCarton = 0;
        private static decimal _totalUnits = 0;
        private static decimal _totalGW = 0;
        private static decimal _totalNW = 0;
        private static decimal _totalMeanM3 = 0;
        private static List<string> _fillColor;
        private static string _packingType;

        public static Stream CreateExcelFile(List<Invoice> invoices, Stream stream = null)
        {
            //_maxRow = 1;
            //_rowMBL = 0;
            //DataTable table = SetData(invoice);

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
                    _rowMBL = 0;
                    excelPackage.Workbook.Properties.Author = Author;
                    CreateInvoice(excelPackage, invoices[i], Title, sheetIndex);

                    _maxRow = 1;
                    sheetIndex++;
                    CreatePackingList(excelPackage, invoices[i], Title, sheetIndex);

                    _maxRow = 1;
                    sheetIndex++;
                    CreateSTDPackingList(excelPackage, invoices[i], Title, sheetIndex);
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
            excelPackage.Workbook.Worksheets.Add("INV - " + invoice.Code);
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.DefaultColWidth = 12;

            CreateHeaderInvoice(workSheet, invoice);
            CreateNotifyPartyInvoice(workSheet, invoice);
            CreateHeaderInvoiceDetail(workSheet, invoice);
            FillDataInvoice(workSheet, invoice);

            string modelRangeBorder = "A1:"
                                    + "L"
                                    + (_maxRow);

            using (var range = workSheet.Cells[modelRangeBorder])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White);
                range.Style.Font.Bold = true;
            }

            // set color invoice code
            workSheet.Cells[6, 2, 6, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[6, 2, 6, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

            /// set color date
            workSheet.Cells[6, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[6, 12].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

            /// set color VSL
            workSheet.Cells[13, 5, 13, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[13, 5, 13, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
            workSheet.Cells[14, 5, 14, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[14, 5, 14, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

            /// set color ETD
            workSheet.Cells[13, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[13, 9].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

            /// set color ETA
            workSheet.Cells[14, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[14, 9].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

            /// set color container
            workSheet.Cells[19, 7, 19, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[19, 7, 19, 12].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

            /// set color MLB
            workSheet.Cells[_rowMBL, 7, _rowMBL, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[_rowMBL, 7, _rowMBL, 11].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

            // Set page break view
            workSheet.View.PageBreakView = true;
            workSheet.PrinterSettings.FitToPage = true;
            workSheet.Row(_maxRow).PageBreak = true;
            workSheet.Column(12).PageBreak = true;

            // Set the tab color
            workSheet.TabColor = Color.FromArgb(0, 176, 80);
        }

        private static void CreatePackingList(ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Start";
            excelPackage.Workbook.Worksheets.Add("PL - " + invoice.Code);
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            //workSheet.DefaultRowHeight = 15.6;
            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells.Style.Font.Bold = true;

            ResetData();
            SetDictionaryAlphabet();

            _countSize = invoice.PackingList.FirstOrDefault()
                    .ItemStyles.FirstOrDefault().OrderDetails.Count;
            _colSize = 7 + _countSize - 1;

            CreateHeaderPL(workSheet, invoice);
            FillDataPL(workSheet, invoice);
            SummaryDataPL(workSheet, invoice);

            string modelRangeBorder = "A1:"
                                    + _dicAlphabel[_dicPositionColumnPackingLine[HEIGHT]]
                                    + (_maxRow);

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
            workSheet.TabColor = Color.FromArgb(0, 0, 255);
        }

        private static void CreateSTDPackingList(ExcelPackage excelPackage, Invoice invoice, string Title, int sheetIndex)
        {
            excelPackage.Workbook.Properties.Title = Title + sheetIndex;
            excelPackage.Workbook.Properties.Comments = "Invoice of Leading Start";
            excelPackage.Workbook.Worksheets.Add("STD PL - " + invoice.Code);
            var workSheet = excelPackage.Workbook.Worksheets[sheetIndex];

            workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            //workSheet.DefaultColWidth = 12;
            workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ResetData();
            SetDictionaryAlphabet();

            _countSize = invoice.PackingList.FirstOrDefault()
                    .ItemStyles.FirstOrDefault().OrderDetails.Count;
            _colSize = 7;
            _colEndSize = _colSize + (_countSize <= 5 ? 5 : _countSize - 1);
            _fillColor = new List<string>();

            CreateHeaderSTDPL(workSheet, invoice);
            CreateHeaderDataSTDPL(workSheet, invoice);
            FillDataSTDPL(workSheet, invoice);

            string modelRangeBorder = "A1:"
                                    + _dicAlphabel[_dicPositionColumnPackingLine[TO_NO]]
                                    + (_maxRow);

            using (var range = workSheet.Cells[modelRangeBorder])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White);
            }

            foreach (var color in _fillColor)
            {
                if (int.TryParse(color.Split(";")[1], out int column)
                    && int.TryParse(color.Split(";")[0], out int row))
                {
                    workSheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                }
            }

            // Set the tab color
            // Set page break view
            workSheet.View.PageBreakView = true;
            workSheet.PrinterSettings.FitToPage = true;
            workSheet.Row(_maxRow).PageBreak = true;
            workSheet.Column(12).PageBreak = true;


            // Set the tab color
            workSheet.TabColor = Color.FromArgb(255, 255, 0);
        }

        #region INVOICE
        private static void CreateHeaderInvoice(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Cells[1, 1].Value = invoice.Company?.Name.ToUpper();
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 16;
            workSheet.Cells[1, 1, 1, 12].Merge = true; // range column A -> M
            workSheet.Cells[1, 1, 1, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[1, 1, 1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.Company?.DisplayAddress.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 12;
            workSheet.Cells[_maxRow, 1, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            _maxRow++;
            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.InvoiceType?.Name.ToUpper() + " INVOICE";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 14;
            workSheet.Cells[_maxRow, 1, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            _maxRow++;
            var rowStart = _maxRow;
            workSheet.Cells[_maxRow, 1].Value = "INVOICE NO.: ";
            workSheet.Cells[_maxRow, 2].Value = invoice.Code;
            workSheet.Cells[_maxRow, 2, _maxRow, 3].Merge = true;


            workSheet.Cells[_maxRow, 11].Value = "DATE: "; // column L
            workSheet.Cells[_maxRow, 12].Value = invoice.Date; // column M
            workSheet.Cells[_maxRow, 12].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, 12].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));


            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Billing Company Full Name: " + invoice.Company?.Name.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Billing Company Address: " + invoice.Company?.DisplayAddress.ToUpper();

            _maxRow++;
            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Assembly Factory Full Name: " + invoice.Company?.Name.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Assembly Factory Address: " + invoice.Company?.DisplayAddress.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "EXP NO.:"; // column G
            workSheet.Cells[_maxRow, 9].Value = invoice.OnBoardDate; // Column I
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Shipped in goods order and condition PER:";
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;
            workSheet.Cells[_maxRow, 5].Value = invoice.VesselVoyageNo; // Column E


            workSheet.Cells[_maxRow, 7].Value = "ETD " + invoice.PortOfLoading?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 9].Value = invoice.EstimatedTimeOfDeparture;
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, 9].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Mother VSL:";
            workSheet.Cells[_maxRow, 5].Value = invoice.VesselVoyageNo;


            workSheet.Cells[_maxRow, 7].Value = "ETA " + invoice.PortOfDischarge?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 9].Value = invoice.EstimatedTimeOfArrival;
            workSheet.Cells[_maxRow, 9].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, 9].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
            workSheet.Cells[_maxRow, 9, _maxRow, 10].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Port of Loading: ";
            workSheet.Cells[_maxRow, 2].Value = invoice.PortOfLoading?.Name?.ToUpper() + ", " + invoice.PortOfLoading?.Country?.Name?.ToUpper();

            workSheet.Cells[_maxRow, 6].Value = "To: "; // COLUMN F
            workSheet.Cells[_maxRow, 7].Value = invoice.ShipTo?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = invoice.ShipTo?.Address?.ToUpper() + ", " + invoice.Consignee?.Country?.Code?.ToUpper();

            string contactInfo = invoice.ShipTo?.ContactName?.ToUpper();

            if (!string.IsNullOrEmpty(contactInfo))
            {
                contactInfo = "ANTT: " + contactInfo;
            }

            if (!string.IsNullOrEmpty(invoice.ShipTo?.PhoneNumber))
            {
                contactInfo += " SELF PHONE " + invoice.ShipTo?.PhoneNumber;
            }
            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = contactInfo;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Port of Discharge: ";
            workSheet.Cells[_maxRow, 2].Value = invoice.PortOfDischarge?.Name?.ToUpper()
                                                + ", " + invoice.CustomerID
                                                + ", " + invoice.PortOfDischarge?.Country?.Name?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "Container/Seal number: " + invoice.ContainerNo + "/" + invoice.SealNumber;
            workSheet.Cells[_maxRow, 7, _maxRow, 12].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 12].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));


            _maxRow++;
            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Consignee: ";
            workSheet.Cells[_maxRow, 2].Value = invoice.Consignee?.Name?.ToUpper();


            contactInfo = invoice.Consignee?.ContactName?.ToUpper();

            if (!string.IsNullOrEmpty(contactInfo))
            {
                contactInfo = ", ANTT: " + contactInfo;
            }

            if (!string.IsNullOrEmpty(invoice.Consignee?.PhoneNumber))
            {
                contactInfo += ", PHONE: " + invoice.Consignee?.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(invoice.Consignee?.Fax))
            {
                contactInfo += ", FAX: " + invoice.Consignee?.Fax;
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = invoice.Consignee?.Address?.ToUpper()
                                        + ", " + invoice.Consignee?.Country?.Code?.ToUpper()
                                        + contactInfo;

            _maxRow++;
            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SHIP TO: ";
            workSheet.Cells[_maxRow, 2].Value = invoice.ShipTo?.Name?.ToUpper();

            contactInfo = invoice.ShipTo?.ContactName?.ToUpper();

            if (!string.IsNullOrEmpty(contactInfo))
            {
                contactInfo = ", ANTT: " + contactInfo;
            }

            if (!string.IsNullOrEmpty(invoice.ShipTo?.PhoneNumber))
            {
                contactInfo += ", PHONE: " + invoice.ShipTo?.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(invoice.ShipTo?.Fax))
            {
                contactInfo += ", FAX: " + invoice.ShipTo?.Fax;
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = invoice.ShipTo?.Address?.ToUpper()
                                        + ", " + invoice.ShipTo?.Country?.Code?.ToUpper()
                                        + contactInfo;

            _maxRow++;
            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Applicant: ";
            workSheet.Cells[_maxRow, 2].Value = invoice.Customer?.FullName?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 2].Value = invoice.Customer?.Address?.ToUpper()
                                    + ", " + invoice.Customer?.Country?.Code?.ToUpper();

        }
        private static void CreateNotifyPartyInvoice(ExcelWorksheet workSheet, Invoice invoice)
        {
            int index = 1;
            foreach (var notifyParty in invoice.NotifyParties)
            {
                _maxRow++;
                workSheet.Cells[_maxRow, 1].Value = "Notify party-" + index;
                workSheet.Cells[_maxRow, 2].Value = notifyParty.Name?.ToUpper();

                string contactInfo = string.Empty;

                if (!string.IsNullOrEmpty(notifyParty?.ContactName))
                {
                    contactInfo += ", ANTT: " + notifyParty?.ContactName?.ToUpper();
                }

                if (!string.IsNullOrEmpty(notifyParty?.PhoneNumber))
                {
                    contactInfo += ", PHONE: " + notifyParty?.PhoneNumber;
                }

                if (!string.IsNullOrEmpty(notifyParty?.Fax))
                {
                    contactInfo += ", FAX: " + notifyParty?.Fax;
                }

                _maxRow++;
                workSheet.Cells[_maxRow, 2].Value = notifyParty?.Address?.ToUpper()
                                            + ", " + notifyParty?.Country?.Code?.ToUpper()
                                            + contactInfo;
                index++;
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Payment term: ";
            workSheet.Cells[_maxRow, 2].Value = invoice.PaymentTerm?.Description?.ToUpper();

            workSheet.Cells[_maxRow, 7].Value = "MBL NO: " + invoice.InvoiceDocument?
                                                                    .FirstOrDefault(x => x.InvoiceDocumentType?.Code == "BL")?
                                                                    .FileName?.ToUpper();
            workSheet.Cells[_maxRow, 7, _maxRow, 11].Merge = true;
            workSheet.Cells[_maxRow, 7, _maxRow, 11].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));

            _rowMBL = _maxRow;

            _maxRow++;

        }
        private static void CreateHeaderInvoiceDetail(ExcelWorksheet workSheet, Invoice invoice)
        {
            int column = 1;
            _maxRow++;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

            workSheet.Cells[_maxRow, column].Value = MARK_AND_NO;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 18.3;
            _dicPositionColumnPackingLine.Add(MARK_AND_NO, column); // add position column 

            column++;
            workSheet.Column(column).Width = 5;
            workSheet.Cells[_maxRow, column - 1, _maxRow, column].Merge = true;

            column++;
            workSheet.Cells[_maxRow, column].Value = DESCRIPTION_OF_GOODS;
            workSheet.Column(column).Width = 15;
            _dicPositionColumnPackingLine.Add(DESCRIPTION_OF_GOODS, column);

            column++;
            workSheet.Column(column).Width = 20;

            column++;
            workSheet.Column(column).Width = 20;

            column++;
            workSheet.Column(column).Width = 20;

            column++;
            workSheet.Column(column).Width = 10;

            column++;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 10;
            workSheet.Cells[_maxRow, column - 5, _maxRow, column].Merge = true;


            column++;
            workSheet.Cells[_maxRow, column].Value = QUANTITY;
            workSheet.Column(column).Width = 10;
            _dicPositionColumnPackingLine.Add(QUANTITY, column);

            column++;
            workSheet.Column(column).Width = 5.5;
            workSheet.Cells[_maxRow, column - 1, _maxRow, column].Merge = true;

            column++;
            workSheet.Cells[_maxRow, column].Value = UNIT_PRICE_EACH;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 18;
            _dicPositionColumnPackingLine.Add(UNIT_PRICE_EACH, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = TOTAL_AMOUNT;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Column(column).Width = 20;
            _dicPositionColumnPackingLine.Add(TOTAL_AMOUNT, column);

            workSheet.Cells[_maxRow, 1, _maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, column].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = "READY MADE GARMENTS";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Value = "FOB CAI MEP PORT, VIETNAM  INCOTERMS " + invoice.IncoTerm?.Year;

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = "Description  of Goods";

        }
        private static void FillDataInvoice(ExcelWorksheet workSheet, Invoice invoice)
        {
            var rowStartFill = _maxRow - 1;
            workSheet.Cells[_maxRow + 3, _dicPositionColumnPackingLine[MARK_AND_NO]].Value = "Buyer: GARAN";
            workSheet.Cells[_maxRow + 4, _dicPositionColumnPackingLine[MARK_AND_NO]].Value = "Printing: ";
            workSheet.Cells[_maxRow + 5, _dicPositionColumnPackingLine[MARK_AND_NO]].Value = "MADE IN VIETNAM";
            workSheet.Cells[_maxRow + 6, _dicPositionColumnPackingLine[MARK_AND_NO]].Value = "N.W: KGS";

            var symbol = invoice.Consignee.Country?.Symbol;
            var amount = (double)invoice.InvoiceDetails.Sum(x => x.Amount);
            var totalQuantity = (double)invoice.InvoiceDetails.Sum(x => x.Quantity);
            var totalCarton = (double)invoice.PackingList.Sum(x => x.PackingLines.Select(y => y.TotalCarton).FirstOrDefault());
            var totalAmount = amount + (amount * PERCENT / 100);

            foreach (var invoiceDetail in invoice.InvoiceDetails)
            {
                _maxRow++;
                _maxRow++;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = invoiceDetail.Description?.ToUpper() + ": "
                    + invoiceDetail.FabricContent?.ToUpper();

                _maxRow++;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = "PO NO.: "
                    + invoiceDetail.CustomerPurchaseOrderNumber;

                _maxRow++;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = "STYLE NO.: ";
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Style.Font.UnderLine = true;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 2].Value = "CLR CODE";
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 2].Style.Font.UnderLine = true;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 3].Value = "COLOR";
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 3].Style.Font.UnderLine = true;

                _maxRow++;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 1].Value = invoiceDetail.CustomerStyle?.ToUpper();
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 2].Value = invoiceDetail.GarmentColorCode?.ToUpper();
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 3].Value = invoiceDetail.GarmentColorName?.ToUpper();

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Value = invoiceDetail.Quantity;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Style.Numberformat.Format = "#,#";
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY] + 1].Value = invoiceDetail.UnitID.ToUpper();

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNIT_PRICE_EACH]].Value = invoiceDetail.UnitPrice;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNIT_PRICE_EACH]]
                    .Style.Numberformat.Format = "_ " + symbol + "* #,##0.000";

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Value = invoiceDetail.Amount;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]]
                    .Style.Numberformat.Format = "_ " + symbol + "* #,##0.00";
            }
            _maxRow += 5;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Value = totalQuantity;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Style.Numberformat.Format = "#,#";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY] + 1].Value = invoice.InvoiceDetails.FirstOrDefault().UnitID.ToUpper();

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Value = amount;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Numberformat.Format = "#,##0.00";

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Value = amount;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Numberformat.Format = symbol + "#,##0.00";

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = PERCENT + PERCENT_DEFECTIVE;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY]].Value = LESS_DEFECTIVE + PERCENT;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY], _maxRow, _dicPositionColumnPackingLine[QUANTITY] + 2].Merge = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Value = amount * PERCENT / 100;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Numberformat.Format = symbol + "#,##0.00";

            _maxRow++;
            var endRowQuantity = _maxRow;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]]
                .Value = "TOTAL: " + NumberToWordsHelpers.ToVerbalCurrency(totalQuantity, "PIECES");
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS],
                            _maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 5].Merge = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY], _maxRow, _dicPositionColumnPackingLine[QUANTITY] + 2].Merge = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Value = totalAmount;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Numberformat.Format = symbol + "#,##0.00";

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = SAY_TOTAL;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 1]
                .Value = NumberToWordsHelpers.ToVerbalCurrency(totalCarton, "CARTONS");
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS],
                            _maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 1].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = "TOTAL CARTONS: ";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 2].Value = totalCarton;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = SAY_TOTAL;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 1]
                .Value = NumberToWordsHelpers.ToVerbalCurrency(totalAmount);
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS],
                            _maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS] + 1].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
            _maxRow++;
            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]]
                .Value = "BANK DETAIL: " + invoice.Company.BankAccount?.BankName?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = invoice.Company.BankAccount?.Address?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]]
                .Value = "BANK ACCOUNT: " + invoice.Company.BankAccount?.BankAccountNumber;

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]]
                .Value = "SWIFT CODE: " + invoice.Company.BankAccount?.SwiftCode;

            _maxRow++;
            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = "Manufacturer(s) name and address:";

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = invoice.Company.Name?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = invoice.Company.DisplayAddress.Substring(0, 57);

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Value = invoice.Company.DisplayAddress.Substring(59);

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]]
                .Value = "No Forced Labor was used in production of the goods sold on this invoice.";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MARK_AND_NO],
                            _maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY] - 1].Value = "Assembly & Inspection Location Name & Address:";

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY] - 1].Value = invoice.Company.Name?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY] - 1].Value = invoice.Company.DisplayAddress.Substring(0, 57);

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[QUANTITY] - 1].Value = invoice.Company.DisplayAddress.Substring(59);
            workSheet.Cells[_maxRow - 4, _dicPositionColumnPackingLine[QUANTITY] - 1,
                            _maxRow, _dicPositionColumnPackingLine[QUANTITY] - 1].Style.Font.SetFromFont(new Font("Times New Roman", 10));

            //Draw botton border start row -> Fill
            workSheet.Cells[5, 1, rowStartFill - 2, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            //Draw border Fill
            workSheet.Cells[rowStartFill, _dicPositionColumnPackingLine[MARK_AND_NO],
                            _maxRow, _dicPositionColumnPackingLine[MARK_AND_NO]].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[rowStartFill, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS],
                            _maxRow - 4, _dicPositionColumnPackingLine[DESCRIPTION_OF_GOODS]].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[rowStartFill, _dicPositionColumnPackingLine[QUANTITY],
                            endRowQuantity, _dicPositionColumnPackingLine[QUANTITY]].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[rowStartFill, _dicPositionColumnPackingLine[QUANTITY],
                            rowStartFill, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[rowStartFill + 1, _dicPositionColumnPackingLine[UNIT_PRICE_EACH],
                            endRowQuantity, _dicPositionColumnPackingLine[UNIT_PRICE_EACH]].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[rowStartFill + 1, _dicPositionColumnPackingLine[TOTAL_AMOUNT],
                            endRowQuantity, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[rowStartFill, _dicPositionColumnPackingLine[TOTAL_AMOUNT],
                            _maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MARK_AND_NO],
                            _maxRow, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[endRowQuantity - 2, _dicPositionColumnPackingLine[QUANTITY],
                            endRowQuantity - 2, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            workSheet.Cells[endRowQuantity, _dicPositionColumnPackingLine[QUANTITY],
                            endRowQuantity, _dicPositionColumnPackingLine[TOTAL_AMOUNT]].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        }
        #endregion INVOICE

        #region PACKING LIST
        private static void CreateHeaderPL(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Cells[1, 1].Value = invoice.Company?.Name.ToUpper();
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 16;
            workSheet.Cells[1, 1, 1, _colSize + 11].Merge = true; // range column A -> M
            workSheet.Cells[1, 1, 1, _colSize + 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[1, 1, 1, _colSize + 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = invoice.Company?.DisplayAddress.ToUpper();
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 12;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize + 11].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize + 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize + 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            _maxRow += 3;
            workSheet.Cells[_maxRow, 1].Value = "PACKING LIST";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].Style.Font.UnderLine = true;
            workSheet.Cells[_maxRow, 1].Style.Font.Size = 16;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize + 11].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize + 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize + 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow += 2;
            var rowStart = _maxRow;
            workSheet.Cells[_maxRow, 1].Value = "INVOICE NO.: ";
            workSheet.Cells[_maxRow, 5].Value = invoice.Code;


            workSheet.Cells[_maxRow, _colSize + 3].Value = "Date: ";
            workSheet.Cells[_maxRow, _colSize + 4].Value = invoice.Date;
            workSheet.Cells[_maxRow, _colSize + 4].Style.Numberformat.Format = "dd-MMM-yy";
            workSheet.Cells[_maxRow, _colSize + 4, _maxRow, _colSize + 6].Merge = true;
            workSheet.Cells[_maxRow, _colSize + 4, _maxRow, _colSize + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "MESSRS.: ";
            workSheet.Cells[_maxRow, 5].Value = invoice.ShipTo?.Name?.ToUpper();

            _maxRow++;

            string contactInfo = invoice.ShipTo?.Address?.ToUpper() + ", "
                                + invoice.Consignee?.Country?.Code?.ToUpper();

            if (!string.IsNullOrEmpty(contactInfo))
            {
                contactInfo += " ANTT: " + invoice?.ShipTo?.ContactName?.ToUpper();
            }

            if (!string.IsNullOrEmpty(invoice.ShipTo?.PhoneNumber))
            {
                contactInfo += " SELF PHONE " + invoice.ShipTo?.PhoneNumber;
            }
            workSheet.Cells[_maxRow, 5].Value = contactInfo;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "INVOICE OF: ";
            workSheet.Cells[_maxRow, 5].Value = "AS PER INVOICE";

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "SHIPPER:";
            workSheet.Cells[_maxRow, 5].Value = invoice.Company?.Name.ToUpper();

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "Port of Loading: ";
            workSheet.Cells[_maxRow, 5].Value = invoice.PortOfLoading?.Name?.ToUpper() + ", " + invoice.PortOfLoading?.Country?.Name?.ToUpper();

            workSheet.Cells[_maxRow, _colSize + 1].Value = "Port of Discharge: " +
                                invoice.PortOfDischarge?.Name?.ToUpper() + ", " + invoice.CustomerID
                                + ", " + invoice.PortOfDischarge?.Country?.Name?.ToUpper();
            _maxRow++;
            workSheet.Cells[_maxRow, 5].Value = invoice.ShipTo?.Name?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 5].Value = contactInfo;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "PAYMENT TERM";
            workSheet.Cells[_maxRow, 5].Value = invoice.PaymentTerm?.Description?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "FINAL DESTINATION:";
            workSheet.Cells[_maxRow, 5].Value = invoice.ShipTo?.Name?.ToUpper();

            _maxRow++;
            workSheet.Cells[_maxRow, 5].Value = contactInfo;

            _maxRow += 2;
        }

        private static void CreateHeaderDataPL(ExcelWorksheet workSheet, int row, PackingList packingList)
        {
            int column = 1;
            _maxRow++;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

            string range = "A" + row + ":C" + (row + 3);
            workSheet.Cells[row, column].Value = "STICKER / C / NO.";
            workSheet.Cells[row, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(FROM_NO, 1); // add position column 
            _dicPositionColumnPackingLine.Add(TO_NO, 3);
            workSheet.Column(column).Width = 6.3;
            workSheet.Column(column + 1).Width = 2.5;
            workSheet.Column(column + 2).Width = 14.5;
            workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column += 3;

            range = "D" + row + ":D" + (row + 3);
            workSheet.Cells[row, column].Value = CNTS;
            workSheet.Cells[row, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            workSheet.Column(column).Width = 21.5;
            _dicPositionColumnPackingLine.Add(CNTS, column);
            workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;

            range = "E" + row + ":" + _dicAlphabel[_colSize] + row;
            workSheet.Cells[row, column].Value = DESCRIPTION;
            workSheet.Cells[range].Merge = true;
            workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[row + 1, column].Value = PO_NO;
            _dicPositionColumnPackingLine.Add(PO_NO, column + 2);

            workSheet.Cells[row + 3, column].Value = COLOR;
            _dicPositionColumnPackingLine.Add(COLOR, column);
            workSheet.Column(column).Width = 20.2;
            workSheet.Row(row + 3).Height = 37.8;
            workSheet.Cells[row + 3, column].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells[row + 3, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row + 3, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;

            workSheet.Cells[row + 3, column].Value = COLOR_NO;
            _dicPositionColumnPackingLine.Add(COLOR_NO, column);
            workSheet.Column(column).Width = 15.2;
            workSheet.Cells[row + 3, column].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells[row + 3, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[row + 3, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            column++;

            range = "G" + (row + 2) + ":" + _dicAlphabel[_colSize] + (row + 2);
            workSheet.Cells[row + 2, column].Value = SIZE;
            workSheet.Cells[range].Merge = true;
            workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var sizeList = packingList.ItemStyles.FirstOrDefault()
                    .OrderDetails.OrderBy(x => x.SizeSortIndex).ToList();

            for (int i = 0; i < _countSize; i++)
            {
                string size = sizeList[i].Size;
                if (!string.IsNullOrEmpty(size))
                {
                    workSheet.Cells[row + 3, column].Value = size;
                    workSheet.Cells[row + 3, column].Style.WrapText = true;
                    _dicPositionColumnPackingLine.Add(size, column);
                    workSheet.Column(column).Width = 6.8;
                    workSheet.Cells[row + 3, column].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    workSheet.Cells[row + 3, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[row + 3, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    column++;
                }
            }

            string colChar = "";
            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + row + ":" + colChar + (row + 3);
                workSheet.Cells[row, column].Value = UNITS_CARTON.Replace('@', '\n');
                workSheet.Cells[range].Merge = true;
                workSheet.Cells[range].Style.WrapText = true;
                workSheet.Column(column).Width = 7.6;
                _dicPositionColumnPackingLine.Add(UNITS_CARTON, column);
                workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + row + ":" + colChar + (row + 3);
                workSheet.Cells[row, column].Value = TOTAL_UNITS.Replace('@', '\n');
                _dicPositionColumnPackingLine.Add(TOTAL_UNITS, column);
                workSheet.Cells[row, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 11.3;
                workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + row + ":" + colChar + (row + 3);
                workSheet.Cells[row, column].Value = GW_CARTON.Replace('@', '\n');
                workSheet.Cells[row, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 6.3;
                _dicPositionColumnPackingLine.Add(GW_CARTON, column);
                workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + row + ":" + colChar + (row + 3);
                workSheet.Cells[row, column].Value = TOTAL_GW.Replace('@', '\n');
                workSheet.Cells[row, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10.2;
                _dicPositionColumnPackingLine.Add(TOTAL_GW, column);
                workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + row + ":" + colChar + (row + 3);
                workSheet.Cells[row, column].Value = NW_CARTON.Replace('@', '\n');
                workSheet.Cells[row, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 6.3;
                _dicPositionColumnPackingLine.Add(NW_CARTON, column);
                workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out colChar))
            {
                range = colChar + row + ":" + colChar + (row + 3);
                workSheet.Cells[row, column].Value = TOTAL_NW.Replace('@', '\n');
                workSheet.Cells[row, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10.2;
                _dicPositionColumnPackingLine.Add(TOTAL_NW, column);
                workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out colChar) &&
                _dicAlphabel.TryGetValue(column + 4, out string colLastChar))
            {
                range = colChar + row + ":" + colLastChar + (row + 3);
                workSheet.Cells[row, column].Value = CARTON_DIMENSION.Replace('@', '\n');
                workSheet.Cells[row, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Cells[range].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                _dicPositionColumnPackingLine.Add(LENGTH, column);
                workSheet.Column(column).Width = 5.5;

                workSheet.Column(column + 1).Width = 2.5;

                _dicPositionColumnPackingLine.Add(WIDTH, column + 2);
                workSheet.Column(column + 2).Width = 5.5;

                workSheet.Column(column + 3).Width = 2.5;

                _dicPositionColumnPackingLine.Add(HEIGHT, column + 4);
                workSheet.Column(column + 4).Width = 5.5;
            }

            _maxRow = row + 4;
        }
        private static void FillDataPL(ExcelWorksheet workSheet, Invoice invoice)
        {
            var inchConvertValue = (decimal)2.54;
            _dicTotalQuantitySize = new Dictionary<string, decimal>();

            foreach (var packinglist in invoice.PackingList)
            {
                var prePack = packinglist.PackingLines.FirstOrDefault().PrePack;
                var styleColorName = "";
                var styleColorCode = "";
                decimal netWeight = 0;
                decimal grossWeight = 0;
                decimal totCarton = 0;
                decimal totUnits = 0;
                decimal totGW = 0;
                decimal totNW = 0;
                decimal totMeanM3 = 0;

                CreateHeaderDataPL(workSheet, _maxRow, packinglist);
                var rowBorder = _maxRow;
                workSheet.Cells[_maxRow - 3, _dicPositionColumnPackingLine[PO_NO]]
                        .Value = packinglist.ItemStyles.FirstOrDefault().PurchaseOrderNumber;

                var sortPackingLines = packinglist.PackingLines.OrderBy(x => x.SequenceNo)
                                            .OrderByDescending(x => x.TotalCarton).ToList();
                var styleCount = packinglist.ItemStyles.Count;

                var sizePacking = sortPackingLines?.FirstOrDefault()?.Size;

                foreach (var data in packinglist.ItemStyles)
                {
                    if (string.IsNullOrEmpty(styleColorName.Trim()))
                        styleColorName = data.ColorName.Trim();
                    else
                        styleColorName += " @/ " + data.ColorName.Trim();

                    if (string.IsNullOrEmpty(styleColorCode.Trim()))
                        styleColorCode = data.ColorCode.Trim();
                    else
                        styleColorCode += " @/ " + data.ColorCode.Trim();

                    /// Calculation net weight for assorted
                    netWeight += (decimal)(sortPackingLines
                        .Where(x => x.LSStyle == data.LSStyle && x.Size == sizePacking)?.FirstOrDefault()?.NetWeight ?? 0);
                }

                if (prePack == "Assorted Size - Solid Color")
                {
                    var summaryCarton = (int)sortPackingLines[0].TotalCarton;
                    var lsStyle = sortPackingLines[0].LSStyle;
                    var firstRow = true;
                    foreach (var packingLine in sortPackingLines)
                    {
                        var itemStyle = packinglist.ItemStyles.FirstOrDefault(x => x.LSStyle == packingLine.LSStyle);

                        if (packingLine.TotalCarton != summaryCarton)
                        {
                            firstRow = true;
                            lsStyle = packingLine.LSStyle;
                            _maxRow++;
                            workSheet.Row(_maxRow).Height = 21.6;
                        }
                        if (firstRow || packingLine.LSStyle == lsStyle)
                        {
                            if (firstRow)
                            {
                                /// Convert packing unit
                                var measM3 = Math.Round((decimal)(packingLine.Length * inchConvertValue)
                                                    * (decimal)(packingLine.Width * inchConvertValue)
                                                    * (decimal)(packingLine.Height * inchConvertValue)
                                                    * (decimal)packingLine.TotalCarton / 1000000, 2);

                                grossWeight = (decimal)(netWeight + packingLine.GrossWeight - packingLine.NetWeight);

                                workSheet.Row(_maxRow).Height = 21.6;

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CNTS]].Value = packingLine.TotalCarton;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_NO]].Value = itemStyle.ColorCode;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Style.Font.Bold = false;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton * styleCount;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = packingLine.QuantityPerCarton * styleCount * packingLine.TotalCarton;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Value = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Value = Math.Round(grossWeight, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Value = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 2);
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Value = Math.Round(netWeight, 2);

                                if (packingLine.BoxDimensionCode.Contains("*"))
                                {
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.BoxDimensionCode.Split("*")[0].Trim();
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH] + 1].Value = "x";
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.BoxDimensionCode.Split("*")[1].Trim();
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH] + 1].Value = "x";
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.BoxDimensionCode.Split("*")[2].Trim();
                                }
                                else
                                {
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]]
                                            .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[0].Trim();
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH] + 1].Value = "x";
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]]
                                            .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[1].Trim();
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH] + 1].Value = "x";
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                            .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[2].Trim();
                                }

                                var key = itemStyle.LSStyle + ";" + packingLine.Size;
                                if (_dicTotalQuantitySize.TryGetValue(key, out decimal quantity))
                                {
                                    quantity += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                    _dicTotalQuantitySize[key] = quantity;
                                }
                                else
                                {
                                    _dicTotalQuantitySize[key] = (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                }

                                totCarton += (int)packingLine.TotalCarton;
                                totNW += (decimal)Math.Round(netWeight, 2);
                                totGW += (decimal)Math.Round(grossWeight, 2);
                                totMeanM3 += Math.Round(measM3, 2);
                                totUnits += (decimal)(packingLine.QuantityPerCarton * styleCount * packingLine.TotalCarton);

                                _totalCarton += (int)packingLine.TotalCarton;
                                _totalNW += (decimal)Math.Round(netWeight, 2);
                                _totalGW += (decimal)Math.Round(grossWeight, 2);
                                _totalMeanM3 += Math.Round(measM3, 2);
                                _totalUnits += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);

                                firstRow = false;
                                summaryCarton = (int)packingLine.TotalCarton;
                                lsStyle = packingLine.LSStyle;
                            }
                            else
                            {
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Style.Font.Bold = false;

                                var key = itemStyle.LSStyle + ";" + packingLine.Size;
                                if (_dicTotalQuantitySize.TryGetValue(key, out decimal quantity))
                                {
                                    quantity += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                    _dicTotalQuantitySize[key] = quantity;
                                }
                                else
                                {
                                    _dicTotalQuantitySize[key] = (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                }

                                _totalUnits += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                summaryCarton = (int)packingLine.TotalCarton;
                                lsStyle = packingLine.LSStyle;
                            }
                        }
                        else
                        {
                            _maxRow++;
                            workSheet.Row(_maxRow).Height = 21.6;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_NO]].Value = itemStyle.ColorCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Style.Font.Bold = false;

                            var key = itemStyle.LSStyle + ";" + packingLine.Size;
                            if (_dicTotalQuantitySize.TryGetValue(key, out decimal quantity))
                            {
                                quantity += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                                _dicTotalQuantitySize[key] = quantity;
                            }
                            else
                            {
                                _dicTotalQuantitySize[key] = (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                            }

                            _totalUnits += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton);
                            summaryCarton = (int)packingLine.TotalCarton;
                            lsStyle = packingLine.LSStyle;
                        }
                    }

                    //// Merge Cell 
                    workSheet.Cells["A" + rowBorder + ":" + "A" + _maxRow].Merge = true;
                    workSheet.Cells["A" + rowBorder + ":" + "A" + _maxRow].Style.Font.Bold = false;

                    workSheet.Cells["B" + rowBorder + ":" + "B" + _maxRow].Merge = true;
                    workSheet.Cells["B" + rowBorder + ":" + "B" + _maxRow].Style.Font.Bold = false;

                    workSheet.Cells["C" + rowBorder + ":" + "C" + _maxRow].Merge = true;
                    workSheet.Cells["C" + rowBorder + ":" + "C" + _maxRow].Style.Font.Bold = false;

                    workSheet.Cells["D" + rowBorder + ":" + "D" + _maxRow].Merge = true;
                    workSheet.Cells["D" + rowBorder + ":" + "D" + _maxRow].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[UNITS_CARTON],
                                    _maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[UNITS_CARTON],
                                    _maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[TOTAL_UNITS],
                                    _maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[TOTAL_UNITS],
                                    _maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[GW_CARTON],
                                    _maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[GW_CARTON],
                                    _maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[TOTAL_GW],
                                    _maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[TOTAL_GW],
                                    _maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[NW_CARTON],
                                    _maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[NW_CARTON],
                                    _maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[TOTAL_NW],
                                    _maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[TOTAL_NW],
                                    _maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[LENGTH],
                                    _maxRow, _dicPositionColumnPackingLine[LENGTH]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[LENGTH],
                                    _maxRow, _dicPositionColumnPackingLine[LENGTH]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[LENGTH] + 1,
                                    _maxRow, _dicPositionColumnPackingLine[LENGTH] + 1].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[LENGTH] + 1,
                                    _maxRow, _dicPositionColumnPackingLine[LENGTH] + 1].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[WIDTH],
                                    _maxRow, _dicPositionColumnPackingLine[WIDTH]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[WIDTH],
                                    _maxRow, _dicPositionColumnPackingLine[WIDTH]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[WIDTH] + 1,
                                    _maxRow, _dicPositionColumnPackingLine[WIDTH] + 1].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[WIDTH] + 1,
                                    _maxRow, _dicPositionColumnPackingLine[WIDTH] + 1].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[HEIGHT],
                                    _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Merge = true;
                    workSheet.Cells[rowBorder, _dicPositionColumnPackingLine[HEIGHT],
                                    _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Font.Bold = false;

                    workSheet.Cells[rowBorder, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                            .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowBorder, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                            .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    _maxRow++;
                }
                else
                {
                    var firstRow = true;
                    var itemStyle = packinglist.ItemStyles.OrderBy(x => x.LSStyle).FirstOrDefault();

                    sortPackingLines = packinglist.PackingLines
                            .Where(x => x.LSStyle == itemStyle.LSStyle).OrderBy(x => x.SequenceNo).ToList();

                    foreach (var packingLine in sortPackingLines)
                    {
                        /// Convert packing unit
                        var measM3 = Math.Round((decimal)(packingLine.Length * inchConvertValue)
                                                    * (decimal)(packingLine.Width * inchConvertValue)
                                                    * (decimal)(packingLine.Height * inchConvertValue)
                                                    * (decimal)packingLine.TotalCarton / 1000000, 2);

                        netWeight = (decimal)(packinglist.PackingLines.Where(x => x.Size == packingLine.Size).Sum(x => x.NetWeight));
                        grossWeight = (decimal)(netWeight + (packingLine.BoxDimension.Weight * packingLine.TotalCarton));

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[CNTS]].Value = packingLine.TotalCarton;

                        if (firstRow)
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = styleColorName.Replace('@', '\n');
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_NO]].Value = styleColorCode.Replace('@', '\n');
                        }

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity * styleCount;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton * styleCount;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                                                = packingLine.QuantityPerCarton * packingLine.TotalCarton * styleCount;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Value
                                                = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Value = Math.Round(grossWeight, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NW_CARTON]].Value
                                                = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Value = Math.Round(netWeight, 2);

                        if (packingLine.BoxDimensionCode.Contains("*"))
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.BoxDimensionCode.Split("*")[0].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH] + 1].Value = "x";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.BoxDimensionCode.Split("*")[1].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH] + 1].Value = "x";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.BoxDimensionCode.Split("*")[2].Trim();
                        }
                        else
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[0].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH] + 1].Value = "x";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[1].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH] + 1].Value = "x";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[2].Trim();
                        }

                        foreach (var style in packinglist.ItemStyles)
                        {
                            var stylepackingLines = packinglist.PackingLines
                                .Where(x => x.LSStyle == style.LSStyle && x.Size == packingLine.Size).ToList();
                            decimal quantitySize = 0;
                            var key = style.LSStyle + ";" + packingLine.Size;
                            if (_dicTotalQuantitySize.TryGetValue(key, out decimal quantity))
                            {
                                stylepackingLines.ForEach(p =>
                                {
                                    quantitySize += (decimal)(p.QuantitySize * p.TotalCarton);
                                });
                                quantity += quantitySize;
                                _dicTotalQuantitySize[key] = quantity;
                            }
                            else
                            {
                                stylepackingLines.ForEach(p =>
                                {
                                    quantitySize += (decimal)(p.QuantitySize * p.TotalCarton);
                                });
                                _dicTotalQuantitySize[key] = quantitySize;
                            }
                        }

                        totCarton += (int)packingLine.TotalCarton;
                        totNW += (decimal)Math.Round(netWeight, 2);
                        totGW += (decimal)Math.Round(grossWeight, 2);
                        totMeanM3 += Math.Round(measM3, 2);
                        totUnits += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton * styleCount);

                        _totalCarton += (int)packingLine.TotalCarton;
                        _totalNW += (decimal)Math.Round(netWeight, 2);
                        _totalGW += (decimal)Math.Round(grossWeight, 2);
                        _totalMeanM3 += Math.Round(measM3, 2);
                        _totalUnits += (decimal)(packingLine.QuantitySize * packingLine.TotalCarton * styleCount);

                        firstRow = false;
                        workSheet.Row(_maxRow).Height = 21.6;
                        _maxRow++;
                    }
                    workSheet.Cells[rowBorder, 1, (_maxRow - 1), _dicPositionColumnPackingLine[HEIGHT]]
                                                    .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowBorder, 1, (_maxRow - 1), _dicPositionColumnPackingLine[HEIGHT]]
                                                    .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowBorder, 1, (_maxRow - 1), _dicPositionColumnPackingLine[HEIGHT]]
                                                    .Style.Font.Bold = false;

                    workSheet.Cells["E" + rowBorder + ":" + "E" + (_maxRow - 1)].Merge = true;
                    workSheet.Cells["E" + rowBorder + ":" + "E" + (_maxRow - 1)].Style.Font.Bold = true;
                    workSheet.Cells["E" + rowBorder + ":" + "E" + (_maxRow - 1)].Style.WrapText = true;

                    workSheet.Cells["F" + rowBorder + ":" + "F" + (_maxRow - 1)].Merge = true;
                    workSheet.Cells["F" + rowBorder + ":" + "F" + (_maxRow - 1)].Style.Font.Bold = true;
                    workSheet.Cells["F" + rowBorder + ":" + "F" + (_maxRow - 1)].Style.WrapText = true;
                }

                /// Display total
                workSheet.Row(_maxRow).Height = 21.6;
                string range = "A" + _maxRow + ":C" + _maxRow;
                workSheet.Cells[_maxRow, 1].Value = "TOTAL:";
                workSheet.Cells[range].Merge = true;

                workSheet.Cells[_maxRow, 4].Value = totCarton;
                workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, 4].Style.Numberformat.Format = "#,##0";

                workSheet.Cells[_maxRow, 5].Value = "CARTONS";
                workSheet.Cells[_maxRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = totUnits;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Numberformat.Format = "#,##0";

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Value = totGW;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Style.Numberformat.Format = "#,##0.00";

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Value = totNW;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Style.Numberformat.Format = "#,##0.00";

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = totMeanM3 + " CBM AS PER B/L";
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH],
                                _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Merge = true;

                workSheet.Cells[rowBorder, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowBorder, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowBorder, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowBorder, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowBorder, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                _maxRow += 2;
            }
        }
        private static void SummaryDataPL(ExcelWorksheet workSheet, Invoice invoice)
        {
            workSheet.Row(_maxRow - 1).Height = 5.5;

            /// Display grand total
            workSheet.Row(_maxRow).Height = 21.6;
            string range = "A" + _maxRow + ":C" + _maxRow;
            workSheet.Cells[_maxRow, 1].Value = "GRAND TOTAL:";
            workSheet.Cells[range].Merge = true;

            workSheet.Cells[_maxRow, 4].Value = _totalCarton;
            workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 4].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, 5].Value = "CARTONS";
            workSheet.Cells[_maxRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow, 6, _maxRow, _colSize].Merge = true;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _totalUnits;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Numberformat.Format = "#,##0";

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Value = _totalGW;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GW]].Style.Numberformat.Format = "#,##0.00";

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Value = _totalNW;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NW]].Style.Numberformat.Format = "#,##0.00";

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = _totalMeanM3 + " CBM AS PER B/L";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH],
                            _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Merge = true;

            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[HEIGHT]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, 4].Value = "SAY TOTAL PACKED";

            workSheet.Cells[_maxRow, 5].Value = NumberToWordsHelpers.ToVerbalCurrency((double)_totalCarton, "CARTONS");

            _maxRow++;
            workSheet.Row(_maxRow).Height = 8.5;

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "TOTAL CARTONS:";
            workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 8].Value = _totalCarton;
            workSheet.Cells[_maxRow, 8].Style.Numberformat.Format = "#,##0";
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 8, _maxRow, 9].Merge = true;

            workSheet.Cells[_maxRow, 10].Value = "CTNS";

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "TOTAL Q'TY:";
            workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 8].Value = _totalUnits;
            workSheet.Cells[_maxRow, 8].Style.Numberformat.Format = "#,##0";
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 8, _maxRow, 9].Merge = true;

            workSheet.Cells[_maxRow, 10].Value = "PCS";

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "TOTAL G.W.:";
            workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 8].Value = _totalGW;
            workSheet.Cells[_maxRow, 8].Style.Numberformat.Format = "#,##0.00";
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 8, _maxRow, 9].Merge = true;

            workSheet.Cells[_maxRow, 10].Value = "KGS";

            _maxRow++;
            workSheet.Cells[_maxRow, 7].Value = "TOTAL N.W.:";
            workSheet.Cells[_maxRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, 8].Value = _totalNW;
            workSheet.Cells[_maxRow, 8].Style.Numberformat.Format = "#,##0.00";
            workSheet.Cells[_maxRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 8, _maxRow, 9].Merge = true;

            workSheet.Cells[_maxRow, 10].Value = "KGS";

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "COLOR/SIZE BREAKDOWN:";

            _maxRow++;
            var rowBorder = _maxRow;
            var keyCompare = "";
            decimal grandTotal = 0;
            decimal totalStyle = 0;

            var sizeList = invoice.PackingList.FirstOrDefault()
                .ItemStyles.FirstOrDefault().OrderDetails.OrderBy(x => x.SizeSortIndex).ToList();
            var itemStyles = new List<ItemStyle>();
            invoice.PackingList.ToList().ForEach(x =>
            {
                x.ItemStyles.ToList().ForEach(i =>
                {
                    itemStyles.Add(i);

                    sizeList.ForEach(s =>
                    {
                        if (!_dicTotalQuantitySize.ContainsKey(i.LSStyle + ";" + s.Size))
                        {
                            _dicTotalQuantitySize.Add(i.LSStyle + ";" + s.Size, 0);
                        }
                    });
                });
            });

            var lastSize = sizeList.OrderByDescending(x => x.Size).FirstOrDefault().Size;
            foreach (var data in _dicTotalQuantitySize.OrderBy(x => x.Key))
            {
                var size = data.Key.Split(";")[1];
                if (data.Key.Split(";")[0] != keyCompare)
                {
                    _maxRow++;
                    rowBorder = _maxRow;
                    totalStyle = 0;
                    var itemStyle = itemStyles.FirstOrDefault(x => x.LSStyle == data.Key.Split(";")[0]);

                    /// Header
                    range = "A" + _maxRow + ":C" + _maxRow;
                    workSheet.Cells[_maxRow, 1].Value = "PO NO.";
                    workSheet.Cells[range].Merge = true;
                    workSheet.Row(_maxRow).Height = 36;
                    workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    range = "A" + (_maxRow + 1) + ":C" + (_maxRow + 1);
                    workSheet.Cells[(_maxRow + 1), 1].Value = itemStyle?.PurchaseOrderNumber;
                    workSheet.Cells[range].Merge = true;
                    workSheet.Row(_maxRow + 1).Height = 21.6;
                    workSheet.Cells[range].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[range].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                    workSheet.Cells[_maxRow, 4].Value = "STYLE NO.";
                    workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[_maxRow, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    workSheet.Cells[_maxRow + 1, 4].Value = itemStyle?.CustomerStyle;
                    workSheet.Cells[_maxRow + 1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[_maxRow + 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    workSheet.Cells[_maxRow, 5].Value = "COLOR & COLOR#";
                    workSheet.Cells[_maxRow, 5, _maxRow, 6].Merge = true;
                    workSheet.Cells[_maxRow, 5, _maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[_maxRow, 5, _maxRow, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    workSheet.Cells[_maxRow + 1, 5].Value = itemStyle?.ColorName;
                    workSheet.Cells[_maxRow + 1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[_maxRow + 1, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    workSheet.Cells[_maxRow + 1, 6].Value = itemStyle?.ColorCode;
                    workSheet.Cells[_maxRow + 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[_maxRow + 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                    for (int i = 0; i < _countSize; i++)
                    {
                        string sizeName = sizeList[i].Size;
                        if (!string.IsNullOrEmpty(sizeName))
                        {
                            workSheet.Cells[_maxRow, 7 + i].Value = sizeName;
                            workSheet.Cells[_maxRow, 7 + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[_maxRow, 7 + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }
                    }

                    workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Value = data.Value;
                    workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    totalStyle += data.Value;
                    grandTotal += data.Value;

                    workSheet.Cells[_maxRow, _colSize + 1].Value = "TOTAL";
                    workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 3].Merge = true;
                    workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    keyCompare = data.Key.Split(";")[0];
                }
                else
                {
                    if (data.Key.Split(";")[1] != lastSize)
                    {
                        workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Value = data.Value;
                        workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        totalStyle += data.Value;
                        grandTotal += data.Value;
                    }
                    else
                    {
                        workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Value = data.Value;
                        workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[size]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        totalStyle += data.Value;
                        grandTotal += data.Value;

                        workSheet.Cells[_maxRow + 1, _colSize + 1].Value = totalStyle;
                        workSheet.Cells[_maxRow + 1, _colSize + 1, _maxRow + 1, _colSize + 2].Merge = true;
                        workSheet.Cells[_maxRow + 1, _colSize + 1, _maxRow + 1, _colSize + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow + 1, _colSize + 1, _maxRow + 1, _colSize + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        workSheet.Cells[_maxRow + 1, _colSize + 3].Value = "PCS";
                        workSheet.Cells[_maxRow + 1, _colSize + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                        workSheet.Cells[rowBorder, 1, _maxRow + 1, _colSize + 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[rowBorder, 1, _maxRow + 1, _colSize + 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[rowBorder, 1, _maxRow + 1, _colSize + 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[rowBorder, 1, _maxRow + 1, _colSize + 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[rowBorder, 1, _maxRow + 1, _colSize + 3].Style.Numberformat.Format = "#,##0";


                        _maxRow += 2;
                        workSheet.Cells[_maxRow, _colSize - 1].Value = "TOTAL =";
                        workSheet.Row(_maxRow).Height = 21.6;
                        workSheet.Cells[_maxRow, _colSize - 1, _maxRow, _colSize].Merge = true;
                        workSheet.Cells[_maxRow, _colSize - 1, _maxRow, _colSize].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[_maxRow, _colSize - 1, _maxRow, _colSize].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        workSheet.Cells[_maxRow, _colSize + 1].Value = totalStyle;
                        workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Merge = true;
                        workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Style.Numberformat.Format = "#,##0";

                        workSheet.Cells[_maxRow, _colSize + 3].Value = "PCS";
                        workSheet.Cells[_maxRow, _colSize + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        workSheet.Cells[_maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[_maxRow, _colSize + 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    keyCompare = data.Key.Split(";")[0];
                }
            }

            _maxRow++;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize + 3].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Row(_maxRow).Height = 5;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "GRAND TOTAL =";
            workSheet.Row(_maxRow).Height = 43.8;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize].Merge = true;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 1, _maxRow, _colSize].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, _colSize + 1].Value = grandTotal;
            workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Merge = true;
            workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Style.Numberformat.Format = "#,##0";
            workSheet.Cells[_maxRow, _colSize + 1, _maxRow, _colSize + 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, _colSize + 3].Value = "PCS";
            workSheet.Cells[_maxRow, _colSize + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, _colSize + 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, _colSize + 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            _maxRow += 2;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = "Assembly & Inspection Location Name & Address:";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.UnderLine = true;

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = invoice.Company.Name?.ToUpper();
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = false;

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = invoice.Company.DisplayAddress.Substring(0, 57);
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = false;

            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = invoice.Company.DisplayAddress.Substring(59);
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = false;
            //workSheet.Cells[_maxRow - 4, _dicPositionColumnPackingLine[TOTAL_UNITS],
            //                _maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.SetFromFont(new Font("Times New Roman", 10));
        }

        #endregion PACKING LIST

        #region STD PACKING LIST
        private static void CreateHeaderSTDPL(ExcelWorksheet workSheet, Invoice invoice)
        {
            var purchaseOrderNo = "";
            foreach (var data in invoice.PackingList)
            {
                if (!string.IsNullOrEmpty(data.ItemStyles.FirstOrDefault().PurchaseOrderNumber) &&
                    string.IsNullOrEmpty(purchaseOrderNo))
                {
                    purchaseOrderNo = data.ItemStyles.FirstOrDefault().PurchaseOrderNumber;
                }
                else if (!purchaseOrderNo.Contains(data.ItemStyles.FirstOrDefault().PurchaseOrderNumber))
                {
                    purchaseOrderNo += "/" + data.ItemStyles.FirstOrDefault().PurchaseOrderNumber;
                }
            }
            _maxRow++;
            workSheet.Row(_maxRow).Height = 24;
            workSheet.Column(1).Width = 17.1;
            workSheet.Cells[_maxRow, 1].Value = "Garan Purch.Order #:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, _colSize + 3].Value = "Container #";
            workSheet.Cells[_maxRow, _colSize + 3].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _colSize + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, _colSize + 4].Value = invoice.ContainerNo;
            workSheet.Cells[_maxRow, _colSize + 4].Style.Font.UnderLine = true;
            _fillColor.Add(_maxRow + ";" + (_colSize + 4));

            workSheet.Cells[_maxRow, _colEndSize + 1].Value = "page";
            workSheet.Cells[_maxRow, _colEndSize + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow, _colEndSize + 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            workSheet.Cells[_maxRow, _colEndSize + 3].Value = "of";
            workSheet.Cells[_maxRow, _colEndSize + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[_maxRow, _colEndSize + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            _maxRow++;
            workSheet.Cells[_maxRow, 4].Value = purchaseOrderNo;
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4, _maxRow + 1, _colSize + 3].Merge = true;
            workSheet.Cells[_maxRow, 4, _maxRow + 1, _colSize + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "Your Invoice Number:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 4].Value = invoice.Code;
            workSheet.Column(4).Width = 15.7;
            workSheet.Cells[_maxRow, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            _fillColor.Add(_maxRow + ";" + 4);

            workSheet.Cells[_maxRow, 5, _maxRow, _colSize].Merge = true;
            workSheet.Cells[_maxRow, 5, _maxRow, _colSize].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[_maxRow, 5, _maxRow, _colSize].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[_maxRow, 5, _maxRow, _colSize].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "Ship to:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, 2].Value = invoice?.ShipTo?.Name?.ToUpper();
            workSheet.Cells[_maxRow, 2].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 2, _maxRow, 6].Merge = true;
            workSheet.Cells[_maxRow, 2, _maxRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[_maxRow, 2, _maxRow, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[_maxRow, _colSize + 1].Value = "Port of loading:";
            workSheet.Cells[_maxRow, _colSize + 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _colSize + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            workSheet.Cells[_maxRow, _colSize + 2].Value = invoice?.PortOfLoading?.Name?.ToUpper();
            workSheet.Cells[_maxRow, _colSize + 2].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _colSize + 2, _maxRow, _colEndSize].Merge = true;
            workSheet.Cells[_maxRow, _colSize + 2, _maxRow, _colEndSize].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Cells[_maxRow, _colSize + 2, _maxRow, _colEndSize].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            workSheet.Cells[_maxRow, _colEndSize + 1].Value = "Port of Discharge: " + invoice?.PortOfDischarge?.Name?.ToUpper();
            workSheet.Cells[_maxRow, _colEndSize + 1].Style.Font.Bold = true;

            int index = 1;
            foreach (var notifyParty in invoice.NotifyParties)
            {
                _maxRow++;
                workSheet.Cells[_maxRow, 1].Value = "address - " + index;
                workSheet.Cells[_maxRow, 2].Value = notifyParty.Name?.ToUpper();

                for (var col = 2; col <= 6; col++)
                {
                    workSheet.Cells[_maxRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                for (var col = _colSize + 2; col <= _colEndSize; col++)
                {
                    workSheet.Cells[_maxRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }

                var contact = string.Empty;

                if (!string.IsNullOrEmpty(notifyParty?.ContactName))
                {
                    contact += ", ANTT: " + notifyParty?.ContactName?.ToUpper();
                }

                if (!string.IsNullOrEmpty(notifyParty?.PhoneNumber))
                {
                    contact += ", PHONE: " + notifyParty?.PhoneNumber;
                }

                if (!string.IsNullOrEmpty(notifyParty?.Fax))
                {
                    contact += ", FAX: " + notifyParty?.Fax;
                }

                _maxRow++;
                workSheet.Cells[_maxRow, 2].Value = notifyParty?.Address?.ToUpper()
                                            + ", " + notifyParty?.Country?.Code?.ToUpper()
                                            + contact;

                for (var col = 2; col <= 6; col++)
                {
                    workSheet.Cells[_maxRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                for (var col = _colSize + 2; col <= _colEndSize; col++)
                {
                    workSheet.Cells[_maxRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }
                index++;
            }

            _maxRow++;
            for (var col = 2; col <= 6; col++)
            {
                workSheet.Cells[_maxRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            for (var col = _colSize + 2; col <= _colEndSize; col++)
            {
                workSheet.Cells[_maxRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            }

            workSheet.Cells[_maxRow, _colEndSize + 1].Value = "any change in critical dimensions";
            workSheet.Cells[_maxRow, _colEndSize + 1].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "City:";
            for (var col = 2; col <= 6; col++)
            {
                workSheet.Cells[_maxRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            for (var col = _colSize + 2; col <= _colEndSize; col++)
            {
                workSheet.Cells[_maxRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            }

            workSheet.Cells[_maxRow, _colEndSize + 1].Value = "requires separate sku line entry";
            workSheet.Cells[_maxRow, _colEndSize + 1].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "State & Zip Code:";

            workSheet.Cells[_maxRow, _colEndSize + 1].Value = "Case Critical Dimensions";
            workSheet.Cells[_maxRow, _colEndSize + 1].Style.Font.Bold = true;

            _maxRow++;
        }
        private static void CreateHeaderDataSTDPL(ExcelWorksheet workSheet, Invoice invoice)
        {
            var rowBorder = _maxRow;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

            //foreach(var packingList in invoice.PackingList)
            //{
            //    if (packingList.PackingLines
            //        .FirstOrDefault().PrePack.Contains("Assorted Size"))
            //    {
            //        _packingType = "ASST";
            //        break;
            //    }
            //}


            workSheet.Cells[_maxRow, 1].Value = "This information same as on case labels";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 4].Merge = true;

            workSheet.Cells[_maxRow, 6].Value = "Total pieces shipped by size using appropriate scale";
            workSheet.Cells[_maxRow, 6].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 6, _maxRow, _colEndSize].Merge = true;

            workSheet.Cells[_maxRow, _colEndSize + 1].Value = "(example 16.25 in)";
            workSheet.Cells[_maxRow, _colEndSize + 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _colEndSize + 1, _maxRow, _colEndSize + 4].Merge = true;

            workSheet.Cells[_maxRow, _colEndSize + 5, _maxRow, _colEndSize + 7].Merge = true;

            _maxRow++;
            int column = 1;
            var title = "";

            workSheet.Cells[_maxRow, column].Value = STYLE.Replace('@', '\n');
            //workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 6, column].Merge = true;
            _dicPositionColumnPackingLine.Add(STYLE, column);

            column++;
            title = "Garan @Color @Code";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            //workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 6, column].Merge = true;
            _dicPositionColumnPackingLine.Add(COLOR_NO, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = PACK.Replace('@', '\n');
            //workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 6, column].Merge = true;
            _dicPositionColumnPackingLine.Add(PACK, column);

            column++;
            title = "Garan @Color @Name @(abbreviate as @on label)";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            //workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column, _maxRow + 6, column].Merge = true;
            _dicPositionColumnPackingLine.Add(COLOR, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = UNIT.Replace('@', '\n');
            workSheet.Cells[_maxRow, column, _maxRow + 6, column].Merge = true;
            _dicPositionColumnPackingLine.Add(UNIT, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = "ASST";
            workSheet.Column(column).Width = 12;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            _dicPositionColumnPackingLine.Add(ASSORTED, column);

            column = _colEndSize + 1;
            title = "outside @lenghth of @ONE cs. @(specify @in inches)";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells[_maxRow, column, _maxRow + 5, column].Merge = true;
            _dicPositionColumnPackingLine.Add(LENGTH, column);

            column++;
            title = "outside @width of @ONE cs. @(specify @in inches)";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells[_maxRow, column, _maxRow + 5, column].Merge = true;
            _dicPositionColumnPackingLine.Add(WIDTH, column);

            column++;
            title = "outside @height of @ONE cs. @(specify @in inches)";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells[_maxRow, column, _maxRow + 5, column].Merge = true;
            _dicPositionColumnPackingLine.Add(HEIGHT, column);

            column++;
            title = "gross @weight of @ONE cs. @(specify @in kilo's)";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells[_maxRow, column, _maxRow + 5, column].Merge = true;
            _dicPositionColumnPackingLine.Add(GW_CARTON, column);

            column++;
            title = "Carton Case @Number @(list #'s @by range)";
            workSheet.Cells[_maxRow, column].Value = title.Replace('@', '\n');
            workSheet.Cells[_maxRow, column].Style.Font.SetFromFont(new Font("Times New Roman", 11));
            workSheet.Cells[_maxRow, column, _maxRow + 5, column + 2].Merge = true;

            _maxRow += 6;
            column = _colSize;
            var sizeList = invoice.PackingList.FirstOrDefault()
                .ItemStyles.FirstOrDefault().OrderDetails.OrderBy(x => x.SizeSortIndex).ToList();

            for (int i = 0; i < _countSize; i++)
            {
                string size = sizeList[i].Size;
                if (!string.IsNullOrEmpty(size))
                {
                    workSheet.Cells[_maxRow, column].Value = size;
                    workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                    workSheet.Cells[_maxRow, column].Style.WrapText = true;
                    _dicPositionColumnPackingLine.Add(size, column);
                    column++;
                }
            }
            workSheet.Row(_maxRow).Height = 37.2;

            column = _colEndSize + 5;
            _dicPositionColumnPackingLine.Add(FROM_NO, column);
            workSheet.Column(column + 1).Width = 2.5;
            _dicPositionColumnPackingLine.Add(TO_NO, column + 2);
            workSheet.Cells[_maxRow, column, _maxRow, column + 2].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "LIST SIZE SCALE";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, column + 2].Merge = true;

            /// BORDER
            string range = "A" + rowBorder + ":"
                + _dicAlphabel[column + 2] + _maxRow;
            using (var borderData = workSheet.Cells[range])
            {
                borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                borderData.Style.WrapText = true;
            }

            _maxRow++;
        }
        private static void FillDataSTDPL(ExcelWorksheet workSheet, Invoice invoice)
        {
            var rowBorder = _maxRow;
            _maxRow++;

            foreach (var packinglist in invoice.PackingList)
            {
                var prePack = packinglist.PackingLines.FirstOrDefault().PrePack;
                var customerStyle = "";
                var styleColorName = "";
                var styleColorCode = "";
                decimal netWeight = 0;
                decimal grossWeight = 0;
                decimal quantityPack = 0;
                var rowPL = _maxRow;

                var sortPackingLines = packinglist.PackingLines.OrderBy(x => x.SequenceNo)
                                            .OrderByDescending(x => x.TotalCarton).ToList();
                var styleCount = packinglist.ItemStyles.Count;

                var sizePacking = sortPackingLines?.FirstOrDefault()?.Size;

                var unit = packinglist?.ItemStyles?.FirstOrDefault()?.Unit?.FullName ?? "PIECES";

                foreach (var data in packinglist.ItemStyles)
                {
                    if (string.IsNullOrEmpty(customerStyle.Trim()))
                        customerStyle = data.CustomerStyle.Trim();
                    else if (!customerStyle.Contains(data.CustomerStyle))
                        customerStyle += " @/ " + data.CustomerStyle.Trim();

                    if (string.IsNullOrEmpty(styleColorName.Trim()))
                        styleColorName = data.ColorName.Trim();
                    else
                        styleColorName += " @/ " + data.ColorName.Trim();

                    if (string.IsNullOrEmpty(styleColorCode.Trim()))
                        styleColorCode = data.ColorCode.Trim();
                    else
                        styleColorCode += " @/ " + data.ColorCode.Trim();

                    /// Calculation net weight for assorted
                    netWeight += (decimal)(sortPackingLines
                        .Where(x => x.LSStyle == data.LSStyle && x.Size == sizePacking)?.FirstOrDefault()?.NetWeight ?? 0);

                    quantityPack = (decimal)sortPackingLines.Sum(x => x.Quantity);
                }

                if (prePack == "Assorted Size - Solid Color")
                {
                    var summaryCarton = (int)sortPackingLines[0].TotalCarton;
                    var lsStyle = sortPackingLines[0].LSStyle;
                    var firstRow = true;
                    foreach (var packingLine in sortPackingLines)
                    {
                        var itemStyle = packinglist.ItemStyles.FirstOrDefault(x => x.LSStyle == packingLine.LSStyle);

                        if (packingLine.TotalCarton != summaryCarton)
                        {
                            firstRow = true;
                            lsStyle = packingLine.LSStyle;
                            _maxRow++;
                            workSheet.Row(_maxRow).Height = 21.6;
                        }
                        if (firstRow || packingLine.LSStyle == lsStyle)
                        {
                            if (firstRow)
                            {
                                grossWeight = (decimal)(netWeight + packingLine.GrossWeight - packingLine.NetWeight);

                                workSheet.Row(_maxRow).Height = 21.6;

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE]].Value = customerStyle.Replace('@', '\n');
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PACK]].Value = quantityPack;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_NO]].Value = itemStyle.ColorCode;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNIT]].Value = unit;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[ASSORTED]].Value = "ASSORTED";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.QuantitySize * packingLine.TotalCarton;
                                if (packingLine.BoxDimensionCode.Contains("*"))
                                {
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.BoxDimensionCode.Split("*")[0].Trim();
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.BoxDimensionCode.Split("*")[1].Trim();
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.BoxDimensionCode.Split("*")[2].Trim();
                                }
                                else
                                {
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]]
                                            .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[0].Trim();
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]]
                                            .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[1].Trim();
                                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                            .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[2].Trim();
                                }
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Value = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);

                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;

                                firstRow = false;
                                summaryCarton = (int)packingLine.TotalCarton;
                                lsStyle = packingLine.LSStyle;
                            }
                            else
                            {
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNIT]].Value = unit;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.QuantitySize * packingLine.TotalCarton;

                                summaryCarton = (int)packingLine.TotalCarton;
                                lsStyle = packingLine.LSStyle;
                            }
                        }
                        else
                        {
                            _maxRow++;
                            workSheet.Row(_maxRow).Height = 21.6;

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_NO]].Value = itemStyle.ColorCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNIT]].Value = unit;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.QuantitySize * packingLine.TotalCarton;

                            summaryCarton = (int)packingLine.TotalCarton;
                            lsStyle = packingLine.LSStyle;
                        }
                    }

                    //// Merge Cell 
                    workSheet.Cells["A" + rowPL + ":" + "A" + (_maxRow + 1)].Merge = true;

                    workSheet.Cells["C" + rowPL + ":" + "C" + _maxRow].Merge = true;

                    workSheet.Cells["F" + rowPL + ":" + "F" + (_maxRow + 1)].Merge = true;

                    int lastColumn = _dicPositionColumnPackingLine[TO_NO];
                    for (var column = _colEndSize + 1; column <= lastColumn; column++)
                    {
                        workSheet.Cells[rowPL, column, _maxRow, column].Merge = true;
                    }

                    _maxRow += 2;
                }
                else
                {
                    var firstRow = true;
                    var itemStyle = packinglist.ItemStyles.OrderBy(x => x.LSStyle).FirstOrDefault();

                    sortPackingLines = packinglist.PackingLines
                            .Where(x => x.LSStyle == itemStyle.LSStyle).OrderBy(x => x.SequenceNo).ToList();

                    foreach (var packingLine in sortPackingLines)
                    {
                        netWeight = (decimal)(packinglist.PackingLines.Where(x => x.Size == packingLine.Size).Sum(x => x.NetWeight));
                        grossWeight = (decimal)(netWeight + (packingLine.BoxDimension.Weight * packingLine.TotalCarton));

                        if (firstRow)
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = styleColorName.Replace('@', '\n');
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_NO]].Value = styleColorCode.Replace('@', '\n');
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE]].Value = customerStyle.Replace('@', '\n');
                        }
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[PACK]].Value = packingLine.QuantityPerCarton * styleCount;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNIT]].Value = unit;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity * packingLine.TotalCarton * styleCount;
                        if (packingLine.BoxDimensionCode.Contains("*"))
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.BoxDimensionCode.Split("*")[0].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.BoxDimensionCode.Split("*")[1].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.BoxDimensionCode.Split("*")[2].Trim();
                        }
                        else
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[0].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[1].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[2].Trim();
                        }
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GW_CARTON]].Value
                                                = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;

                        firstRow = false;
                        workSheet.Row(_maxRow).Height = 21.6;
                        _maxRow++;
                    }

                    //// Merge Cell 
                    workSheet.Cells["A" + rowPL + ":" + "A" + _maxRow].Merge = true;

                    workSheet.Cells["B" + rowPL + ":" + "B" + _maxRow].Merge = true;

                    workSheet.Cells["D" + rowPL + ":" + "D" + _maxRow].Merge = true;

                    workSheet.Cells["F" + rowPL + ":" + "F" + _maxRow].Merge = true;

                    _maxRow += 1;
                }
            }

            /// BORDER SECOND TOTAL
            string range = "A" + rowBorder + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[TO_NO]] + (_maxRow - 1);

            using (var borderData = workSheet.Cells[range])
            {
                borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                borderData.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                borderData.Style.Font.Bold = true;
                borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                borderData.Style.WrapText = true;
            }

            _maxRow++;
            workSheet.Cells[_maxRow, _colEndSize - 1].Value = "Assembly & Inspection Location Name & Address:";
            workSheet.Cells[_maxRow, _colEndSize - 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _colEndSize - 1].Style.Font.UnderLine = true;

            _maxRow++;
            workSheet.Cells[_maxRow, _colEndSize - 1].Value = invoice.Company.Name?.ToUpper();
            workSheet.Cells[_maxRow, _colEndSize - 1].Style.Font.SetFromFont(new Font("Times New Roman", 11));

            _maxRow++;
            workSheet.Cells[_maxRow, _colEndSize - 1].Value = invoice.Company.DisplayAddress.Substring(0, 57);
            workSheet.Cells[_maxRow, _colEndSize - 1].Style.Font.SetFromFont(new Font("Times New Roman", 11));

            _maxRow++;
            workSheet.Cells[_maxRow, _colEndSize - 1].Value = invoice.Company.DisplayAddress.Substring(59);
            workSheet.Cells[_maxRow, _colEndSize - 1].Style.Font.SetFromFont(new Font("Times New Roman", 11));
        }

        #endregion  STD PACKING LIST

        #region General Function
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

        private static void ResetData()
        {
            _colSize = 0;
            _countSize = 0;
            _totalCarton = 0;
            _totalUnits = 0;
            _totalGW = 0;
            _totalNW = 0;
            _totalMeanM3 = 0;
            _dicAlphabel = new Dictionary<int, string>();
            _packingType = string.Empty;
        }
        #endregion
    }
}
