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
    public class ImportPartProcess
    {
        public static void Import(string filePath, string userName,
            List<ItemStyle> oldItemStyles,
            bool update,
            ref List<Part> oldPart,
            out List<Part> newParts,
            out List<Part> deleteParts,
            out List<ItemStyle> updateItemStyles,
            out List<ItemStyle> newGenItemStyles,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            newParts = new List<Part>();
            deleteParts = new List<Part>();
            updateItemStyles = new List<ItemStyle>();
            newGenItemStyles = new List<ItemStyle>();
            var dicPart = oldPart.Where(x => x.ContractNo != null).ToDictionary(x => x.ContractNo + x.GarmentColorCode);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        Dictionary<string, ItemStyle> oldItemStyleProduction = new Dictionary<string, ItemStyle>();
                        Dictionary<string, ItemStyle> dicOneItemStyleProductions = new Dictionary<string, ItemStyle>();
                        Dictionary<string, List<ItemStyle>> dicMultiItemStyleProductions = new Dictionary<string, List<ItemStyle>>();
                        Dictionary<string, List<ItemStyle>> dicAllItemStyles = new Dictionary<string, List<ItemStyle>>();

                        for (int i = 0; i < package.Workbook.Worksheets.Count; i++)
                        {
                            //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>. " + package.Workbook.Worksheets[i].Name);
                            var productionItemStyles = package.Workbook.Worksheets[i].ImportProductionItemStyle_HA(
                                ref dicAllItemStyles,
                                out errorMessage);

                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                return;
                            }

                            foreach (var productionItem in productionItemStyles)
                            {
                                if (!oldItemStyleProduction.TryGetValue(productionItem.LSStyle, out ItemStyle rsItemStyle))
                                {
                                    oldItemStyleProduction[productionItem.LSStyle] = productionItem;
                                }
                            }
                        }

                        foreach (var item in dicAllItemStyles)
                        {
                            if (item.Value.Count() > 1)
                            {
                                dicMultiItemStyleProductions[item.Key] = item.Value;
                            }
                            else if (item.Value.Count() == 1)
                            {
                                dicOneItemStyleProductions[item.Key] = item.Value[0];
                            }
                        }

                        if (!update)
                        {
                            Dictionary<string, Part> dicNewPartItemStyleProduction = new Dictionary<string, Part>();

                            foreach (var itemStyle in oldItemStyleProduction)
                            {
                                var subFix = itemStyle.Value.LSStyle.Split("-");
                                if (subFix != null && subFix.Length > 1)
                                {
                                    //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>. " + itemStyle.Value.LSStyle);
                                    Part newPart = new Part();
                                    //newPart.ID = string.Empty;
                                    newPart.ContractNo = itemStyle.Value.ContractNo;
                                    newPart.CustomerID = "HA";
                                    newPart.Character = subFix[1].Substring(0, 1);
                                    newPart.LastSequenceNumber = int.Parse(subFix[1].Substring(1));
                                    newPart.GarmentColorCode = itemStyle.Value.ColorCode;
                                    newPart.GarmentColorName = itemStyle.Value.ColorName;

                                    string key = newPart.ContractNo + newPart.GarmentColorCode;

                                    if (dicNewPartItemStyleProduction.TryGetValue(key, out Part rsPart))
                                    {
                                        if (rsPart.LastSequenceNumber < newPart.LastSequenceNumber)
                                        {
                                            rsPart.LastSequenceNumber = newPart.LastSequenceNumber;
                                            dicNewPartItemStyleProduction[key] = rsPart;
                                        }
                                    }
                                    else
                                    {
                                        dicNewPartItemStyleProduction[key] = newPart;
                                    }
                                }
                            }

                            foreach (var newPart in dicNewPartItemStyleProduction)
                            {
                                newPart.Value.LastSequenceNumber += 1;
                                newParts.Add(newPart.Value);

                                var key = newPart.Value.ContractNo + newPart.Value.GarmentColorCode;

                                if (dicPart.TryGetValue(key, out Part rsValue))
                                {
                                    deleteParts.Add(rsValue);
                                }
                            }
                        }
                        else
                        {
                            /// update LSStyle
                            UpdateLSStyle(userName, oldItemStyles, dicOneItemStyleProductions, dicMultiItemStyleProductions,
                                out updateItemStyles,
                                out newGenItemStyles);

                            EPPlusExtensions.GenerateLSStyleNonSeason("HA", userName, ref oldPart, newGenItemStyles, out newParts, out errorMessage);
                        }

                    }
                    else
                    {
                        errorMessage = "Not found sheet";
                    }
                }

            }

        }

        private static void UpdateLSStyle(string userName, List<ItemStyle> oldItemStyles,
            Dictionary<string, ItemStyle> dicOneItemStyleProductions,
            Dictionary<string, List<ItemStyle>> dicMultiItemStyleProductions,
            out List<ItemStyle> updateItemStyles,
            out List<ItemStyle> newGenItemStyles)
        {
            updateItemStyles = new List<ItemStyle>();
            newGenItemStyles = new List<ItemStyle>();
            Dictionary<string, ItemStyle> dicOneOldItemStyles = new Dictionary<string, ItemStyle>();
            Dictionary<string, List<ItemStyle>> dicMultiOldItemStyles = new Dictionary<string, List<ItemStyle>>();
            Dictionary<string, List<ItemStyle>> dicAllOldItemStyles = new Dictionary<string, List<ItemStyle>>();

            foreach (var itemStyle in oldItemStyles)
            {
                var keyItemStyle = itemStyle.PurchaseOrderNumber;
                if (dicAllOldItemStyles.TryGetValue(keyItemStyle, out List<ItemStyle> rsOldItemStyle))
                {
                    rsOldItemStyle.Add(itemStyle);
                    dicAllOldItemStyles[keyItemStyle] = rsOldItemStyle;
                }
                else
                {
                    List<ItemStyle> itemStyles = new List<ItemStyle>();
                    itemStyles.Add(itemStyle);
                    dicAllOldItemStyles[keyItemStyle] = itemStyles;
                }
            }

            foreach (var item in dicAllOldItemStyles)
            {
                if (item.Value.Count() > 1)
                {
                    dicMultiOldItemStyles[item.Key] = item.Value;
                }
                else if (item.Value.Count() == 1)
                {
                    dicOneOldItemStyles[item.Key] = item.Value[0];
                }
            }

            foreach (var item in dicOneOldItemStyles)
            {

                if (dicOneItemStyleProductions.TryGetValue(item.Key, out ItemStyle rsItemStyle))
                {
                    item.Value.OldLSStyle = item.Value.LSStyle;
                    item.Value.LSStyle = rsItemStyle.LSStyle;
                    item.Value.SetUpdateAudit(userName);
                    item.Value.ItemStyleStatusCode = "2";
                    updateItemStyles.Add(item.Value);
                }
                else if (dicMultiItemStyleProductions.TryGetValue(item.Key, out List<ItemStyle> rsItemStyleProductions))
                {
                    var keyOldItemStyleQty = item.Value.PurchaseOrderNumber + item.Value.TotalQuantity;
                    var keyOldItemStyleShipDate = item.Value.PurchaseOrderNumber + item.Value.ShipDate?.ToString("yyyy/MM/dd");
                    bool checkUpdate = false;

                    foreach (var itemStyle in rsItemStyleProductions)
                    {
                        var keyItemStyleProductionsQty = itemStyle.PurchaseOrderNumber + itemStyle.TotalQuantity;
                        var keyItemStyleProductionsShipDate = itemStyle.PurchaseOrderNumber + itemStyle.ShipDate?.ToString("yyyy/MM/dd");

                        if (keyOldItemStyleQty == keyItemStyleProductionsQty)
                        {
                            item.Value.OldLSStyle = item.Value.LSStyle;
                            item.Value.LSStyle = itemStyle.LSStyle; ;
                            item.Value.SetUpdateAudit(userName);
                            item.Value.ItemStyleStatusCode = "2";
                            updateItemStyles.Add(item.Value);
                            checkUpdate = true;
                        }
                        else if (keyOldItemStyleShipDate == keyItemStyleProductionsShipDate)
                        {
                            item.Value.OldLSStyle = item.Value.LSStyle;
                            item.Value.LSStyle = itemStyle.LSStyle; ;
                            item.Value.SetUpdateAudit(userName);
                            item.Value.ItemStyleStatusCode = "2";
                            updateItemStyles.Add(item.Value);
                            checkUpdate = true;
                        }
                    }

                    if (!checkUpdate)
                    {
                        var shortItemStyles = rsItemStyleProductions.OrderBy(x => x.LSStyle);

                        item.Value.OldLSStyle = item.Value.LSStyle;
                        item.Value.LSStyle = shortItemStyles.First().LSStyle; ;
                        item.Value.SetUpdateAudit(userName);
                        item.Value.ItemStyleStatusCode = "2";
                        updateItemStyles.Add(item.Value);
                    }
                }
                else
                {


                    item.Value.OldLSStyle = item.Value.LSStyle;
                    item.Value.LSStyle = String.Empty;
                    newGenItemStyles.Add(item.Value);
                }
            }

            Dictionary<string, bool> dicCheckDupLSStyle = new Dictionary<string, bool>();

            foreach (var dicItem in dicMultiOldItemStyles)
            {
                if (dicMultiItemStyleProductions.TryGetValue(dicItem.Key, out List<ItemStyle> rsItemStyleProductions))
                {
                    for (int i = 0; i < dicItem.Value.Count; i++)
                    {
                        var keyOldItemStyleQty = dicItem.Value[i].PurchaseOrderNumber + dicItem.Value[i].TotalQuantity;
                        var keyOldItemStyleShipDate = dicItem.Value[i].PurchaseOrderNumber + dicItem.Value[i].ShipDate?.ToString("yyyy/MM/dd");
                        bool checkUpdate = false;
                        foreach (var itemPro in rsItemStyleProductions)
                        {
                            var keyItemStyleProductionQty = itemPro.PurchaseOrderNumber + itemPro.TotalQuantity;
                            var keyItemStyleProductionShipDate = itemPro.PurchaseOrderNumber + itemPro.ShipDate?.ToString("yyyy/MM/dd");

                            if (keyItemStyleProductionQty == keyOldItemStyleQty)
                            {
                                if (!dicCheckDupLSStyle.ContainsKey(itemPro.LSStyle))
                                {
                                    dicItem.Value[i].OldLSStyle = dicItem.Value[i].LSStyle;
                                    dicItem.Value[i].LSStyle = itemPro.LSStyle;
                                    dicItem.Value[i].PurchaseOrderNumberIndex = i;
                                    dicItem.Value[i].SetUpdateAudit(userName);
                                    dicItem.Value[i].ItemStyleStatusCode = "2";
                                    dicCheckDupLSStyle[itemPro.LSStyle] = true;

                                    updateItemStyles.Add(dicItem.Value[i]);
                                    checkUpdate = true;
                                    break;
                                }
                            }
                            else if (keyItemStyleProductionShipDate == keyOldItemStyleShipDate)
                            {
                                if (!dicCheckDupLSStyle.ContainsKey(itemPro.LSStyle))
                                {
                                    dicItem.Value[i].OldLSStyle = dicItem.Value[i].LSStyle;
                                    dicItem.Value[i].LSStyle = itemPro.LSStyle;
                                    dicItem.Value[i].PurchaseOrderNumberIndex = i;
                                    dicItem.Value[i].SetUpdateAudit(userName);
                                    dicItem.Value[i].ItemStyleStatusCode = "2";
                                    dicCheckDupLSStyle[itemPro.LSStyle] = true;

                                    updateItemStyles.Add(dicItem.Value[i]);
                                    checkUpdate = true;
                                    break;
                                }
                            }

                        }

                        if (!checkUpdate)
                        {
                            foreach (var itemPro in rsItemStyleProductions)
                            {
                                if (!dicCheckDupLSStyle.ContainsKey(itemPro.LSStyle))
                                {
                                    dicItem.Value[i].OldLSStyle = dicItem.Value[i].LSStyle;
                                    dicItem.Value[i].LSStyle = itemPro.LSStyle;
                                    dicItem.Value[i].PurchaseOrderNumberIndex = i;
                                    dicItem.Value[i].SetUpdateAudit(userName);
                                    dicItem.Value[i].ItemStyleStatusCode = "2";
                                    dicCheckDupLSStyle[itemPro.LSStyle] = true;

                                    updateItemStyles.Add(dicItem.Value[i]);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (dicOneItemStyleProductions.TryGetValue(dicItem.Key, out ItemStyle rsItemStyle))
                {
                    foreach (var item in dicItem.Value)
                    {
                        item.OldLSStyle = item.LSStyle;
                        item.LSStyle = rsItemStyle.LSStyle;
                        item.SetUpdateAudit(userName);
                        item.ItemStyleStatusCode = "2";
                        updateItemStyles.Add(item);
                    }
                }
                else
                {
                    foreach (var item in dicItem.Value)
                    {
                        item.OldLSStyle = item.LSStyle;
                        item.LSStyle = String.Empty;
                        newGenItemStyles.Add(item);
                    }
                }
            }
        }
    }
}
