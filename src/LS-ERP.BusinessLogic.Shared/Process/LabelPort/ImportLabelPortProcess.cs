using LS_ERP.EntityFrameworkCore.Entities;
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
    public static class ImportLabelPortProcess
    {
        public static void Import(string filePath, string userName,
            string customerID,
            out List<LabelPort> newLabelPorts,
            out Dictionary<string, LabelPort> dicLabelPort,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            newLabelPorts = new List<LabelPort>();
            dicLabelPort = new Dictionary<string, LabelPort>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets.First();
                        var dataTable = workSheet.ToDataTable(1, 1, 4, 1);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            string division = row["Division"]?.ToString();

                            if (!String.IsNullOrEmpty(division))
                            {
                                var labelPort = new LabelPort()
                                {
                                    Division = row["Division"]?.ToString(),
                                    LabelCode = row["Label"]?.ToString(),
                                    LabelName = row["Label Name"]?.ToString(),
                                    ETAPort = row["ETA PORT"]?.ToString(),
                                    CustomerID = customerID,

                                };

                                labelPort.SetCreateAudit(userName);
                                newLabelPorts.Add(labelPort);

                                string key = labelPort.Division + labelPort.LabelCode;
                                dicLabelPort[key] = labelPort;
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Not found sheet";
                    }
                }
            }
        }
    }
}
