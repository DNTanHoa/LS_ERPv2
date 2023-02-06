using LS_ERP.Ultilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Extensions
{
    public static class DataTableExtension
    {
        public static T? AsObject<T>(this DataRow row)
        {
            try
            {
                Type temp = typeof(T);
                T obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (pro.Name == column.ColumnName && row[column.ColumnName] != DBNull.Value)
                        {
                            if (pro.PropertyType == typeof(DateTime) || pro.PropertyType == typeof(DateTime?))
                            {
                                if (!DateTime.TryParse(row[column.ColumnName]?.ToString(), out DateTime time))
                                {
                                    if (!DateTime.TryParseExact(row[column.ColumnName]?.ToString(),
                                        "dd/MM/yyyy", null, DateTimeStyles.None, out time))
                                    {
                                        pro.SetValue(obj, time, null);
                                    }
                                    else
                                    {
                                        pro.SetValue(obj, time, null);
                                    }
                                }
                                else
                                {
                                    pro.SetValue(obj, time, null);
                                }
                                continue;
                            }

                            pro.SetValue(obj, row[column.ColumnName].ChangeType(pro.PropertyType));
                        }
                    }
                }
                return obj;
            }
            catch(Exception ex)
            {
                return default(T);
            }

        }
        public static bool ContainComlumnWithName(this DataTable table, string columnName)
        {
            DataColumnCollection columns = table.Columns;
            return columns.Contains(columnName);
        }
        public static List<T?>? AsListObject<T>(this DataTable table)
        {
            if (table == null)
                return null;

            var results = new List<T?>();

            foreach (DataRow dataRow in table.Rows)
            {
                var obj = dataRow.AsObject<T>();

                results.Add(obj);
            }

            return results;
        }
    }
}
