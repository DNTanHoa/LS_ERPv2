using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Extensions;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class ImportPartRevisionProcess
    {
        public static bool Import(string filePath, string Username, string Filename,
            string customerID,
            string season,
            List<MaterialType> materialTypes,
            IQueryable<Item> items,
            out PartRevision partRevision,
            out List<PartRevision> partRevisions,
            out List<PartMaterial> partMaterials,
            out List<Item> newItems,
            out string errorMessage)
        {
            partRevision = new PartRevision();
            errorMessage = string.Empty;
            newItems = new List<Item>();
            partMaterials = new List<PartMaterial>();
            partRevision.Season = season;
            partRevision.CustomerID = customerID;
            partRevisions = new List<PartRevision>();
            var dicItems = items.ToDictionary(x => x.ToSearchKey());

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
                            switch (customerID)
                            {
                                case "DE":
                                    {
                                        int row = 0;
                                        if (workSheet.Dimension != null)
                                        {
                                            row = workSheet.Dimension.End.Row;
                                        }

                                        /// Check file gốc hay export
                                        bool originalFile = false;
                                        var mergeCell = workSheet.MergedCells;

                                        if ( !int.TryParse(workSheet.Cells[7, 1].GetValue<string>(), out int value) &&
                                                string.IsNullOrEmpty(workSheet.Cells[7, 2].GetValue<string>()))
                                        {
                                            originalFile = true;
                                            //break;
                                        }

                                        int externalCode = 1;
                                        MaterialType materialType = new MaterialType();
                                        Dictionary<string, PartMaterial> dicMaterial = new Dictionary<string, PartMaterial>();
                                        bool checkRowPart = false;
                                        string garmentColorCode = string.Empty;
                                        for (int i = 4; i <= row; i++)
                                        {
                                            int column = 1;
                                            try
                                            {
                                                string dataLine = workSheet.Cells[i, column].GetValue<string>();
                                                string columnPart = workSheet.Cells[i + 1, column].GetValue<string>();

                                                if (dataLine == null && columnPart == null)
                                                {
                                                    materialType = new MaterialType();
                                                }

                                                if (!string.IsNullOrEmpty(dataLine))
                                                {
                                                    if (dataLine.Replace(" ", "")
                                                                .Replace("-", "")
                                                                .ToUpper().Trim().Equals("BOMDeveloppement"
                                                                                         .ToUpper()))
                                                    {
                                                        continue;
                                                    }

                                                    if (dataLine.Contains("R3 Code"))
                                                    {
                                                        string newGarmentColorCode = dataLine.Substring(
                                                                                     dataLine.LastIndexOf(":") + 2,
                                                                                     dataLine.Length - dataLine.LastIndexOf(":") - 3).Trim();
                                                        if (garmentColorCode != newGarmentColorCode)
                                                        {
                                                            externalCode = 1;
                                                            garmentColorCode = newGarmentColorCode;
                                                        }

                                                        continue;
                                                    }

                                                    if (!string.IsNullOrEmpty(columnPart) && columnPart.Equals("Part"))
                                                    {
                                                        checkRowPart = true;
                                                    }
                                                    else
                                                    {
                                                        checkRowPart = false;
                                                    }

                                                    if (materialType != null && !string.IsNullOrEmpty(materialType.Name)
                                                        && !materialType.Name.Equals(dataLine) && checkRowPart)
                                                    {
                                                        materialType = materialTypes.FirstOrDefault(x => x.Name == dataLine);

                                                        if (materialType != null && !materialType.Code.Equals("FB") && dataLine != "Accessories")
                                                        {
                                                            materialType = materialTypes.FirstOrDefault(x => x.Name == "Accessories");
                                                        }

                                                        continue;
                                                    }
                                                    else if (materialType != null && string.IsNullOrEmpty(materialType.Name) && checkRowPart)
                                                    {
                                                        materialType = materialTypes.FirstOrDefault(x => x.Name == dataLine);

                                                        if (materialType != null && !materialType.Code.Equals("FB") && dataLine != "Accessories")
                                                        {
                                                            materialType = materialTypes.FirstOrDefault(x => x.Name == "Accessories");
                                                        }

                                                        continue;
                                                    }

                                                    if (materialType != null && !dataLine.Equals("Part") && !checkRowPart)
                                                    {
                                                        //i += 2;
                                                        PartMaterial partMaterial = new PartMaterial();

                                                        partMaterial.GarmentColorCode = garmentColorCode;
                                                        partMaterial.ExternalCode = externalCode.ToString().PadLeft(3, '0');
                                                        if (originalFile) /// Modify get data column Other Name
                                                        {
                                                            partMaterial.OtherName = workSheet.Cells[i, column].GetValue<string>();
                                                        }
                                                        else
                                                        {
                                                            partMaterial.OtherName = workSheet.Cells[i, (column + 1)].GetValue<string>();
                                                        }
                                                        column += 2; // B
                                                        partMaterial.DsmItemID = workSheet.Cells[i, column++].GetValue<string>();  // C

                                                        partMaterial.ItemID = workSheet.Cells[i, column++].GetValue<string>();  // D
                                                        partMaterial.ItemName = workSheet.Cells[i, column++].GetValue<string>(); // E
                                                        column++; // F
                                                        partMaterial.ItemColorCode = workSheet.Cells[i, column++].GetValue<string>();// G
                                                        column++; // H
                                                        partMaterial.ItemColorName = workSheet.Cells[i, column++].GetValue<string>(); // I
                                                        column++; // J
                                                        partMaterial.GarmentSize = workSheet.Cells[i, column++].GetValue<string>()?.ToUpper().Trim();  // K
                                                        partMaterial.QuantityPerUnit = workSheet.Cells[i, column++].GetValue<decimal>(); // L
                                                        column++; // M
                                                        partMaterial.PerUnitID = workSheet.Cells[i, column++].GetValue<string>()?.ToUpper().Trim(); // N
                                                        partMaterial.Position = workSheet.Cells[i, column++].GetValue<string>(); // O
                                                        column++; // P
                                                        column++; // Q
                                                        column++; // R
                                                        partMaterial.Specify = workSheet.Cells[i, column++].GetValue<string>(); // S
                                                        partMaterial.Price = workSheet.Cells[i, column++].GetValue<decimal>(); // T
                                                        partMaterial.PriceUnitID = workSheet.Cells[i, column++].GetValue<string>()?.ToUpper().Trim(); // U
                                                        partMaterial.CurrencyID = workSheet.Cells[i, column++].GetValue<string>()?.ToUpper().Trim(); // V
                                                        partMaterial.VendorID = workSheet.Cells[i, column++].GetValue<string>()?.ToUpper().Trim(); // W
                                                        partMaterial.LeadTime = workSheet.Cells[i, column++].GetValue<int>(); // X  

                                                        partMaterial.WastagePercent = workSheet.Cells[i, column++].GetValue<decimal>(); // Y
                                                        partMaterial.LessPercent = workSheet.Cells[i, column++].GetValue<decimal>(); // Z
                                                        partMaterial.OverPercent = workSheet.Cells[i, column++].GetValue<decimal>(); // AA
                                                        partMaterial.FabricWeight = workSheet.Cells[i, column++].GetValue<decimal>(); // AB
                                                        partMaterial.FabricWidth = workSheet.Cells[i, column++].GetValue<decimal>(); // AC

                                                        partMaterial.MaterialTypeCode = materialType.Code;
                                                        partMaterial.SetCreateAudit(Username);
                                                        string key = partMaterial.DsmItemID + partMaterial.Specify + partMaterial.GarmentSize;

                                                        if (dicMaterial.TryGetValue(key, out PartMaterial rsPartMaterial))
                                                        {
                                                            if (string.IsNullOrEmpty(partMaterial.Specify))
                                                            {
                                                                partMaterial.Specify = rsPartMaterial.Specify;
                                                            }


                                                            if (partMaterial.Price == null ||
                                                                (partMaterial.Price != null && partMaterial.Price == 0))
                                                            {
                                                                partMaterial.Price = rsPartMaterial.Price;
                                                            }


                                                            if (string.IsNullOrEmpty(partMaterial.PriceUnitID))
                                                            {
                                                                partMaterial.PriceUnitID = rsPartMaterial.PriceUnitID;
                                                            }


                                                            if (string.IsNullOrEmpty(partMaterial.CurrencyID))
                                                            {
                                                                partMaterial.CurrencyID = rsPartMaterial.CurrencyID;
                                                            }


                                                            if (string.IsNullOrEmpty(partMaterial.VendorID))
                                                            {
                                                                partMaterial.VendorID = rsPartMaterial.VendorID;
                                                            }


                                                            if (partMaterial.LeadTime == null ||
                                                                (partMaterial.LeadTime != null && partMaterial.LeadTime == 0))
                                                            {
                                                                partMaterial.LeadTime = rsPartMaterial.LeadTime;
                                                            }


                                                            if (partMaterial.WastagePercent == null ||
                                                                (partMaterial.WastagePercent != null && partMaterial.WastagePercent == 0))
                                                            {
                                                                partMaterial.WastagePercent = rsPartMaterial.WastagePercent;
                                                            }


                                                            if (partMaterial.LessPercent == null ||
                                                                (partMaterial.LessPercent != null && partMaterial.LessPercent == 0))
                                                            {
                                                                partMaterial.LessPercent = rsPartMaterial.LessPercent;
                                                            }


                                                            if (partMaterial.OverPercent == null ||
                                                                (partMaterial.OverPercent != null && partMaterial.OverPercent == 0))
                                                            {
                                                                partMaterial.OverPercent = rsPartMaterial.OverPercent;
                                                            }


                                                            if (partMaterial.FabricWeight == null ||
                                                                (partMaterial.FabricWeight != null && partMaterial.FabricWeight == 0))
                                                            {
                                                                partMaterial.FabricWeight = rsPartMaterial.FabricWeight;
                                                            }


                                                            if (partMaterial.FabricWidth == null ||
                                                                (partMaterial.FabricWidth != null && partMaterial.FabricWidth == 0))
                                                            {
                                                                partMaterial.FabricWidth = rsPartMaterial.FabricWidth;
                                                            }

                                                        }
                                                        else
                                                        {
                                                            dicMaterial[key] = partMaterial;
                                                        }

                                                        CreateItem(Username, partRevision, dicItems, ref partMaterial, ref newItems);

                                                        partRevision.PartMaterials.Add(partMaterial);
                                                        partRevision.SetCreateAudit(Username);
                                                        externalCode++;


                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Fail(ex, workSheet, column, out errorMessage);
                                                return false;
                                            }
                                        }
                                    }
                                    break;
                                case "PU":
                                    {
                                        ImportPU(filePath, Username, Filename,
                                                customerID, season, materialTypes, items,
                                               out partRevisions,
                                               out partMaterials,
                                               out newItems,
                                               out errorMessage);
                                    }
                                    break;
                                default:
                                    {
                                        var dataTable = workSheet.ToDataTable();

                                        foreach (DataRow row in dataTable.Rows)
                                        {
                                            var partMaterial = new PartMaterial()
                                            {
                                                ExternalCode = row["Item No"]?.ToString(),
                                                ItemID = row["Material Code"]?.ToString(),
                                                ItemName = row["Description"]?.ToString(),
                                                ItemColorCode = row["Color Code"]?.ToString(),
                                                ItemColorName = row["Color Name"]?.ToString(),
                                                Specify = row["Specification"]?.ToString(),
                                                Position = row["Position"]?.ToString(),
                                                Division = row["Division"]?.ToString(),
                                                ItemStyleNumber = row["Style"]?.ToString(),
                                                MaterialTypeCode = row["Material Class"]?.ToString().ToUpper().Trim(),
                                                PerUnitID = row["Unit (BOM)"]?.ToString().ToUpper().Trim(),
                                                PriceUnitID = row["Unit (Price)"]?.ToString().ToUpper().Trim(),
                                                CurrencyID = row["Currency"]?.ToString()?.ToUpper().Trim(),
                                                VendorID = row["Vendor Code"]?.ToString()?.ToUpper().Trim(),
                                                MaterialTypeClass = row["Material Class Type"]?.ToString()?.ToUpper().Trim(),
                                                GarmentColorCode = row["Color Garment Code"]?.ToString(),
                                                GarmentColorName = row["Color Garment Name"]?.ToString(),
                                                LabelCode = row["Label"]?.ToString(),
                                                GarmentSize = row["Garment Size"]?.ToString().ToUpper().Trim()
                                            };

                                            if (!string.IsNullOrEmpty(row["Lead Time"]?.ToString()))
                                            {
                                                var leadTimeParseResult = int.TryParse(row["Lead Time"]?.ToString(), out int leadTime);

                                                if (!leadTimeParseResult)
                                                {
                                                    errorMessage = "Invalid lead time input for ex code " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.LeadTime = leadTime;
                                            }
                                            else
                                            {
                                                partMaterial.LeadTime = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Consumption"]?.ToString()))
                                            {
                                                var quantityPerUnitParseResult = decimal.TryParse(row["Consumption"]?.ToString(),
                                                    out decimal quantityPerUnit);

                                                if (!quantityPerUnitParseResult)
                                                {
                                                    errorMessage = "Invalid quantity per unit input for ex code "
                                                        + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.QuantityPerUnit = quantityPerUnit;
                                            }
                                            else
                                            {
                                                partMaterial.QuantityPerUnit = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Price"]?.ToString()))
                                            {
                                                var priceParseResult = decimal.TryParse(row["Price"]?.ToString(), out decimal Price);

                                                if (!priceParseResult)
                                                {
                                                    errorMessage = "Invalid price input for ex code " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.Price = Price;
                                            }
                                            else
                                            {
                                                partMaterial.Price = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Over (%)"]?.ToString()))
                                            {
                                                var overPercentParseResult = decimal.TryParse(row["Over (%)"]?.ToString(),
                                                    out decimal overPercent);

                                                if (!overPercentParseResult)
                                                {
                                                    errorMessage = "Invalid over percent input for ex code " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.OverPercent = overPercent;
                                            }
                                            else
                                            {
                                                partMaterial.OverPercent = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Less (%)"]?.ToString()))
                                            {
                                                var lessPercentParseResult = decimal.TryParse(row["Less (%)"]?.ToString(),
                                                    out decimal lessPercent);

                                                if (!lessPercentParseResult)
                                                {
                                                    errorMessage = "Invalid less percent input for ex code " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.LessPercent = lessPercent;
                                            }
                                            else
                                            {
                                                partMaterial.LessPercent = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Wastage (%)"]?.ToString()))
                                            {
                                                var wastagePercentParseResult = decimal.TryParse(row["Wastage (%)"]?.ToString(),
                                                    out decimal wastagePercent);

                                                if (!wastagePercentParseResult)
                                                {
                                                    errorMessage = "Invalid wastage percent input for ex code " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.WastagePercent = wastagePercent;
                                            }
                                            else
                                            {
                                                partMaterial.WastagePercent = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Free Percent"]?.ToString()))
                                            {
                                                var freePercentParseResult = decimal.TryParse(row["Free Percent"]?.ToString(),
                                                    out decimal freePercent);

                                                if (!freePercentParseResult)
                                                {
                                                    errorMessage = "Invalid wastage percent input for free percent " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.FreePercent = freePercent;
                                            }
                                            else
                                            {
                                                partMaterial.FreePercent = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Fabrics Weight (g/m2)"]?.ToString()))
                                            {
                                                var fabricsWeightParseResult = decimal.TryParse(row["Fabrics Weight (g/m2)"]?.ToString(),
                                                    out decimal fabricsWeight);

                                                if (!fabricsWeightParseResult)
                                                {
                                                    errorMessage = "Invalid fabrics weight input for ex code " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.FabricWeight = fabricsWeight;
                                            }
                                            else
                                            {
                                                partMaterial.FabricWeight = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Fabrics Width (inch)"]?.ToString()))
                                            {
                                                var fabricsWidthParseResult = decimal.TryParse(row["Fabrics Width (inch)"]?.ToString(),
                                                    out decimal fabricsWidth);

                                                if (!fabricsWidthParseResult)
                                                {
                                                    errorMessage = "Invalid fabrics width input for ex code " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.FabricWidth = fabricsWidth;
                                            }
                                            else
                                            {
                                                partMaterial.FabricWidth = 0;
                                            }

                                            if (!string.IsNullOrEmpty(row["Fabrics Width Cut (inch)"]?.ToString()))
                                            {
                                                var cutWidthParseResult = decimal.TryParse(row["Fabrics Width Cut (inch)"]?.ToString(),
                                                    out decimal cutWidth);

                                                if (!cutWidthParseResult)
                                                {
                                                    errorMessage = "Invalid fabrics width input for ex code " + partMaterial.ExternalCode;
                                                    return false;
                                                }

                                                partMaterial.CutWidth = cutWidth;
                                            }
                                            else
                                            {
                                                partMaterial.CutWidth = 0;
                                            }

                                            CreateItem(Username, partRevision, dicItems, ref partMaterial, ref newItems);

                                            partMaterial.SetCreateAudit(Username);

                                            partRevision.PartMaterials.Add(partMaterial);
                                            partRevision.SetCreateAudit(Username);
                                        }
                                    }
                                    break;
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

        public static bool ImportPU(string filePath, string Username, string Filename,
            string customerID,
            string season,
            List<MaterialType> materialTypes,
            IQueryable<Item> items,
            out List<PartRevision> partRevisions,
            out List<PartMaterial> partMaterials,
            out List<Item> newItems,
            out string errorMessage)
        {
            partRevisions = new List<PartRevision>();
            errorMessage = string.Empty;
            newItems = new List<Item>();
            partMaterials = new List<PartMaterial>();
            Dictionary<string, PartRevision> dicPartRevision = new Dictionary<string, PartRevision>();
            var dicItems = items.ToDictionary(x => x.ToSearchKey());

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
                            switch (customerID)
                            {
                                case "PU":
                                    var puDataTable = workSheet.ToDataTable();

                                    foreach (DataRow row in puDataTable.Rows)
                                    {
                                        string marterialSize = row["Material size"]?.ToString().ToUpper().Trim();
                                        string itemColorName = row["Color"]?.ToString().Trim();

                                        if (!String.IsNullOrEmpty(marterialSize) && marterialSize.Equals("*"))
                                        {
                                            continue;
                                        }

                                        if (!String.IsNullOrEmpty(itemColorName) && itemColorName.Equals("*"))
                                        {
                                            continue;
                                        }

                                        var partMaterial = new PartMaterial()
                                        {
                                            ExternalCode = row["Item No"]?.ToString(),
                                            ItemID = row["Material Code"]?.ToString().Trim(),
                                            //ItemName = row["Description"]?.ToString(),
                                            ItemColorCode = row["Material Color"]?.ToString().Trim(),
                                            ItemColorName = itemColorName,
                                            Specify = row["MatrSize_1"]?.ToString(),
                                            //Position = row["Position"]?.ToString(),
                                            Division = row["Division"]?.ToString(),
                                            ItemStyleNumber = row["Style"]?.ToString(),
                                            MaterialTypeClass = row["Matr Class"]?.ToString()?.ToUpper().Trim(),
                                            //MaterialTypeCode = row["Matr Class"]?.ToString().ToUpper().Trim(),
                                            PerUnitID = row["Unit"]?.ToString().ToUpper().Trim(),
                                            PriceUnitID = row["Unit"]?.ToString().ToUpper().Trim(),
                                            CurrencyID = "VND",
                                            VendorID = "PU",
                                            Price = 0,
                                            //GarmentColorCode = row["Garment Color"]?.ToString(),
                                            //LabelCode = row["Label"]?.ToString(),
                                            //GarmentSize = row["Size                    Garment Size"]?.ToString().ToUpper().Trim(),
                                            MaterialSize = row["Material size"]?.ToString().ToUpper().Trim(),
                                            ContractNo = row["Order No"]?.ToString().ToUpper().Trim(),
                                            PartMaterialStatusCode = "3"
                                        };

                                        var garmentSize = row["Size                    Garment Size"]?.ToString().ToUpper().Trim().Split("/");

                                        if (garmentSize != null && garmentSize.Length > 0)
                                        {
                                            partMaterial.GarmentSize = garmentSize[0];
                                        }


                                        var colorGarment = row["Garment Color"]?.ToString()?.Split(" ");
                                        if (colorGarment != null && colorGarment.Count() > 0)
                                        {
                                            partMaterial.GarmentColorCode = colorGarment[0];

                                            if (colorGarment.Count() > 1)
                                            {
                                                partMaterial.GarmentColorName = colorGarment[1];
                                            }
                                        }

                                        if (partMaterial.MaterialTypeClass.Contains("F"))
                                        {
                                            partMaterial.MaterialTypeCode = "FB";
                                        }
                                        else
                                        {
                                            partMaterial.MaterialTypeCode = "AC";
                                        }

                                        if (!string.IsNullOrEmpty(row["Consumption"]?.ToString()))
                                        {
                                            var consumption = decimal.TryParse(row["Consumption"].ToString(),
                                                    out decimal quantityPerUnit);

                                            if (!consumption)
                                            {
                                                errorMessage = "Invalid quantity per unit input for ex code "
                                                    + partMaterial.ExternalCode;
                                                return false;
                                            }

                                            partMaterial.QuantityPerUnit = quantityPerUnit;
                                        }

                                        if (!string.IsNullOrEmpty(row["SizeCon"]?.ToString()))
                                        {
                                            var consumptionSize = decimal.TryParse(row["SizeCon"].ToString(),
                                                    out decimal quantityPerUnitSize);

                                            if (!consumptionSize)
                                            {
                                                errorMessage = "Invalid SizeCon input for ex code "
                                                    + partMaterial.ExternalCode;
                                                return false;
                                            }

                                            partMaterial.MaterialSizeConsumption = quantityPerUnitSize;

                                            if (partMaterial.QuantityPerUnit == null ||
                                                (partMaterial.QuantityPerUnit != null && partMaterial.QuantityPerUnit == 0))
                                            {
                                                partMaterial.QuantityPerUnit = partMaterial.MaterialSizeConsumption;
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(row["Wastage%"]?.ToString()))
                                        {
                                            var wastage = decimal.TryParse(row["Wastage%"].ToString(),
                                                    out decimal wastagePercent);

                                            if (!wastage)
                                            {
                                                errorMessage = "Invalid wastage percent input for ex code "
                                                    + partMaterial.ExternalCode;
                                                return false;
                                            }

                                            partMaterial.WastagePercent = wastagePercent;
                                        }

                                        if (dicPartRevision.TryGetValue(partMaterial.ContractNo, out PartRevision rsPartRevision))
                                        {
                                            CreateItem(Username, rsPartRevision, dicItems, ref partMaterial, ref newItems);
                                        }
                                        else
                                        {
                                            PartRevision partRevision = new PartRevision();
                                            partRevision.Season = season;
                                            partRevision.CustomerID = customerID;
                                            partRevision.PartNumber = partMaterial.ContractNo;
                                            partRevision.RevisionNumber = "v1";
                                            partRevision.EffectDate = DateTime.Now;
                                            partRevision.IsConfirmed = true;

                                            CreateItem(Username, partRevision, dicItems, ref partMaterial, ref newItems);
                                            dicPartRevision[partMaterial.ContractNo] = partRevision;
                                        }

                                        partMaterial.SetCreateAudit(Username);
                                        partMaterials.Add(partMaterial);
                                    }

                                    foreach (var partRevision in dicPartRevision)
                                    {
                                        partRevision.Value.SetCreateAudit(Username);
                                        partRevisions.Add(partRevision.Value);
                                    }
                                    break;
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
        public static void Fail(Exception ex, ExcelWorksheet worksheet,
                    int column,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            if (ex != null && !String.IsNullOrEmpty(ex.Message))
            {
                errorMessage = ex.Message;
            }

            var val = worksheet.Cells[2, column];
            if (val != null && !String.IsNullOrEmpty(val.Text))
            {

                errorMessage += ". Column " + val.Text + " has value incorrect format";
            }
        }


        public static void CreateItem(string Username,
            PartRevision partRevision,
            Dictionary<string, Item> dicItems,
            ref PartMaterial partMaterial, ref List<Item> newItems)
        {

            string keyItem = partMaterial.ToSearchKey(partRevision.CustomerID, partRevision.Season);

            if (!dicItems.TryGetValue(keyItem, out Item rsItem))
            {
                string firstChar = "AC";

                if (partMaterial.MaterialTypeCode.Equals("FB"))
                {
                    firstChar = "FB";
                }

                Item item = new Item();
                item.Code = firstChar
                           + Nanoid.Nanoid.Generate("0123456789", 9);
                item.ID = string.Empty;
                item.Name = partMaterial.ItemName;
                item.Specify = string.Empty;
                item.ColorCode = string.Empty;
                item.ColorName = string.Empty;
                item.CustomerID = string.Empty;
                item.MaterialTypeCode = firstChar;
                item.Season = string.Empty;
                item.SetCreateAudit(Username);
                newItems.Add(item);

                /// Asign code
                partMaterial.ItemCode = item.Code;

                /// Asign new to dic
                dicItems.Add(keyItem, item);
            }
            else
            {
                partMaterial.ItemCode = rsItem.Code;
            }
        }

        public static void MergeItems(string Username,
            IQueryable<PartRevision> partRevisions,
            IQueryable<Item> items,
            out PartMaterial updatePartMaterial, out List<Item> newItems)
        {
            updatePartMaterial = new PartMaterial();
            newItems = new List<Item>();

            foreach (var partRevision in partRevisions)
            {
                var dicItems = items.ToDictionary(x => x.ToSearchKey());


            }
        }
    }
}
