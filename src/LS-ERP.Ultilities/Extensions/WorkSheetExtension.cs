using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Extensions
{
    public static class WorksheetExtension
    {
        public static DataTable ToDataTable(this ExcelWorksheet worksheet, bool hasHeader = true)
        {
            DataTable tbl = new DataTable();
            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
            {
                tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
            }
            var startRow = hasHeader ? 2 : 1;
            for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var wsRow = worksheet.Cells[rowNum, 1, rowNum, tbl.Columns.Count];
                DataRow row = tbl.Rows.Add();
                foreach (var cell in wsRow)
                {
                    try
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        continue;
                    }
                }
            }
            return tbl;
        }

        public static DataTable ToDataTable(this ExcelWorksheet worksheet, int startHeader, bool hasHeader = true)
        {
            DataTable tbl = new DataTable();
            foreach (var firstRowCell in worksheet.Cells[startHeader, 1, 1, worksheet.Dimension.End.Column])
            {
                tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
            }
            var startRow = hasHeader ? startHeader + 1 : 1;
            for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var wsRow = worksheet.Cells[rowNum, 1, rowNum, tbl.Columns.Count];
                DataRow row = tbl.Rows.Add();
                foreach (var cell in wsRow)
                {
                    try
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        continue;
                    }
                }
            }
            return tbl;
        }

        public static DataTable ToDataTable(this ExcelWorksheet worksheet, int startHeader, 
            bool isHeaderMerge, int numberRowInMerge,
            bool hasHeader = true)
        {
            DataTable tbl = new DataTable();
            foreach (var firstRowCell in worksheet.Cells[startHeader, 1, startHeader + 1, worksheet.Dimension.End.Column])
            {
                try
                {
                    if (!isHeaderMerge)
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text :
                            string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    else
                    {
                        var headerText = firstRowCell.Text;
                        if (string.IsNullOrEmpty(headerText))
                        {
                            if(startHeader > 1)
                            {
                                var headerCell = worksheet.Cells[startHeader - numberRowInMerge, firstRowCell.Start.Column];
                                headerText = headerCell.Text;
                            }

                            tbl.Columns.Add(string.IsNullOrEmpty(headerText) ?
                                string.Format("Column {0}", firstRowCell.Start.Column) : headerText);
                        }
                        else
                        {
                            tbl.Columns.Add(headerText);
                        }
                    }
                }
                catch(Exception ex)
                {
                    break;
                }
            }
            var startRow = hasHeader ? startHeader + 1 : 1;
            for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var wsRow = worksheet.Cells[rowNum, 1, rowNum, tbl.Columns.Count];
                DataRow row = tbl.Rows.Add();
                foreach (var cell in wsRow)
                {
                    try
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        continue;
                    }
                }
            }
            return tbl;
        }

        public static DataTable ToDataTable(this ExcelWorksheet worksheet, int startHeader,
            int startColumn, int endColumn, int startRow = 2, bool hasHeader = true)
        {
            DataTable tbl = new DataTable();
            foreach (var firstRowCell in worksheet.Cells[startHeader, startColumn, 
                1, endColumn])
            {
                tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
            }

            startRow = hasHeader ? startRow + 1 : startRow;

            for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var wsRow = worksheet.Cells[rowNum, startColumn, rowNum, endColumn];
                DataRow row = tbl.Rows.Add();
                foreach (var cell in wsRow)
                {
                    try
                    {
                        row[cell.Start.Column - startColumn] = cell.Text;
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        continue;
                    }
                }
            }
            return tbl;
        }

        public static string GetMergedRangeAddress(this ExcelRange @this)
        {
            if (@this.Merge)
            {
                var idx = @this.Worksheet.GetMergeCellId(@this.Start.Row, @this.Start.Column);
                return @this.Worksheet.MergedCells[idx - 1];
            }
            else
            {
                return @this.Address;
            }
        }
    }
}
