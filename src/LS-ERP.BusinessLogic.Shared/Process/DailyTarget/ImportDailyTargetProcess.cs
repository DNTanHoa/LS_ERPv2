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
    public class ImportDailyTargetProcess
    {
        public static bool Import(string filePath, string Username, string Filename,
            out List<DailyTarget> dailyTargets,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            dailyTargets = new List<DailyTarget>();

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
                        try
                        {
                            var dataTable = workSheet.ToDataTable();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var dailytarget = new DailyTarget()
                                {
                                    CustomerName = row["Tên khách hàng"]?.ToString(),
                                    StyleNO = row["Mã hàng"]?.ToString(),
                                    Item = row["Item"]?.ToString(),
                                    SMV = decimal.Parse(row["SMV"]?.ToString()),
                                    InlineDate = DateTime.Parse(row["Ngày lên chuyền"]?.ToString()),
                                    ProduceDate = DateTime.Parse(row["Ngày sản xuất"]?.ToString()),
                                    WorkCenterName = row["Chuyền"]?.ToString(),
                                    TotalTargetQuantity = decimal.Parse(row["Mục tiêu"]?.ToString()),
                                    NumberOfWorker = int.Parse(row["Số lượng người"]?.ToString()),
                                    Operation = "SEWING",
                                };

                                if (!string.IsNullOrEmpty(row["SMV"]?.ToString()))
                                {
                                    var smvParseResult = decimal.TryParse(row["SMV"]?.ToString(), out decimal smv);

                                    if (!smvParseResult)
                                    {
                                        errorMessage = "Invalid lead time input for ex code " + dailytarget.WorkCenterName;
                                        return false;
                                    }

                                    dailytarget.SMV = smv;
                                }
                                else
                                {
                                    dailytarget.SMV = 0;
                                }
                                if (!string.IsNullOrEmpty(row["Ngày lên chuyền"]?.ToString()))
                                {
                                    var inlineDateParseResult = DateTime.TryParse(row["Ngày lên chuyền"]?.ToString(), out DateTime inlineDate);

                                    if (!inlineDateParseResult)
                                    {
                                        errorMessage = "Invalid lead time input for ex code " + dailytarget.WorkCenterName;
                                        return false;
                                    }

                                    dailytarget.InlineDate = inlineDate;
                                }
                                else
                                {
                                    dailytarget.InlineDate = DateTime.Now;
                                }
                                if (!string.IsNullOrEmpty(row["Ngày sản xuất"]?.ToString()))
                                {
                                    var produceDateParseResult = DateTime.TryParse(row["Ngày sản xuất"]?.ToString(), out DateTime produceDate);

                                    if (!produceDateParseResult)
                                    {
                                        errorMessage = "Invalid lead time input for ex code " + dailytarget.WorkCenterName;
                                        return false;
                                    }

                                    dailytarget.ProduceDate = produceDate;
                                }
                                else
                                {
                                    dailytarget.ProduceDate = DateTime.Now;
                                }
                                if (!string.IsNullOrEmpty(row["Mục tiêu"]?.ToString()))
                                {
                                    var totalTargetQuantityParseResult = int.TryParse(row["Mục tiêu"]?.ToString(), out int totalTargetQuantity);

                                    if (!totalTargetQuantityParseResult)
                                    {
                                        errorMessage = "Invalid lead time input for ex code " + dailytarget.WorkCenterName;
                                        return false;
                                    }

                                    dailytarget.TotalTargetQuantity = totalTargetQuantity;
                                }
                                else
                                {
                                    dailytarget.TotalTargetQuantity = 0;
                                }

                                if (!string.IsNullOrEmpty(row["Số lượng người"]?.ToString()))
                                {
                                    var numberOfWorkerParseResult = int.TryParse(row["Số lượng người"]?.ToString(), out int numberOfWorker);

                                    if (!numberOfWorkerParseResult)
                                    {
                                        errorMessage = "Invalid lead time input for ex code " + dailytarget.WorkCenterName;
                                        return false;
                                    }

                                    dailytarget.NumberOfWorker = numberOfWorker;
                                }
                                else
                                {
                                    dailytarget.NumberOfWorker = 0;
                                }
                                dailytarget.SetCreateAudit(Username);
                                dailyTargets.Add(dailytarget);
                            }                          
                        }
                        catch (Exception ex)
                        {
                            errorMessage = ex.Message;
                        }
                    }
                }
            }
            else
            {
                errorMessage = "Invalid file";
            }

            return true;
        }
    }
}
