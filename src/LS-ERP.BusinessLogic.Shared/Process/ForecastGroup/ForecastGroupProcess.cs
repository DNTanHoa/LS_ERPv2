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
    public class ForecastGroupProcess
    {
        public static bool Import(IEnumerable<Size> sizes, string filePath, string fileName, string customerID, string userName, 
            out string errorMessage, out List<ForecastOverall> forecastOveralls)
        {
            errorMessage = string.Empty;
            forecastOveralls = new List<ForecastOverall>();

            switch(customerID)
            {
                case "DE":
                    forecastOveralls = ReadDataDE(sizes, filePath, fileName, out errorMessage);
                    break;
                case "GA":
                    forecastOveralls = ReadDataGA(sizes, filePath, fileName, out errorMessage);
                    break;
                default:
                    break;
            }

            return string.IsNullOrEmpty(errorMessage) ? true : false;
        }

        #region support function

        public static List<ForecastOverall> ReadDataGA(IEnumerable<Size> sizes, string filePath, string fileName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (File.Exists(filePath) &&
               Path.GetExtension(filePath).Equals(".xlsx"))
            {
                var result = new List<ForecastOverall>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets.First();
                        var table = workSheet.ToDataTable(2, true, 1);
                        var sizeTable = workSheet.ToDataTable(startHeader: 2, startColumn: 15,
                            endColumn: 51, startRow: 2);

                        foreach (DataRow row in table.Rows)
                        {
                            if (string.IsNullOrEmpty(row["GARAN STYLE#"]?.ToString()))
                                break;

                            var forecastOverall = new ForecastOverall()
                            {
                                CustomerStyle = row["GARAN STYLE#"]?.ToString(),
                                GarmentColorCode = row["COLOR #"]?.ToString(),
                                GarmentColorName = row["COLOR NAME"]?.ToString(),
                                Description = row["STYLE DESCRIPTION"]?.ToString(),
                                PurchaseOrderNumber = row["ORDER NO"]?.ToString(),
                                ContractNo = row["WALMART STYLE#"]?.ToString(),
                                FabricContent = row["Fabric content"]?.ToString(),
                                Zone = row["Order Type"]?.ToString(),
                                ShipDate = DateTime.Parse(row["Shipment date"]?.ToString()),
                                SaveFilePath = filePath,
                                FileName = fileName,
                                ForecastDetails = new List<ForecastDetail>(),
                            };

                            foreach (DataColumn column in sizeTable.Columns)
                            {
                                if (column.ColumnName.Contains("Column"))
                                    break;

                                var quatityRow = sizeTable.Rows[table.Rows.IndexOf(row)];


                                if (string.IsNullOrEmpty(quatityRow[sizeTable.Columns.IndexOf(column)]?
                                            .ToString().Trim()))
                                    continue;

                                var size = GetCorrectSize(ref sizes, column.ColumnName);

                                if (size == null)
                                {
                                    errorMessage = "Can't find size with name " + column.ColumnName;
                                    return null;
                                }

                                var forcastDetail = new ForecastDetail()
                                {
                                    GarmentSize = size.Code,
                                    PlannedQuantity = int.Parse(quatityRow[sizeTable.Columns.IndexOf(column)]?
                                            .ToString().Replace(".", "").Replace(",", "")),
                                    SizeSortIndex = size.SequeneceNumber ?? 0,
                                };

                                forecastOverall.ForecastDetails.Add(forcastDetail);
                            }


                            result.Add(forecastOverall);
                        }
                    }
                }

                return result;
            }
            else
            {
                errorMessage = "Invalid file";
            }
            return null;
        }
        public static List<ForecastOverall> ReadDataDE(IEnumerable<Size> sizes,
            string filePath, string fileName, out string errorMessage)
        {
            var result = new List<ForecastOverall>();
            errorMessage = string.Empty;

            if (File.Exists(filePath) &&
               Path.GetExtension(filePath).Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    var workSheet = package.Workbook.Worksheets[1];
                    var table = workSheet.ToDataTable(4);

                    string currentStyle = string.Empty;
                    string currentColorCode = string.Empty;
                    string currentSeason = string.Empty;
                    string currentZone = string.Empty;
                    string currentCHDWeek = string.Empty;

                    ForecastOverall forcastOverall = null;

                    foreach (DataRow row in table.Rows)
                    {
                        if (string.IsNullOrEmpty(row["IMAN"]?.ToString().Trim()))
                        {
                            if (forcastOverall != null &&
                                string.IsNullOrEmpty(forcastOverall.ID))
                            {
                                forcastOverall.ID = Nanoid.Nanoid
                                        .Generate("123456789ABCDEFGHIJKLMNOPRST", 15);
                                result.Add(forcastOverall);
                            }
                            break;
                        }

                        var sizeDescription = row["ITEM NAME"]?.ToString();

                        if (currentStyle != row["IMAN"]?.ToString() ||
                            currentColorCode != row["MODEL"]?.ToString() ||
                            currentSeason != row["Season"]?.ToString() ||
                            currentCHDWeek != row["CHD WEEK"]?.ToString() ||
                            currentZone != row["ZONE"]?.ToString())
                        {
                            currentStyle = row["IMAN"]?.ToString();
                            currentColorCode = row["MODEL"]?.ToString();
                            currentSeason = row["Season"]?.ToString();
                            currentCHDWeek = row["CHD WEEK"]?.ToString();
                            currentZone = row["ZONE"]?.ToString();

                            if (forcastOverall != null &&
                                string.IsNullOrEmpty(forcastOverall.ID))
                            {
                                forcastOverall.ID = Nanoid.Nanoid
                                        .Generate("123456789ABCDEFGHIJKLMNOPRST", 15);
                                result.Add(forcastOverall);
                            }

                            ForecastOverall existForcastOverall = null;
                            existForcastOverall = result
                                .FirstOrDefault(x => x.CustomerStyle == currentStyle &&
                                                     x.GarmentColorCode == currentColorCode &&
                                                     x.Season == currentSeason &&
                                                     x.Zone == currentZone &&
                                                     x.CreateWeekTitle == currentCHDWeek);

                            if(existForcastOverall == null)
                            {
                                forcastOverall = new ForecastOverall()
                                {
                                    ID = string.Empty,
                                    CustomerStyle = row["IMAN"]?.ToString(),
                                    GarmentColorCode = row["MODEL"]?.ToString(),
                                    GarmentColorName = row["MODEL"]?.ToString(),
                                    Description = sizeDescription.Split(",").First().Trim(),
                                    PurchaseOrderNumber = row["FC NUMBER"]?.ToString(),
                                    Zone = row["ZONE"]?.ToString(),
                                    Season = row["Season"]?.ToString(),
                                    SaveFilePath = filePath,
                                    FileName = fileName,
                                    CreateWeekTitle = row["CHD WEEK"]?.ToString(),

                                    ForecastDetails = new List<ForecastDetail>(),
                                };
                            }
                            else
                            {
                                forcastOverall = existForcastOverall;
                            }

                            var size = GetCorrectSize(ref sizes, sizeDescription.Split(',').Last().Trim().Replace(".",""));

                            if (size == null)
                            {
                                errorMessage = "Can't find size with name " +
                                    sizeDescription.Split(',').Last().Trim();
                                return null;
                            }

                            var forcastDetail = new ForecastDetail()
                            {
                                GarmentSize = size.Code,
                                PlannedQuantity = int.Parse(row["QUANTITY"]?.ToString()),
                                SizeSortIndex = size.SequeneceNumber ?? 0,
                                ItemCode = row["ITEM CODE"]?.ToString()
                            };

                            forcastOverall.ForecastDetails.Add(forcastDetail);
                        }
                        else
                        {
                            var size = GetCorrectSize(ref sizes, sizeDescription.Split(',').Last().Trim().Replace(".", ""));

                            if (size == null)
                            {
                                errorMessage = "Can't find size with name " + 
                                    sizeDescription.Split(',').Last().Trim();
                                return null;
                            }

                            var forcastDetail = new ForecastDetail()
                            {
                                GarmentSize = size.Code,
                                PlannedQuantity = int.Parse(row["QUANTITY"]?.ToString()),
                                SizeSortIndex = size.SequeneceNumber ?? 0,
                                ItemCode = row["ITEM CODE"]?.ToString()
                            };

                            forcastOverall.ForecastDetails.Add(forcastDetail);
                        }

                        if(row == table.Rows[table.Rows.Count - 1])
                        {
                            if (forcastOverall != null &&
                                string.IsNullOrEmpty(forcastOverall.ID))
                            {
                                forcastOverall.ID = Nanoid.Nanoid
                                        .Generate("123456789ABCDEFGHIJKLMNOPRST", 15);
                                result.Add(forcastOverall);
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

        public static Size GetCorrectSize(ref IEnumerable<Size> sizes, string inputSize)
        {
            var size = sizes.FirstOrDefault(x => x.Code.ToUpper().Replace(" ", "").Trim() ==
                                                        inputSize.ToUpper().Replace(" ", "").Trim());
            return size;
        }

        #endregion
    }
}
