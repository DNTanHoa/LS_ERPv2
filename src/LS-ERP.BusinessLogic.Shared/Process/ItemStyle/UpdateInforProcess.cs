using LS_ERP.Ultilities.Extensions;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class UpdateInforProcess
    {
        public static (bool, string, List<dynamic>) Import(string filePath)
        {
            if (File.Exists(filePath) &&
                 Path.GetExtension(filePath).Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets.First();
                        var dataTable = workSheet.ToDataTable();

                        var data = new List<dynamic>();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (string.IsNullOrEmpty(row["Style"]?.ToString()))
                                break;

                            var rowData = new
                            {
                                LSStyle = row["Style"]?.ToString(),
                                IssuedDate = DateTime.Parse(row["Issued Date"]?.ToString()),
                                ProductionSketDeliveryDate = DateTime.Parse(row["PSDD"]?.ToString()),
                                AccessoriesDate = DateTime.Parse(row["Accessories Date"]?.ToString()),
                                FabricDate = DateTime.Parse(row["Fabric Date"]?.ToString()),
                                Remark = row["Remark"]?.ToString(),
                            };

                            data.Add(rowData);
                        }

                        return (true, string.Empty, data);
                    }

                    return (false, "No data file", null);
                }
            }
            else
            {
                return (false, "Invalid file", null);
            }
        }
    }
}
