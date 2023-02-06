using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Extensions
{
    public static class DataTableExtensions
    {
        public static T AsObject<T>(this DataRow row)
        {
            var result = Activator.CreateInstance<T>();

            return result;
        }

        public static bool ContainComlumnWithName(this DataTable table, string columnName)
        {
            DataColumnCollection columns = table.Columns;
            if (columns.Contains(columnName))
            {
                return true;
            }
            return false;
        }
    }
}
