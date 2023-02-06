using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class ItemModelProcess
    {
        public static void ImportItemModel(string filePath, string fileName,
            string userName, string customerID, IQueryable<Size> sizes,
            out List<ItemModel> newItemModels,
            out Dictionary<string, Dictionary<string, ItemModel>> LSStyles,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            newItemModels = new List<ItemModel>();
            LSStyles = new Dictionary<string, Dictionary<string, ItemModel>>(); // key1 = lsstyle, key2 = lsstyle+size, value = item model

            if (!string.IsNullOrEmpty(customerID))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                FileInfo fileInfo = new FileInfo(filePath);
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var worksheet = package.Workbook.Worksheets[0];  // sheet 1
                                                                         //int colCount = workSheet.Dimension.End.Column;  //get Column Count
                                                                         //int rowCount = workSheet.Dimension.End.Row;
                        if (customerID.Equals("GA"))
                        {
                            worksheet = package.Workbook.Worksheets["UPC"];
                        }

                        int row = 0;
                        if (worksheet.Dimension != null)
                        {
                            row = worksheet.Dimension.End.Row;
                        }

                        switch (customerID)
                        {
                            case "HA":
                                DataHA(worksheet, customerID, userName, row, sizes, fileInfo, fileName, out errorMessage, out newItemModels);
                                break;
                            case "GA":
                                DataGA(worksheet, customerID, userName, row, sizes, fileInfo, fileName, out errorMessage, out newItemModels);
                                break;
                            case "IFG":
                                DataIFG(worksheet, customerID, userName, row, sizes, fileInfo, fileName, out errorMessage, out newItemModels, out LSStyles);
                                break;
                            default:
                                break;
                        }

                    }
                    else
                    {
                        errorMessage = "Not found sheet";
                    }
                }
            }
            else
            {
                errorMessage = "File has exists in system, please change file another";
            }
        }

        private static void DataGA(ExcelWorksheet worksheet, string customerID, string userName, int row, IQueryable<Size> queryItemSize,
            FileInfo fileInfo, string fileName,
            out string errorMessage, out List<ItemModel> newItemModels)
        {
            errorMessage = string.Empty;
            newItemModels = new List<ItemModel>();

            if (CheckTemplateGA(worksheet, out errorMessage))
            {
                for (int i = 25; i <= row; i++)
                {
                    string size = worksheet.Cells[i, 11].GetValue<string>();

                    CheckSize(queryItemSize, ref size, out errorMessage);

                    ItemModel itemImportUPC = new ItemModel();
                    itemImportUPC.ContractNo = worksheet.Cells[i, 2].GetValue<string>();
                    itemImportUPC.Mfg = worksheet.Cells[i, 3].GetValue<string>();
                    itemImportUPC.SuppPlt = worksheet.Cells[i, 4].GetValue<string>();
                    itemImportUPC.Style = worksheet.Cells[i, 5].GetValue<string>();

                    itemImportUPC.GarmentColorCode = worksheet.Cells[i, 6].GetValue<string>();
                    itemImportUPC.GarmentColorName = worksheet.Cells[i, 7].GetValue<string>();
                    itemImportUPC.Season = worksheet.Cells[i, 8].GetValue<string>();
                    itemImportUPC.ReplCode = worksheet.Cells[i, 9].GetValue<string>();
                    itemImportUPC.DeptSubFineline = worksheet.Cells[i, 10].GetValue<string>();

                    itemImportUPC.GarmentSize = size;
                    itemImportUPC.UPC = worksheet.Cells[i, 12].GetValue<string>();
                    itemImportUPC.FixtureCode = worksheet.Cells[i, 13].GetValue<string>();
                    itemImportUPC.TagSticker = worksheet.Cells[i, 14].GetValue<string>();
                    itemImportUPC.FileName = fileName;
                    itemImportUPC.CustomerID = customerID;
                    itemImportUPC.SetCreateAudit(userName);

                    if (!string.IsNullOrEmpty(itemImportUPC.Style)
                        && !string.IsNullOrEmpty(itemImportUPC.GarmentColorCode)
                        && !string.IsNullOrEmpty(itemImportUPC.GarmentSize))
                    {
                        newItemModels.Add(itemImportUPC);
                    }
                }
            }
        }

        private static void DataHA(ExcelWorksheet worksheet, string customerID, string userName, int row, IQueryable<Size> queryItemSize,
            FileInfo fileInfo, string fileName,
            out string errorMessage, out List<ItemModel> newItemModels)
        {
            errorMessage = string.Empty;
            newItemModels = new List<ItemModel>();

            int template = CheckTemplateHA(worksheet, out errorMessage);
            // read template 1
            if (template == 1)
            {
                for (int i = 2; i <= row; i++)
                {
                    string size = worksheet.Cells[i, 9].GetValue<string>();

                    CheckSizeHA(template, queryItemSize, ref size, out errorMessage);

                    ItemModel itemImportUPC = new ItemModel();
                    itemImportUPC.Style = worksheet.Cells[i, 4].GetValue<string>().Trim();
                    itemImportUPC.GarmentSize = size.Trim();
                    itemImportUPC.GarmentColorCode = worksheet.Cells[i, 5].GetValue<string>().Trim();
                    itemImportUPC.UPC = worksheet.Cells[i, 13].GetValue<string>().Trim();
                    itemImportUPC.FileName = fileName;
                    itemImportUPC.CustomerID = customerID;
                    itemImportUPC.SetCreateAudit(userName);

                    if (!string.IsNullOrEmpty(itemImportUPC.Style)
                        && !string.IsNullOrEmpty(itemImportUPC.GarmentColorCode)
                        && !string.IsNullOrEmpty(itemImportUPC.GarmentSize))
                    {
                        newItemModels.Add(itemImportUPC);
                    }
                }
            }
            else // read template 2
            {
                for (int i = 5; i <= row; i++)
                {
                    string size = worksheet.Cells[i, 4].GetValue<string>();

                    CheckSizeHA(template, queryItemSize, ref size, out errorMessage);

                    ItemModel itemImportUPC = new ItemModel();
                    itemImportUPC.Style = worksheet.Cells[i, 2].GetValue<string>().Trim();
                    itemImportUPC.GarmentSize = size.Trim();
                    itemImportUPC.GarmentColorCode = worksheet.Cells[i, 3].GetValue<string>().Trim();
                    itemImportUPC.UPC = worksheet.Cells[i, 7].GetValue<string>().Trim();
                    itemImportUPC.FileName = fileName;
                    itemImportUPC.CustomerID = customerID;
                    itemImportUPC.SetCreateAudit(userName);

                    if (!string.IsNullOrEmpty(itemImportUPC.Style)
                        && !string.IsNullOrEmpty(itemImportUPC.GarmentColorCode)
                        && !string.IsNullOrEmpty(itemImportUPC.GarmentSize))
                    {
                        newItemModels.Add(itemImportUPC);
                    }
                }
            }
        }

        private static void DataIFG(ExcelWorksheet worksheet, string customerID, string userName, int row, IQueryable<Size> queryItemSize,
            FileInfo fileInfo, string fileName,
            out string errorMessage,
            out List<ItemModel> newItemModels,
            out Dictionary<string, Dictionary<string, ItemModel>> dicLSStyleSizes
            )
        {
            errorMessage = string.Empty;
            newItemModels = new List<ItemModel>();
            dicLSStyleSizes = new Dictionary<string, Dictionary<string, ItemModel>>();

            if (CheckTemplateIFG(worksheet, out errorMessage))
            {
                for (int i = 2; i <= row; i++)
                {
                    string size = worksheet.Cells[i, 6].GetValue<string>();

                    CheckSize(queryItemSize, ref size, out errorMessage);

                    ItemModel itemImportUPC = new ItemModel();
                    itemImportUPC.Style = worksheet.Cells[i, 1].GetValue<string>()?.Trim();
                    //itemImportUPC.StyleDescription = worksheet.Cells[i, 2].GetValue<string>();
                    itemImportUPC.GarmentColorName = worksheet.Cells[i, 3].GetValue<string>()?.Trim();
                    itemImportUPC.CustomerColorCode = worksheet.Cells[i, 4].GetValue<string>()?.Trim();
                    itemImportUPC.Label = worksheet.Cells[i, 5].GetValue<string>()?.Trim();

                    itemImportUPC.GarmentSize = size?.Trim();
                    itemImportUPC.UPC = worksheet.Cells[i, 7].GetValue<string>()?.Trim();
                    itemImportUPC.MSRP = worksheet.Cells[i, 8].GetValue<decimal>();
                    itemImportUPC.GarmentColorCode = worksheet.Cells[i, 9].GetValue<string>()?.Trim();
                    itemImportUPC.ContractNo = worksheet.Cells[i, 10].GetValue<string>()?.Trim(); //J
                    itemImportUPC.Barcode = worksheet.Cells[i, 11].GetValue<string>()?.Trim();//K
                    itemImportUPC.LSStyle = worksheet.Cells[i, 12].GetValue<string>()?.Trim();//L
                    itemImportUPC.Season = worksheet.Cells[i, 13].GetValue<string>()?.Trim();//M
                    itemImportUPC.FileName = fileName;
                    itemImportUPC.CustomerID = customerID;
                    itemImportUPC.SetCreateAudit(userName);

                    if (!string.IsNullOrEmpty(itemImportUPC.Style)
                         && !string.IsNullOrEmpty(itemImportUPC.GarmentColorCode)
                         && !string.IsNullOrEmpty(itemImportUPC.GarmentSize))
                    {
                        newItemModels.Add(itemImportUPC);
                    }

                    if (!string.IsNullOrEmpty(itemImportUPC.LSStyle))
                    {
                        var lsstyles = itemImportUPC.LSStyle.Split(';');

                        foreach (var lsstyle in lsstyles)
                        {
                            if (dicLSStyleSizes.TryGetValue(lsstyle, out Dictionary<string, ItemModel> rsDicBarcodes)){

                                //Dictionary<string, ItemModel> dicNewObject = new Dictionary<string, ItemModel>();

                                rsDicBarcodes[lsstyle + size] = itemImportUPC;

                                dicLSStyleSizes[lsstyle] = rsDicBarcodes;
                            }
                            else
                            {
                                Dictionary<string, ItemModel> dicNewObject = new Dictionary<string, ItemModel>();

                                dicNewObject[lsstyle + size] = itemImportUPC;

                                dicLSStyleSizes[lsstyle] = dicNewObject;
                            }
                        }
                    }
                }
            }
        }

        private static void CheckSizeHA(int template, IQueryable<Size> listSize, ref string size, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(size))
            {
                if (size.Contains("NW"))
                {
                    size = size.Substring(2);
                }
                else
                {
                    string[] arrSize = size.Split(' ');
                    if (arrSize.Count() > 1)
                    {
                        size = arrSize[1];
                        switch (size)
                        {
                            case "SM":
                                size = "S";
                                break;
                            case "MED":
                                size = "M";
                                break;
                            case "LRG":
                                size = "L";
                                break;
                            case "XLG":
                                size = "XL";
                                break;
                        }
                    }
                    else
                    {
                        if (template == 1)
                        {
                            if (size.Equals("ASSTD"))
                            {
                                size = "";
                            }
                        }
                        else
                        {
                            size = arrSize[0];
                        }

                    }
                }
            }

            if (!String.IsNullOrEmpty(size))
            {
                string newSize = size;
                var checkSize = listSize.ToList().FirstOrDefault(x => x.Code.Equals(newSize));

                if (checkSize == null)
                {
                    errorMessage = "Size " + size + " not found in system.";
                }
            }
        }

        private static void CheckSize(IQueryable<Size> listSize, ref string size, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!String.IsNullOrEmpty(size))
            {
                string newSize = size;
                var checkSize = listSize.ToList().FirstOrDefault(x => x.Code.Equals(newSize));

                if (checkSize == null)
                {
                    errorMessage = "Size " + size + " not found in system.";
                }
            }
        }

        /// <summary>
        /// check template excel of Haddad
        /// </summary>
        /// <param name="worksheet"></param>
        /// <returns>1 if template is 1</returns>
        /// <returns>2 if template is 2</returns>
        private static int CheckTemplateHA(ExcelWorksheet worksheet, out string errorMessage)
        {
            errorMessage = string.Empty;
            string template1_config = "Send To Catalog;Send To Catalog OVR;Division;Style;Color;Label;Request;Size;Size Description;Catalogs;Haddad UPC;NRF Size;NRF Color;Color Name;Levi's UPC;Status";
            string template2_config = "PO#;Style;Color;Size;Color Desc;Retail Price;UPC";


            string[] template1 = template1_config.Split(';');
            string[] template2 = template2_config.Split(';');
            bool check = true;
            //string columnError = "";
            int column = 1;

            // check template 1
            for (int i = 0; i < template1.Count(); i++)
            {
                var val = worksheet.Cells[1, column++];
                if (val != null && !val.Text.ToUpper().Replace(" ", "").Trim().Contains(template1[i].ToUpper().Replace(" ", "").Trim()))
                {
                    //columnError = val.Text;
                    check = false;
                    break;
                }

                if (val != null && "Catalogs".ToUpper().Replace(" ", "").Trim().Equals(template1[i].ToUpper().Replace(" ", "").Trim()))
                {
                    column += 2;
                }
            }

            // check template 2
            if (!check)
            {
                column = 1;
                for (int i = 0; i < template2.Count(); i++)
                {
                    var val = worksheet.Cells[4, column++];
                    if (val != null && !val.Text.ToUpper().Replace(" ", "").Trim().Contains(template2[i].ToUpper().Replace(" ", "").Trim()))
                    {
                        errorMessage = "Column " + template2[i] + " incorrect template";
                    }
                }
            }
            else
            {
                return 1;
            }

            return 2;
        }

        private static bool CheckTemplateGA(ExcelWorksheet worksheet, out string errorMessage)
        {
            errorMessage = string.Empty;
            string template_config = "Walmart Style #;Mfg;Sup Plt;Garan Style #;Color #;Color Desc.;Season Code;Repl Code;Dept/Sub Fineline;Size;UPC Code;Fixture Code;Tag-Sticker";


            string[] template = template_config.Split(';');
            bool check = true;
            int column = 2;

            // check template 1
            for (int i = 0; i < template.Count(); i++)
            {
                var val = worksheet.Cells[24, column++];
                if (val != null && !val.Text.ToUpper().Replace(" ", "").Trim().Contains(template[i].ToUpper().Replace(" ", "").Trim()))
                {
                    //columnError = val.Text;
                    check = false;
                    errorMessage = "Column " + template[i] + " incorrect template";

                }
            }

            return check;
        }

        private static bool CheckTemplateIFG(ExcelWorksheet worksheet, out string errorMessage)
        {
            errorMessage = string.Empty;
            string template_config = "Style; Style Description; Color; Code; LABEL; Size; UPC; MSRP; Garment Color; Master Style; With check digit; LSStyle; Season ";


            string[] template = template_config.Split(';');
            bool check = true;
            int column = 1;

            // check template 1
            for (int i = 0; i < template.Count(); i++)
            {
                var val = worksheet.Cells[1, column++];
                if (val != null && !val.Text.ToUpper().Replace(" ", "").Trim().Contains(template[i].ToUpper().Replace(" ", "").Trim()))
                {
                    //columnError = val.Text;
                    check = false;
                    errorMessage = "Column " + template[i] + " incorrect template";

                }
            }

            return check;
        }
    }
}
