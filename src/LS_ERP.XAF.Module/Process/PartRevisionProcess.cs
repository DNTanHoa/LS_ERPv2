using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class PartRevisionProcess
    {
        static string customerStyle = "CustomerStyle";
        static string materialClass = "MaterialClass";
        static string title = "Title";

        public static Stream CreateExcelFileDE(PartRevision partRevision, Stream stream = null)
        {
            string Author = "Leading Star";
            int row = 2;
            var dicFormat = new Dictionary<string, List<int>>();
            Dictionary<string, Dictionary<string, List<ExportPartRevisionDto>>> dicStyle =
                SetDataDE(partRevision, out row);

            if (!String.IsNullOrEmpty(partRevision.CreatedBy))
            {
                Author = partRevision.CreatedBy;
            }

            string Title = partRevision.PartNumber;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Master Bom of Leading Start";
                excelPackage.Workbook.Worksheets.Add(Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 13));
                workSheet.DefaultColWidth = 12;

                int startRow = 4;
                int startCol = 1;

                foreach (var item in dicStyle)
                {
                    startCol = 1;
                    List<int> listCustomer = new List<int>();

                    if (dicFormat.TryGetValue(customerStyle, out List<int> rs))
                    {
                        listCustomer = rs;
                    }

                    listCustomer.Add(startRow);
                    dicFormat[customerStyle] = listCustomer;

                    workSheet.Cells[startRow++, startCol].Value = item.Key.ToString();

                    foreach (var itemChild in item.Value)
                    {
                        startCol = 1;

                        List<int> listMaterialClass = new List<int>();

                        if (dicFormat.TryGetValue(materialClass, out List<int> rsMaterial))
                        {
                            listMaterialClass = rsMaterial;
                        }

                        listMaterialClass.Add(startRow);
                        dicFormat[materialClass] = listMaterialClass;

                        workSheet.Cells[startRow++, startCol].Value = itemChild.Key.ToString();


                        List<int> listTitle = new List<int>();

                        if (dicFormat.TryGetValue(title, out List<int> rsTitle))
                        {
                            listTitle = rsTitle;
                        }

                        listTitle.Add(startRow);
                        dicFormat[title] = listTitle;

                        workSheet.Cells[startRow, startCol++].Value = "Part";
                        workSheet.Cells[startRow, startCol++].Value = "";

                        workSheet.Cells[startRow, startCol++].Value = "Dsm Code";
                        workSheet.Cells[startRow, startCol++].Value = "Model Code";

                        workSheet.Cells[startRow, startCol++].Value = "Component";
                        workSheet.Cells[startRow, startCol++].Value = "";
                        workSheet.Cells[startRow, startCol++].Value = "Item Code";
                        workSheet.Cells[startRow, startCol++].Value = "";
                        workSheet.Cells[startRow, startCol++].Value = "Grid Value";
                        workSheet.Cells[startRow, startCol++].Value = "";
                        workSheet.Cells[startRow, startCol++].Value = "Items";
                        workSheet.Cells[startRow, startCol++].Value = "Qty";
                        workSheet.Cells[startRow, startCol++].Value = "";
                        workSheet.Cells[startRow, startCol++].Value = "Unit";
                        workSheet.Cells[startRow, startCol++].Value = "Comments";
                        workSheet.Cells[startRow, startCol++].Value = "";
                        workSheet.Cells[startRow, startCol++].Value = "";
                        workSheet.Cells[startRow, startCol++].Value = "";


                        workSheet.Cells[startRow, startCol++].Value = "Specification";
                        workSheet.Cells[startRow, startCol++].Value = "Price";
                        workSheet.Cells[startRow, startCol++].Value = "Unit (Price)";
                        workSheet.Cells[startRow, startCol++].Value = "Currency";
                        workSheet.Cells[startRow, startCol++].Value = "Vendor Code";
                        workSheet.Cells[startRow, startCol++].Value = "Lead Time";
                        workSheet.Cells[startRow, startCol++].Value = "Wastage (%)";
                        workSheet.Cells[startRow, startCol++].Value = "Less (%)";
                        workSheet.Cells[startRow, startCol++].Value = "Over (%)";
                        workSheet.Cells[startRow, startCol++].Value = "Fabrics Weight g/m2";
                        workSheet.Cells[startRow, startCol++].Value = "Fabrics Width inch";

                        startRow++;
                        foreach (var itemChildExport in itemChild.Value)
                        {
                            startCol = 1;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.ItemNo;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.OtherName; // get Other Name 
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.DsmCode;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.MaterialCode;

                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Description;
                            workSheet.Cells[startRow, startCol++].Value = "";
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.ColorCode;
                            workSheet.Cells[startRow, startCol++].Value = "";
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.ColorName;
                            workSheet.Cells[startRow, startCol++].Value = "";
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.GarmentSize;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Consumption;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Consumption;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.UnitBOM;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Position;
                            workSheet.Cells[startRow, startCol++].Value = "";
                            workSheet.Cells[startRow, startCol++].Value = "";
                            workSheet.Cells[startRow, startCol++].Value = "";
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Specification;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Price;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.UnitPrice;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Currency;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Vendor;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.LeadTime;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Wastage;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Less;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.Over;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.FabricsWeight;
                            workSheet.Cells[startRow, startCol++].Value = itemChildExport.FabricsWidth;

                            startRow++;
                        }
                    }
                }

                if (startRow > 1)
                {
                    string modelRangeBorder = "A5:AC" + startRow.ToString();
                    using (var range = workSheet.Cells[modelRangeBorder])
                    {
                        range.Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.AutoFitColumns();
                    }

                    foreach (var itemRange in dicFormat)
                    {
                        if (itemRange.Key.ToString().Equals(customerStyle))
                        {
                            foreach (var itemCustomer in itemRange.Value)
                            {
                                string modelRangeCustomer = "A" + itemCustomer + ":AC" + itemCustomer;
                                using (var rangeCustomer = workSheet.Cells[modelRangeCustomer])
                                {
                                    rangeCustomer.Style.Font.SetFromFont(new Font("Times New Roman", 18));
                                    rangeCustomer.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    rangeCustomer.Style.Font.Bold = true;
                                    rangeCustomer.Style.Font.Color.SetColor(Color.White);
                                    rangeCustomer.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 0, 0));

                                }
                            }
                        }

                        else if (itemRange.Key.ToString().Equals(materialClass))
                        {
                            foreach (var itemCustomer in itemRange.Value)
                            {
                                string modelRangeMaterial = "A" + itemCustomer + ":AC" + itemCustomer;
                                using (var rangeMaterial = workSheet.Cells[modelRangeMaterial])
                                {
                                    rangeMaterial.Style.Font.SetFromFont(new Font("Times New Roman", 14));
                                    rangeMaterial.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    rangeMaterial.Style.Font.Bold = true;
                                    rangeMaterial.Style.Font.Color.SetColor(Color.Blue);
                                    rangeMaterial.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                                }
                            }
                        }

                        else if (itemRange.Key.ToString().Equals(title))
                        {
                            foreach (var itemCustomer in itemRange.Value)
                            {
                                string modelRangeTitle = "A" + itemCustomer + ":AC" + itemCustomer;
                                using (var rangeTitle = workSheet.Cells[modelRangeTitle])
                                {
                                    rangeTitle.Style.Font.SetFromFont(new Font("Times New Roman", 13));
                                    rangeTitle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    rangeTitle.Style.Font.Bold = true;
                                    rangeTitle.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(178, 195, 196));
                                }
                            }
                        }
                    }


                    for (int i = 1; i < startRow; i++)
                    {
                        using (var range = workSheet.Cells["A" + i + ":B" + i])
                        {
                            if (!int.TryParse(workSheet.Cells[i, 1].GetValue<string>(), out int value))
                            {
                                range.Merge = true;
                            }

                        }
                        //using (var range = workSheet.Cells["C" + i + ":D" + i])
                        //{
                        //    range.Merge = true;

                        //}
                        using (var range = workSheet.Cells["E" + i + ":F" + i])
                        {
                            range.Merge = true;

                        }
                        using (var range = workSheet.Cells["G" + i + ":H" + i])
                        {
                            range.Merge = true;

                        }
                        using (var range = workSheet.Cells["I" + i + ":J" + i])
                        {

                            range.Merge = true;


                        }
                        using (var range = workSheet.Cells["L" + i + ":M" + i])
                        {
                            range.Merge = true;
                            range.Style.Numberformat.Format = "#,##0.00";

                        }
                        using (var range = workSheet.Cells["O" + i + ":R" + i])
                        {
                            range.Merge = true;

                        }
                    }

                }

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public static Stream CreateExcelFileDefault(PartRevision partRevision, Stream stream = null)
        {
            string Author = "Leading Star";
            if (!String.IsNullOrEmpty(partRevision.CreatedBy))
            {
                Author = partRevision.CreatedBy;
            }

            var data = SetDataTableDefault(partRevision);

            string Title = partRevision.PartNumber;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Master Bom of Leading Start";
                excelPackage.Workbook.Worksheets.Add(Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet = SetDefaultHeader(workSheet, data.Rows.Count);
                workSheet.Cells[2, 1].LoadFromDataTable(data);

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public static void CreateHeader(ExcelWorksheet workSheet, string CustomerID)
        {
            switch (CustomerID)
            {
                case "DE":
                    CreateHeaderDE(workSheet);
                    break;
                default:
                    break;
            }
        }

        public static void CreateHeaderDE(ExcelWorksheet workSheet)
        {
            workSheet.Cells[1, 1].Value = "Item No";
            workSheet.Cells[1, 2].Value = "Style";
            workSheet.Cells[1, 3].Value = "Color Garment Code";
            workSheet.Cells[1, 4].Value = "Color Garment Name";
            workSheet.Cells[1, 5].Value = "Material Code";
            workSheet.Cells[1, 6].Value = "Description";
            workSheet.Cells[1, 7].Value = "Color Code";
            workSheet.Cells[1, 8].Value = "Color Name";
            workSheet.Cells[1, 9].Value = "Garment Size";
            workSheet.Cells[1, 10].Value = "Garment Deben";
            workSheet.Cells[1, 11].Value = "Specification";
            workSheet.Cells[1, 12].Value = "Label";
            workSheet.Cells[1, 13].Value = "Division";
            workSheet.Cells[1, 14].Value = "Consumption";
            workSheet.Cells[1, 15].Value = "Unit (BOM)";
            workSheet.Cells[1, 16].Value = "Position";
            workSheet.Cells[1, 17].Value = "Material Class";
            workSheet.Cells[1, 18].Value = "Price";
            workSheet.Cells[1, 19].Value = "Unit (Price)";
            workSheet.Cells[1, 20].Value = "Currency";
            workSheet.Cells[1, 21].Value = "Vendor Code";
            workSheet.Cells[1, 22].Value = "Lead Time";
            workSheet.Cells[1, 23].Value = "Wastage (%)";
            workSheet.Cells[1, 24].Value = "Less (%)";
            workSheet.Cells[1, 25].Value = "Over (%)";
            workSheet.Cells[1, 26].Value = "Fabrics Weight (g/m2)";
            workSheet.Cells[1, 27].Value = "Fabrics Width (inch)";
            workSheet.Cells[1, 28].Value = "Fabrics Width Cut (inch)";
            workSheet.Cells[1, 29].Value = "Material Class Type";
            workSheet.Cells[1, 30].Value = "Free Percent";
        }

        public static Dictionary<string, Dictionary<string, List<ExportPartRevisionDto>>>
            SetDataDE(PartRevision partRevision, out int row)
        {
            Dictionary<string, Dictionary<string, List<ExportPartRevisionDto>>> listData =
                new Dictionary<string, Dictionary<string, List<ExportPartRevisionDto>>>();

            row = 2;
            var arrChangeRow = new List<int>();

            var partMaterials = partRevision.PartMaterials
                .OrderBy(x => x.ExternalCode);

            foreach (PartMaterial partMaterial in partMaterials)
            {
                var exportDto = new ExportPartRevisionDto()
                {
                    ItemNo = partMaterial.ExternalCode,
                    Style = partRevision.PartNumber,
                    ColorGarmentCode = partMaterial.GarmentColorCode,
                    ColorGarmentName = partMaterial.GarmentColorName,
                    MaterialCode = partMaterial.ItemID,
                    DsmCode = partMaterial.DsmItemID,
                    Description = partMaterial.ItemName,
                    ColorCode = partMaterial.ItemColorCode,
                    ColorName = partMaterial.ItemColorName,

                    GarmentSize = partMaterial.GarmentSize,
                    GarmentDeben = false,
                    Specification = partMaterial.Specify,
                    Label = partMaterial.LabelCode,
                    Division = partMaterial.Division,

                    Consumption = partMaterial.QuantityPerUnit ?? 0,
                    UnitBOM = partMaterial.PerUnitID,
                    Position = partMaterial.Position,
                    MaterialClass = partMaterial.MaterialType.Code,
                    Price = partMaterial.Price ?? 0,

                    UnitPrice = partMaterial.PriceUnitID,
                    Currency = partMaterial.CurrencyID,
                    Vendor = partMaterial.VendorID,
                    LeadTime = partMaterial.LeadTime ?? 0,
                    Wastage = partMaterial.WastagePercent ?? 0,
                    Less = partMaterial.LessPercent ?? 0,
                    Over = partMaterial.OverPercent ?? 0,
                    FabricsWeight = partMaterial.FabricWeight ?? 0,
                    FabricsWidth = partMaterial.FabricWidth ?? 0,
                    FabricsWidthCut = partMaterial.CutWidth ?? 0,
                    MaterialClassType = partMaterial.MaterialTypeClass,
                    FreePercent = partMaterial.FreePercent ?? 0,
                    OtherName = partMaterial.OtherName,
                };
                row++;

                string key = partRevision.Season + ":" + partRevision.PartNumber
                    + " - -(R3 Code: " + exportDto.ColorGarmentCode + ")";

                Dictionary<string, List<ExportPartRevisionDto>> childDicData =
                    new Dictionary<string, List<ExportPartRevisionDto>>();
                List<ExportPartRevisionDto> childListData = new List<ExportPartRevisionDto>();

                string keyChildDicData = partMaterial.MaterialType.Name;

                if (partMaterial.MaterialType.Equals("PKG"))
                {
                    keyChildDicData = "Packaging";
                }

                if (listData.TryGetValue(key, out Dictionary<string, List<ExportPartRevisionDto>> rsDic))
                {
                    childDicData = rsDic;

                    if (childDicData.TryGetValue(keyChildDicData, out List<ExportPartRevisionDto> rsList))
                    {
                        childListData = rsList;
                    }
                }

                childListData.Add(exportDto);
                childDicData[keyChildDicData] = childListData;
                listData[key] = childDicData;
            }

            return listData;
        }

        public static List<ExportPartRevisionDto>
            SetDataDefault(PartRevision partRevision)
        {
            List<ExportPartRevisionDto> listData =
                new List<ExportPartRevisionDto>();
            var partMaterials = partRevision.PartMaterials
                .OrderBy(x => x.ExternalCode);

            foreach (var partMaterial in partMaterials)
            {
                ExportPartRevisionDto exportBOM_DTO = new ExportPartRevisionDto();

                exportBOM_DTO.ItemNo = partMaterial.ExternalCode;
                exportBOM_DTO.Style = partMaterial.ItemStyleNumber;
                exportBOM_DTO.ColorGarmentCode = partMaterial.GarmentColorCode;
                exportBOM_DTO.ColorGarmentName = partMaterial.GarmentColorName;
                exportBOM_DTO.DsmCode = partMaterial.DsmItemID;
                exportBOM_DTO.MaterialCode = partMaterial.ItemID;
                exportBOM_DTO.Description = partMaterial.ItemName;
                exportBOM_DTO.ColorCode = partMaterial.ItemColorCode;
                exportBOM_DTO.ColorName = partMaterial.ItemColorName;

                exportBOM_DTO.GarmentSize = partMaterial.GarmentSize;
                exportBOM_DTO.GarmentDeben = false;
                exportBOM_DTO.Specification = partMaterial.Specify;
                exportBOM_DTO.Label = partMaterial.LabelCode;
                exportBOM_DTO.Division = partMaterial.Division;

                exportBOM_DTO.Consumption = partMaterial.QuantityPerUnit ?? 0;
                exportBOM_DTO.UnitBOM = partMaterial.PerUnitID;
                exportBOM_DTO.Position = partMaterial.Position;
                exportBOM_DTO.MaterialClass = partMaterial.MaterialTypeCode;
                exportBOM_DTO.Price = partMaterial.Price ?? 0;

                exportBOM_DTO.UnitPrice = partMaterial.PriceUnitID;
                exportBOM_DTO.Currency = partMaterial.CurrencyID;
                exportBOM_DTO.Vendor = partMaterial.VendorID;
                exportBOM_DTO.LeadTime = partMaterial.LeadTime ?? 0;
                exportBOM_DTO.Wastage = partMaterial.WastagePercent ?? 0;

                exportBOM_DTO.Less = partMaterial.LessPercent ?? 0;
                exportBOM_DTO.Over = partMaterial.OverPercent ?? 0;
                exportBOM_DTO.FabricsWeight = partMaterial.FabricWeight ?? 0;
                exportBOM_DTO.FabricsWidth = partMaterial.FabricWidth ?? 0;
                exportBOM_DTO.FabricsWidthCut = partMaterial.CutWidth ?? 0;

                exportBOM_DTO.MaterialClassType = partMaterial.MaterialTypeClass;
                exportBOM_DTO.FreePercent = partMaterial.FreePercent ?? 0;

                listData.Add(exportBOM_DTO);
            }

            return listData;
        }

        public static DataTable
            SetDataTableDefault(PartRevision partRevision)
        {
            var table = new DataTable();
            var partMaterials = partRevision.PartMaterials
                .OrderBy(x => x.ExternalCode);

            for (int i = 0; i < 31; i++)
            {
                var columnName = "Column " + i.ToString();
                table.Columns.Add(new DataColumn(columnName));
            }

            foreach (var partMaterial in partMaterials)
            {
                var row = table.NewRow();

                row["Column 0"] = partMaterial.ExternalCode;
                row["Column 1"] = partMaterial.ItemStyleNumber;
                row["Column 2"] = partMaterial.GarmentColorCode;
                row["Column 3"] = partMaterial.GarmentColorName;
                row["Column 4"] = partMaterial.ItemID;
                row["Column 5"] = partMaterial.ItemName;
                row["Column 6"] = partMaterial.ItemColorCode;
                row["Column 7"] = partMaterial.ItemColorName;

                row["Column 8"] = partMaterial.GarmentSize;
                row["Column 9"] = false;
                row["Column 10"] = partMaterial.Specify;
                row["Column 11"] = partMaterial.LabelCode;
                row["Column 12"] = partMaterial.Division;

                row["Column 13"] = partMaterial.QuantityPerUnit ?? 0;
                row["Column 14"] = partMaterial.PerUnitID;
                row["Column 15"] = partMaterial.Position;
                row["Column 16"] = partMaterial.MaterialTypeCode;
                row["Column 17"] = partMaterial.Price ?? 0;

                row["Column 18"] = partMaterial.PriceUnitID;
                row["Column 19"] = partMaterial.CurrencyID;
                row["Column 20"] = partMaterial.VendorID;
                row["Column 21"] = partMaterial.LeadTime ?? 0;
                row["Column 22"] = partMaterial.WastagePercent ?? 0;

                row["Column 23"] = partMaterial.LessPercent ?? 0;
                row["Column 24"] = partMaterial.OverPercent ?? 0;
                row["Column 25"] = partMaterial.FabricWeight ?? 0;
                row["Column 26"] = partMaterial.FabricWidth ?? 0;
                row["Column 27"] = partMaterial.CutWidth ?? 0;

                row["Column 28"] = partMaterial.MaterialTypeClass;
                row["Column 29"] = partMaterial.FreePercent ?? 0;

                table.Rows.Add(row);
            }

            return table;
        }

        public static ExcelWorksheet SetDefaultHeader(ExcelWorksheet worksheet, int row, int startRow = 1)
        {
            worksheet.Cells[startRow, 1].Value = "Item No";
            worksheet.Cells[startRow, 2].Value = "Style";
            worksheet.Cells[startRow, 3].Value = "Color Garment Code";
            worksheet.Cells[startRow, 4].Value = "Color Garment Name";
            worksheet.Cells[startRow, 5].Value = "Material Code";
            worksheet.Cells[startRow, 6].Value = "Description";
            worksheet.Cells[startRow, 7].Value = "Color Code";
            worksheet.Cells[startRow, 8].Value = "Color Name";
            worksheet.Cells[startRow, 9].Value = "Garment Size";
            worksheet.Cells[startRow, 10].Value = "Garment Deben";
            worksheet.Cells[startRow, 11].Value = "Specification";
            worksheet.Cells[startRow, 12].Value = "Label";
            worksheet.Cells[startRow, 13].Value = "Division";
            worksheet.Cells[startRow, 14].Value = "Consumption";
            worksheet.Cells[startRow, 15].Value = "Unit (BOM)";
            worksheet.Cells[startRow, 16].Value = "Position";
            worksheet.Cells[startRow, 17].Value = "Material Class";
            worksheet.Cells[startRow, 18].Value = "Price";
            worksheet.Cells[startRow, 19].Value = "Unit (Price)";
            worksheet.Cells[startRow, 20].Value = "Currency";
            worksheet.Cells[startRow, 21].Value = "Vendor Code";
            worksheet.Cells[startRow, 22].Value = "Lead Time";
            worksheet.Cells[startRow, 23].Value = "Wastage (%)";
            worksheet.Cells[startRow, 24].Value = "Less (%)";
            worksheet.Cells[startRow, 25].Value = "Over (%)";
            worksheet.Cells[startRow, 26].Value = "Fabrics Weight (g/m2)";
            worksheet.Cells[startRow, 27].Value = "Fabrics Width (inch)";
            worksheet.Cells[startRow, 28].Value = "Fabrics Width Cut (inch)";
            worksheet.Cells[startRow, 29].Value = "Material Class Type";
            worksheet.Cells[startRow, 30].Value = "Free Percent";

            string modelRangeBorder = "A1:AD" + (row + 1).ToString();
            using (var range = worksheet.Cells[modelRangeBorder])
            {
                range.Style.Font.SetFromFont(new Font("Times New Roman", 13));
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.AutoFitColumns();
            }

            using (var range = worksheet.Cells["A1:AD1"])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }

            return worksheet;
        }
    }
}
