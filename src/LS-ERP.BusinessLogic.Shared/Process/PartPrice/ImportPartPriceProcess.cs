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
    public class ImportPartPriceProcess
    {
        public static List<PartPrice> Import(string filePath, string CustomerID, string UserName, out string errorMessage)
        {
            errorMessage = string.Empty;
            var result = new List<PartPrice>();

            if (File.Exists(filePath) &&
                Path.GetExtension(filePath).Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets[0];

                        var dataTable = workSheet.ToDataTable();

                        //var dataTable = workSheet.ToDataTable(5);

                        decimal price = 0;
                        var effDate = new DateTime();
                        var expDate = new DateTime();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (!DateTime.TryParse(row["EffectiveDate"].ToString(), out effDate))
                            {
                                effDate = DateTime.Now;
                            }
                            if (!DateTime.TryParse(row["ExpiryDate"].ToString(), out expDate))
                            {
                                expDate = DateTime.MaxValue;
                            }

                            if (decimal.TryParse(row["CM"].ToString(), out price))
                            {
                                var partPrice = new PartPrice()
                                {
                                    StyleNO = row["STYLE"].ToString(),
                                    GarmentColorCode = row["MODEL"].ToString(),
                                    Season = row["SEASON"].ToString(),
                                    EffectiveDate = effDate,
                                    ExpiryDate = expDate,
                                    Price = price,
                                    ProductionType = "CM",
                                    CustomerID = CustomerID
                                };
                                partPrice.SetCreateAudit(UserName);

                                result.Add(partPrice);
                            }

                            if (decimal.TryParse(row["CM & TRIMS "].ToString(), out price))
                            {
                                var partPrice = new PartPrice()
                                {
                                    StyleNO = row["STYLE"].ToString(),
                                    GarmentColorCode = row["MODEL"].ToString(),
                                    Season = row["SEASON"].ToString(),
                                    EffectiveDate = effDate,
                                    ExpiryDate = expDate,
                                    Price = price,
                                    ProductionType = "CMT",
                                    CustomerID = CustomerID
                                };
                                partPrice.SetCreateAudit(UserName);

                                result.Add(partPrice);
                            }

                            if (decimal.TryParse(row["EXW Price "].ToString(), out price))
                            {
                                var partPrice = new PartPrice()
                                {
                                    StyleNO = row["STYLE"].ToString(),
                                    GarmentColorCode = row["MODEL"].ToString(),
                                    Season = row["SEASON"].ToString(),
                                    EffectiveDate = effDate,
                                    ExpiryDate = expDate,
                                    Price = price,
                                    ProductionType = "FOB",
                                    CustomerID = CustomerID
                                };
                                partPrice.SetCreateAudit(UserName);

                                result.Add(partPrice);
                            }
                        } 
                    }
                }
            }
            else
            {
                errorMessage = "Invalid file";
            }

            return result;
        }
    }
}
