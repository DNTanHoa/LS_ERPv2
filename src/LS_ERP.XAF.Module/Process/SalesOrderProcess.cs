using LS_ERP.EntityFrameworkCore.Entities;
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
using LS_ERP.Ultilities.Helpers;

namespace LS_ERP.XAF.Module.Process
{
    /// <summary>
    /// Process relate to sale order
    /// </summary>
    public class SalesOrderProcess
    {
        private static Dictionary<string, int> _dicPositionSize;
        private static int _maxColumn;

        public static Stream CreateExcelFile(SalesOrder salesOrder, List<ItemStyle> itemStyles = null, Stream stream = null)
        {
            _maxColumn = 0;
            _dicPositionSize = new Dictionary<string, int>();
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

                CreateHeader(workSheet, Title, salesOrder.CustomerID, itemStyles);


                DataTable table = SetData(salesOrder, itemStyles);

                switch (salesOrder.CustomerID)
                {
                    case "DE":
                        {
                            workSheet.Cells[3, 1].LoadFromDataTable(table);
                            FormatStyleFor_DE(workSheet, table);
                        }
                        break;
                    case "HA":
                        {
                            excelPackage.Workbook.Properties.Title = "SO.HADDAD."
                                                                                + itemStyles.FirstOrDefault().ShipDate?.ToString("MM") + "."
                                                                                + itemStyles.FirstOrDefault().ShipDate?.ToString("yyyy")
                                                                        ;
                            workSheet.Cells[4, 1].LoadFromDataTable(table);
                            FormatStyleFor_HA(workSheet, table);
                        }
                        break;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public static void CreateHeader(ExcelWorksheet workSheet, string Title, string CustomerID, List<ItemStyle> itemStyles = null)
        {
            switch (CustomerID)
            {
                case "DE":
                    CreateHeaderDE(workSheet, Title);
                    break;
                case "HA":
                    CreateHeaderHA(workSheet, itemStyles);
                    break;
                case "PU":
                    CreateHeaderPU(workSheet, Title);
                    break;
                default:
                    break;
            }
        }

        public static void CreateHeaderPU(ExcelWorksheet workSheet, string Title)
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

        public static void CreateHeaderHA(ExcelWorksheet workSheet, List<ItemStyle> itemStyles)
        {
            //workSheet.Cells[1, 1].Value = "";
            //workSheet.Cells[1, 1].Style.Font.Bold = true;

            workSheet.Cells[2, 1].Value = "HADDAD_PRODUCTION SCHEDULE";
            workSheet.Cells[2, 1].Style.Font.Bold = true;
            workSheet.Cells[2, 1].Style.Font.Size = 18;
            workSheet.Cells[2, 1, 2, 10].Merge = true;
            workSheet.Cells[2, 1, 2, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[2, 1, 2, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[2, 1, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            workSheet.Cells[3, 1].Value = "Division";
            workSheet.Cells[3, 2].Value = "Season";
            workSheet.Cells[3, 3].Value = "Year";
            workSheet.Cells[3, 4].Value = "Ref#";
            workSheet.Cells[3, 5].Value = "STYLE";
            workSheet.Cells[3, 6].Value = "Color";
            workSheet.Cells[3, 7].Value = "Label";
            workSheet.Cells[3, 8].Value = "Fashion Color";
            workSheet.Cells[3, 9].Value = "LSStyle";
            workSheet.Cells[3, 10].Value = "PO#";
            workSheet.Cells[3, 11].Value = "Label Name";
            workSheet.Cells[3, 12].Value = "Ship Date";
            workSheet.Cells[3, 13].Value = "PO Qty";
            workSheet.Cells[3, 14].Value = "UNIT";
            workSheet.Cells[3, 15].Value = "MSRP($)";
            workSheet.Cells[3, 16].Value = "FABRIC ART";
            workSheet.Cells[3, 17].Value = "Product Description";
            workSheet.Cells[3, 18].Value = "Size Configuration";
            workSheet.Cells[3, 19].Value = "Pack Ratio";
            workSheet.Cells[3, 20].Value = "Master Box Quantity";
            workSheet.Cells[3, 21].Value = "Hang/Flat";
            workSheet.Cells[3, 22].Value = "ETA PORT";

            int position = _maxColumn = 23;

            workSheet.Cells[2, 23].Value = "SIZE BREAKDOWN";
            workSheet.Cells[2, 23].Style.Font.Bold = true;

            var dicSizeIndex = new Dictionary<int, string>();

            foreach (var itemStyle in itemStyles)
            {
                foreach (var size in itemStyle.OrderDetails)
                {
                    if (!dicSizeIndex.ContainsKey((int)size.SizeSortIndex))
                    {
                        dicSizeIndex[(int)size.SizeSortIndex] = size.Size;
                    }
                }
            }

            var listSortSize = dicSizeIndex.OrderBy(x => x.Key);

            if (_dicPositionSize == null)
            {
                _dicPositionSize = new Dictionary<string, int>();
            }

            foreach (var item in listSortSize)
            {
                workSheet.Cells[3, position].Value = item.Value;

                if (!_dicPositionSize.ContainsKey(item.Value))
                {
                    _dicPositionSize[item.Value] = position;
                    position++;
                }
            }
            _maxColumn = position - 1;
            workSheet.Cells[2, 23, 2, _maxColumn].Merge = true;

            workSheet.Cells[2, position].Value = "SIMPLE SIZE";
            workSheet.Cells[2, position].Style.Font.Bold = true;

            var dicSimpleSizeIndex = new Dictionary<int, string>();

            foreach (var itemStyle in itemStyles)
            {
                foreach (var size in itemStyle.OrderDetails)
                {
                    if (!dicSimpleSizeIndex.ContainsKey((int)size.SizeSortIndex))
                    {
                        dicSimpleSizeIndex[(int)size.SizeSortIndex] = size.Size;
                    }
                }
            }

            var listSortSimpleSize = dicSimpleSizeIndex.OrderBy(x => x.Key);

            if (_dicPositionSize == null)
            {
                _dicPositionSize = new Dictionary<string, int>();
            }

            foreach (var item in listSortSimpleSize)
            {
                string value = "S_" + item.Value;
                workSheet.Cells[3, position].Value = value;

                if (!_dicPositionSize.ContainsKey(value))
                {
                    _dicPositionSize[value] = position;
                    position++;
                }
            }

            workSheet.Cells[2, _maxColumn + 1, 2, position - 1].Merge = true;

            _maxColumn = position - 1;

       
            workSheet.Cells[2, 23, 2, _maxColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[2, 23, 2, _maxColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 23, 2, _maxColumn].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[2, 23, 2, _maxColumn].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[2, 23, 2, _maxColumn].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[2, 23, 2, _maxColumn].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            workSheet.Cells[3, 23, 3, _maxColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[3, 23, 3, _maxColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        public static void CreateHeaderDE(ExcelWorksheet workSheet, string Title)
        {
            workSheet.Cells[1, 1].Value = "Order No: ";
            workSheet.Cells[1, 1].Style.Font.Bold = true;

            workSheet.Cells[1, 2].Value = Title;
            workSheet.Cells[1, 2].Style.Font.Bold = true;

            workSheet.Cells[2, 1].Value = "LS Style";
            workSheet.Cells[2, 2].Value = "Order Creation Date";
            workSheet.Cells[2, 3].Value = "Order";
            workSheet.Cells[2, 4].Value = "Production Line";
            workSheet.Cells[2, 5].Value = "Contractual Supplier Handover";
            workSheet.Cells[2, 6].Value = "Order Type";
            workSheet.Cells[2, 7].Value = "Model";
            workSheet.Cells[2, 8].Value = "Designation";
            workSheet.Cells[2, 9].Value = "Item";
            workSheet.Cells[2, 10].Value = "Size";
            workSheet.Cells[2, 11].Value = "PCB";
            workSheet.Cells[2, 12].Value = "UE";
            workSheet.Cells[2, 13].Value = "Packaging";
            workSheet.Cells[2, 14].Value = "IMAN Code";
            workSheet.Cells[2, 15].Value = "Ordered Qty";
            workSheet.Cells[2, 16].Value = "Unit Price";
            workSheet.Cells[2, 17].Value = "Delivery Place";
            workSheet.Cells[2, 18].Value = "Order Status";
            workSheet.Cells[2, 19].Value = "Contractual Delivery Date";
            workSheet.Cells[2, 20].Value = "Estimated Supplier Handover";

            workSheet.Cells[2, 3, 10000, 3].Style.Numberformat.Format = "0";
            workSheet.Cells[2, 7, 10000, 7].Style.Numberformat.Format = "0";
            workSheet.Cells[2, 14, 10000, 14].Style.Numberformat.Format = "0";
            workSheet.Cells[2, 15, 10000, 15].Style.Numberformat.Format = "0";
        }
        public static void FormatStyleForExportSaleOrder(ExcelWorksheet workSheet, int rows)
        {
            int row = 2 + rows;

            string modelRangeOrderDate = "C3:C" + row.ToString();
            using (var range = workSheet.Cells[modelRangeOrderDate])
            {
                range.Style.Numberformat.Format = "dd-MMM-yyyy";
            }

            string modelRangeCHD = "F3:F" + row.ToString();
            using (var range = workSheet.Cells[modelRangeCHD])
            {
                range.Style.Numberformat.Format = "dd-MMM-yyyy";
            }

            string modelRangeDeliveryDate = "T3:T" + row.ToString();
            using (var range = workSheet.Cells[modelRangeDeliveryDate])
            {
                range.Style.Numberformat.Format = "dd-MMM-yyyy";
            }

            string modelRangeEHD = "U3:U" + row.ToString();
            using (var range = workSheet.Cells[modelRangeEHD])
            {
                range.Style.Numberformat.Format = "dd-MMM-yyyy";
            }

            string modelRangeBorder = "A2:U" + row.ToString();
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

            //string modelRange = "A2:T2";
            using (var range = workSheet.Cells["A2:U2"])
            {
                range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

        }

        public static void FormatStyleFor_DE(ExcelWorksheet workSheet, DataTable table)
        {
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
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            string modelDate = "B3:B" + (table.Rows.Count + 2).ToString();
            using (var range = workSheet.Cells[modelDate])
            {
                range.Style.Numberformat.Format = "dd-MMM-yyyy";
            }

            modelDate = "E3:E" + (table.Rows.Count + 2).ToString();
            using (var range = workSheet.Cells[modelDate])
            {
                range.Style.Numberformat.Format = "dd-MMM-yyyy";
            }

            modelDate = "S3:T" + (table.Rows.Count + 2).ToString();
            using (var range = workSheet.Cells[modelDate])
            {
                range.Style.Numberformat.Format = "dd-MMM-yyyy";
            }

        }

        public static void FormatStyleFor_HA(ExcelWorksheet workSheet, DataTable table)
        {
            var dicAlphabel = AlphabelColumnExcelHelpers.DictionaryAlphabet(3);

            string modelRangeBorder = "A3:"; // V + (table.Rows.Count + 3).ToString();
            string columnName = "V";

            if (dicAlphabel.TryGetValue(_maxColumn, out columnName))
            {
                modelRangeBorder += columnName + (table.Rows.Count + 3).ToString();
            }

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

            using (var range = workSheet.Cells["A3:V3"])
            {
                range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                range.Style.Font.Bold = true;
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(91, 155, 213));
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            string rangeHeader = "W3:" + columnName + "3";

            using (var range = workSheet.Cells[rangeHeader])
            {
                range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                range.Style.Font.Bold = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            //string modelNumeric = "A3:T" + (table.Rows.Count + 2).ToString();
            //using (var range = workSheet.Cells[modelNumeric])
            //{
            //    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //}

            string modelDate = "L3:L" + (table.Rows.Count + 3).ToString();
            using (var range = workSheet.Cells[modelDate])
            {
                range.Style.Numberformat.Format = "MM-dd-yyyy";
            }

            string modelPOQty = "M3:M" + (table.Rows.Count + 3).ToString();
            using (var range = workSheet.Cells[modelPOQty])
            {
                range.Style.Numberformat.Format = "#,##0.0";
            }

            string modelMSRP = "O3:O" + (table.Rows.Count + 3).ToString();
            using (var range = workSheet.Cells[modelMSRP])
            {
                range.Style.Numberformat.Format = "#,##0.0";
            }
        }
        public static DataTable SetData(SalesOrder salesOrder, List<ItemStyle> itemStyles = null)
        {
            DataTable table = null;
            switch (salesOrder.CustomerID)
            {
                case "DE":
                    table = SetDataDE(salesOrder);
                    return table;
                case "HA":
                    table = SetDataHA(salesOrder, itemStyles);
                    return table;
                case "PU":
                    table = SetDataPU(salesOrder);
                    return table;
                default:
                    break;
            }

            return null;
        }
        public static DataTable SetDataPU(SalesOrder salesOrder)
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
        public static DataTable SetDataDE(SalesOrder salesOrder)
        {
            DataTable table = new DataTable();
            for (int i = 0; i <= 32; i++)
            {
                DataColumn column = new DataColumn("Column" + i);

                if (i == 14 || i == 2 || i == 6 || i == 13)
                {
                    column.DataType = typeof(long);
                }
                if (i == 1 || i == 4 || i == 18 || i == 19)
                {
                    column.DataType = typeof(DateTime);
                }

                table.Columns.Add(column);
            }

            foreach (var itemStyle in salesOrder.ItemStyles)
            {
                var orderDetails = itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex);

                foreach (var orderDetail in orderDetails)
                {
                    DataRow row = table.NewRow();
                    var barCode = itemStyle.Barcodes
                        .FirstOrDefault(x => x.Size.ToUpper().Replace(" ", "").Trim() ==
                            orderDetail.Size.ToUpper().Replace(" ", "").Trim());

                    row["Column0"] = itemStyle.LSStyle;
                    row["Column1"] = itemStyle.PurchaseOrderDate?.ToString("dd-MMM-yyyy");
                    row["Column2"] = long.Parse(itemStyle.PurchaseOrderNumber);
                    row["Column3"] = itemStyle.ProductionDescription;
                    row["Column4"] = itemStyle.ContractDate?.ToString("dd-MMM-yyyy");
                    row["Column5"] = itemStyle.PurchaseOrderType?.Name;
                    row["Column6"] = long.Parse(itemStyle.ColorCode);
                    row["Column7"] = itemStyle.Description;
                    row["Column8"] = barCode?.BarCode;
                    row["Column9"] = orderDetail.Size;
                    row["Column10"] = barCode?.PCB;
                    row["Column11"] = barCode?.UE;
                    row["Column12"] = barCode?.Packing;
                    row["Column13"] = long.Parse(itemStyle.CustomerStyle);
                    row["Column14"] = long.Parse(orderDetail.Quantity?.ToString("G29"));
                    row["Column15"] = orderDetail.Price?.ToString("G29");
                    row["Column16"] = itemStyle.DeliveryPlace;
                    row["Column17"] = salesOrder.SalesOrderStatus?.Name;
                    row["Column18"] = itemStyle.DeliveryDate?.ToString("dd-MMM-yyyy");
                    row["Column19"] = itemStyle.EstimatedSupplierHandOver?.ToString("dd-MMM-yyyy");
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        public static DataTable SetDataHA(SalesOrder salesOrder, List<ItemStyle> itemStyles)
        {
            DataTable table = new DataTable();
            for (int i = 0; i <= _maxColumn; i++)
            {
                DataColumn column = new DataColumn("Column" + i);

                if (i == 12 || i == 14 || i == 19 || i > 21)
                {
                    column.DataType = typeof(decimal);
                }
                if (i == 11)
                {
                    column.DataType = typeof(DateTime);
                }

                table.Columns.Add(column);
            }

            foreach (var itemStyle in itemStyles)
            {
                DataRow row = table.NewRow();

                row["Column0"] = itemStyle.Division;
                row["Column1"] = itemStyle.Season;
                row["Column2"] = itemStyle.Year;
                row["Column3"] = itemStyle.ContractNo;
                row["Column4"] = itemStyle.CustomerStyle;
                row["Column5"] = itemStyle.ColorCode;
                row["Column6"] = itemStyle.LabelCode;
                row["Column7"] = itemStyle.ColorName;
                row["Column8"] = itemStyle.LSStyle;
                row["Column9"] = itemStyle.PurchaseOrderNumber;
                row["Column10"] = itemStyle.LabelName;
                row["Column11"] = itemStyle.ShipDate?.ToString("MM-dd-yyyy");
                row["Column12"] = itemStyle.TotalQuantity;
                row["Column13"] = itemStyle.UnitID;
                row["Column14"] = itemStyle.MSRP;
                row["Column15"] = "";
                row["Column16"] = itemStyle.ProductionDescription;
                row["Column17"] = itemStyle.UE;
                row["Column18"] = itemStyle.Packing;
                row["Column19"] = itemStyle.PCB;
                row["Column20"] = itemStyle.HangFlat;
                row["Column21"] = itemStyle.ETAPort;


                foreach (var size in itemStyle.OrderDetails)
                {
                    if (_dicPositionSize.TryGetValue(size.Size, out int position))
                    {
                        row["Column" + (position - 1)] = size.Quantity;
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
