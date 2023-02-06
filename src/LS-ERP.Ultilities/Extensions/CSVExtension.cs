using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Extensions
{
    public class CSVExtension
    {
        public DataTable ToDataTable(string filePath, string separatorChar, string[] Columns, bool hasHeader = true)
        {
            DataTable tbl = new DataTable();

            string[] rows = File.ReadAllLines(filePath);

            string[] rowValues = null;
            DataRow dr = tbl.NewRow();

            //Creating columns
            if (rows.Length > 0 && hasHeader)
            {
                foreach (string columnName in rows[0].Split(separatorChar))
                    tbl.Columns.Add(columnName);

                for (int row = 1; row < rows.Length; row++)
                {
                    rowValues = rows[row].Split(separatorChar);
                    dr = tbl.NewRow();
                    dr.ItemArray = rowValues;
                    tbl.Rows.Add(dr);
                }
            }
            else if (rows.Length > 0 && Columns.Length > 0)
            {
                foreach (string columnName in Columns)
                    tbl.Columns.Add(columnName);

                for (int row = 0; row < rows.Length; row++)
                {
                    rowValues = rows[row].Split(separatorChar);
                    dr = tbl.NewRow();
                    dr.ItemArray = rowValues;
                    tbl.Rows.Add(dr);
                }
            }

            return tbl;
        }
    }
}
