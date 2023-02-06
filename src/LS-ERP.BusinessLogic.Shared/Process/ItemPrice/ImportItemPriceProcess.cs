using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Extensions;
using LS_ERP.Ultilities.Extensions;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class ImportItemPriceProcess
    {
        public static bool Import(string filePath, string fileName, string userName, string CustomerID,
            List<ShippingTerm> shippingTerms, List<Item> items,
            out string errorMessage, out List<ItemPrice> itemPrices, out List<Item> newItems)
        {
            errorMessage = string.Empty;
            itemPrices = new List<ItemPrice>();
            newItems = new List<Item>();
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
                        var dataTable = workSheet.ToDataTable();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            bool parsePriceResult = decimal.TryParse(row["Price"]?.ToString(), out decimal price);
                            bool parseEffectDateResult = DateTime.TryParse(row["Effect Date"]?.ToString(), 
                                out DateTime effectDate);

                            if (!parsePriceResult)
                            {
                                errorMessage = "Invalid price input for item " + row["Item ID"]?.ToString() + " - "
                                    + row["Item Name"]?.ToString();
                                return false;
                            }

                            if (!parseEffectDateResult)
                            {
                                errorMessage = "Invalid effect date input for item " + row["Item ID"]?.ToString() + " - "
                                    + row["Item Name"]?.ToString();
                                return false;
                            }

                            var itemPrice = new ItemPrice()
                            {
                                ItemID = row["Item ID"]?.ToString(),
                                ItemName = row["Item Name"]?.ToString(),
                                ItemColorCode = row["Item Color Code"]?.ToString(),
                                ItemColorName = row["Item Color Name"]?.ToString(),
                                Specify = row["Specify"]?.ToString(),
                                Position = row["Position"]?.ToString(),
                                Season = row["Season"]?.ToString(),
                                LabelCode = row["Label Code"]?.ToString(),
                                PriceUnitID = row["Price Unit"]?.ToString(),
                                VendorID = row["Vendor"]?.ToString(),
                                CurrencyID = row["Currency"]?.ToString(),
                                MaterialTypeCode = row["Material Type"]?.ToString(),
                                CustomerID = CustomerID,
                                ShippingTermCode = row["Shipping"]?.ToString(),
                                EffectDate = effectDate,
                                Price = price
                            };

                            itemPrice.SetCreateAudit(userName);

                            //CreateItem(userName, CustomerID, dicItems,ref itemPrice, ref newItems);

                            itemPrices.Add(itemPrice);
                        }
                    }
                }
            }

            return true;
        }

        #region Support Function
        public static void CreateItem(string Username, string customerID,
            Dictionary<string, Item> dicItems,
            ref ItemPrice itemPrice, ref List<Item> newItems)
        {
            string keyItem = itemPrice.ToSearchKey();

            if (!dicItems.TryGetValue(keyItem, out Item rsItem))
            {
                Item item = new Item();
                if(item.MaterialTypeCode == "FB")
                {
                    item.Code = itemPrice.MaterialTypeCode
                           + Nanoid.Nanoid.Generate("0123456789", 9);
                }
                else
                {
                    item.Code = "AC"
                           + Nanoid.Nanoid.Generate("0123456789", 9);
                }
                item.ID = string.Empty;
                item.Name = itemPrice.ItemName;
                item.Specify = string.Empty;
                item.ColorCode = string.Empty;
                item.ColorName = string.Empty;
                item.CustomerID = string.Empty;
                item.MaterialTypeCode = itemPrice.MaterialTypeCode;
                item.Season = string.Empty;
                item.CustomerID = customerID;
                item.SetCreateAudit(Username);
                newItems.Add(item);

                /// Asign code
                itemPrice.ItemCode = item.Code;

                /// Asign new to dic
                dicItems.Add(keyItem, item);
            }
            else
            {
                itemPrice.ItemCode = rsItem.Code;
            }
        }
        #endregion
    }
}
