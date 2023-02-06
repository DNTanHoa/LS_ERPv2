using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class InvoiceProcessor
    {
        public static void CreateOrUpdateInvoiceDetail(Invoice invoice, List<PackingList> packingLists, string userName,
            Unit ifgUnit, List<PartPrice> partPrices, List<SalesOrder> salesOrders, out string errorMessage)
        {
            errorMessage = String.Empty;

            if (packingLists.Any())
            {

                if (invoice.PackingList == null)
                {
                    invoice.PackingList = new List<PackingList>();
                }

                if (invoice.InvoiceDetails == null)
                {
                    invoice.InvoiceDetails = new List<InvoiceDetail>();
                }

                var dicPLCode = invoice.PackingList.ToDictionary(x => x.PackingListCode);

                foreach (var item in invoice.PackingList)
                {
                    if (!dicPLCode.TryGetValue(item.PackingListCode, out PackingList rsPL))
                    {
                        packingLists.Add(item);
                    }
                }

                switch (invoice.Customer?.ID)
                {
                    case "IFG":
                        {
                            InvoiceProcessor.CreateInvoice_IFG(invoice, packingLists.ToList(), userName, ifgUnit);
                        }
                        break;
                    case "GA":
                        {
                            InvoiceProcessor.CreateInvoice_GA(invoice, packingLists.ToList(), userName,
                                out errorMessage);

                        }
                        break;
                    case "DE":
                        {
                            InvoiceProcessor.CreateInvoice_DE(invoice, packingLists.ToList(), partPrices, salesOrders, userName,
                                out errorMessage);

                        }
                        break;
                    case "PU":
                        {
                            InvoiceProcessor.CreateInvoice_PU(invoice, packingLists.ToList(), partPrices, userName,
                                out errorMessage);

                        }
                        break;
                    default:
                        {
                            //CreateInvoiceProcessor.CreateInvoice_IFG(viewObject, packingLists, SecuritySystem.CurrentUserName);
                        }
                        break;
                }
            }
        }

        public static void DeleteInvoiceDetail(Invoice invoice, List<PackingList> deletePackingLists, string userName,
            out string errorMessage)
        {
            errorMessage = String.Empty;

            if (deletePackingLists.Any())
            {
                switch (invoice.Customer?.ID)
                {
                    case "GA":
                        {
                            InvoiceProcessor.DeleteInvoice_GA(invoice, deletePackingLists.ToList(), userName,
                                out errorMessage);
                        }
                        break;
                }
            }
        }
        public static void CreateInvoice_IFG(Invoice invoice, List<PackingList> packingList, string userName, Unit ifgUnit)
        {
            Dictionary<string, InvoiceDetail> dicInvoiceDetail = new Dictionary<string, InvoiceDetail>();
            Dictionary<string, Dictionary<string, int>> dicSizeMin = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, Dictionary<string, int>> dicSizeMax = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, int> dicSize = new Dictionary<string, int>();
            var currentInvoiceDetail = invoice?.InvoiceDetails
                .ToDictionary(x => x.CustomerStyle + x.CustomerPurchaseOrderNumber + x.GarmentColorCode);

            foreach (var packing in packingList)
            {
                foreach (var itemStyle in packing.ItemStyles)
                {
                    string key = string.Empty;
                    //decimal? price = 0;
                    decimal? quantity = 0;
                    decimal? amount = 0;
                    foreach(var line in packing?.PackingLines?.
                        Where(x => x.LSStyle == itemStyle.LSStyle))
                    {
                        var sizeQty = line.QuantitySize * line.TotalCarton;
                        amount += sizeQty * itemStyle?.OrderDetails?.
                            Where(o => o.ItemStyleNumber == itemStyle.Number &&
                                    o.Size == line.Size)?.FirstOrDefault()?.Price;
                        quantity += sizeQty;

                    }
                    //itemStyle.OrderDetails.FirstOrDefault().Price;
                    var sizes = packing.PackingLines.OrderBy(x => x.SequenceNo).Select(x => x.Size).ToList();

                    key = itemStyle.CustomerStyle + itemStyle.PurchaseOrderNumber + itemStyle.ColorCode;

                    InvoiceDetail invoiceDetail = new InvoiceDetail();
                    invoiceDetail.GarmentColorCode = itemStyle.ColorCode;
                    invoiceDetail.GarmentColorName = itemStyle.ColorName;
                    invoiceDetail.CustomerStyle = itemStyle.CustomerStyle;
                    invoiceDetail.CustomerPurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                    invoiceDetail.Description = itemStyle.Description;
                    invoiceDetail.UnitID = ifgUnit?.ID ?? "DZ";
                    invoiceDetail.Quantity = quantity / (ifgUnit?.Factor ?? 12);
                    invoiceDetail.Amount = amount;
                    invoiceDetail.UnitPrice = invoiceDetail.Amount / invoiceDetail.Quantity;
                    invoiceDetail.GarmentSize = JoinSizeIFG(key, itemStyle, packing, ref dicSizeMin, ref dicSizeMax, ref dicSize);

                    if (currentInvoiceDetail.TryGetValue(key, out InvoiceDetail rsInvoiceDetail))
                    {
                        rsInvoiceDetail.Quantity += invoiceDetail.Quantity;
                        rsInvoiceDetail.Amount += invoiceDetail.Amount;
                        rsInvoiceDetail.UnitPrice = rsInvoiceDetail.Amount / rsInvoiceDetail.Quantity;
                        rsInvoiceDetail.SetUpdateAudit(userName);
                    }
                    else
                    {
                        //invoiceDetail.Amount = invoiceDetail.Quantity * invoiceDetail.UnitPrice;
                        invoiceDetail.SetCreateAudit(userName);
                        dicInvoiceDetail[key] = invoiceDetail;
                    }
                }
                invoice.PackingList.Add(packing);
            }

            invoice.SetCreateAudit(userName);
            foreach (var invoiceDetail in dicInvoiceDetail)
            {
                invoice.InvoiceDetails.Add(invoiceDetail.Value);
            }
        }

        public static void CreateInvoice_GA(Invoice invoice, List<PackingList> packingList, string userName,
            out string errorMessage)
        {
            Dictionary<string, InvoiceDetail> dicInvoiceDetail = new Dictionary<string, InvoiceDetail>();
            Dictionary<string, ItemStyle> dicItemStyle = new Dictionary<string, ItemStyle>();
            Dictionary<string, int> dicSize = new Dictionary<string, int>();
            errorMessage = string.Empty;

            try
            {
                dicInvoiceDetail = invoice.InvoiceDetails?.ToDictionary(x => x.CustomerStyle + x.CustomerPurchaseOrderNumber + x.GarmentColorCode);

                if (dicInvoiceDetail == null)
                {
                    dicInvoiceDetail = new Dictionary<string, InvoiceDetail>();
                }

                foreach (var packing in packingList)
                {
                    foreach (var itemStyle in packing.ItemStyles)
                    {
                        if (!dicItemStyle.ContainsKey(itemStyle.LSStyle))
                        {
                            dicItemStyle[itemStyle.LSStyle] = itemStyle;
                        }
                    }

                    foreach (var packingLine in packing.PackingLines)
                    {
                        if (dicItemStyle.TryGetValue(packingLine.LSStyle, out ItemStyle rsItemStyle))
                        {
                            string key = string.Empty;
                            var price = rsItemStyle.OrderDetails.FirstOrDefault().Price;

                            //key = rsItemStyle.CustomerStyle + rsItemStyle.PurchaseOrderNumber + rsItemStyle.ColorCode;

                            InvoiceDetail invoiceDetail = new InvoiceDetail();
                            invoiceDetail.GarmentColorCode = rsItemStyle.ColorCode;
                            invoiceDetail.GarmentColorName = rsItemStyle.ColorName;
                            invoiceDetail.CustomerStyle = rsItemStyle.CustomerStyle;
                            invoiceDetail.CustomerPurchaseOrderNumber = rsItemStyle.PurchaseOrderNumber;
                            invoiceDetail.Description = rsItemStyle.Description;
                            invoiceDetail.UnitID = rsItemStyle.UnitID;
                            invoiceDetail.UnitPrice = price;
                            invoiceDetail.Quantity = packingLine.QuantitySize * packingLine.TotalCarton;
                            invoiceDetail.Amount = invoiceDetail.Quantity * invoiceDetail.UnitPrice;
                            if (dicInvoiceDetail != null && dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out InvoiceDetail rsInvoiceDetail))
                            {
                                rsInvoiceDetail.Quantity += invoiceDetail.Quantity;
                                rsInvoiceDetail.Amount += invoiceDetail.Amount;
                                rsInvoiceDetail.SetUpdateAudit(userName);
                            }
                            else
                            {
                                invoiceDetail.SetCreateAudit(userName);
                                dicInvoiceDetail[invoiceDetail.KeyInvoice()] = invoiceDetail;
                            }
                        }
                    }

                    invoice.PackingList.Add(packing);
                }

                invoice.SetCreateAudit(userName);

                Dictionary<string, bool> dicDuplicateInvoiceDetail = new Dictionary<string, bool>();

                if (invoice.InvoiceDetails != null)
                {
                    foreach (var invoiceDetail in invoice.InvoiceDetails)
                    {
                        if (dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out InvoiceDetail rsInvoiceDetail))
                        {
                            invoiceDetail.Quantity = rsInvoiceDetail.Quantity;
                            dicDuplicateInvoiceDetail[invoiceDetail.KeyInvoice()] = true;
                        }
                    }
                }

                foreach (var keyValuePair in dicInvoiceDetail)
                {
                    if (!dicDuplicateInvoiceDetail.ContainsKey(keyValuePair.Value.KeyInvoice()))
                    {
                        invoice.InvoiceDetails.Add(keyValuePair.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " !!! Please check invoice details";
            }
        }

        public static void CreateInvoice_DE(Invoice invoice, List<PackingList> packingList, 
            List<PartPrice> partPrices, List<SalesOrder> salesOrders, string userName, out string errorMessage)
        {
            Dictionary<string, InvoiceDetail> dicInvoiceDetail = new Dictionary<string, InvoiceDetail>();
            Dictionary<string, ItemStyle> dicItemStyle = new Dictionary<string, ItemStyle>();
            Dictionary<string, int> dicSize = new Dictionary<string, int>();
            errorMessage = string.Empty;

            try
            {
                dicInvoiceDetail = invoice.InvoiceDetails?.ToDictionary(x => x.CustomerStyle + x.CustomerPurchaseOrderNumber + x.GarmentColorCode);

                if (dicInvoiceDetail == null)
                {
                    dicInvoiceDetail = new Dictionary<string, InvoiceDetail>();
                }

                foreach (var packing in packingList)
                {
                    foreach (var itemStyle in packing?.ItemStyles)
                    {
                        var priceTermCode = salesOrders?.FirstOrDefault(s => s.ID == itemStyle?.SalesOrderID)?.PriceTermCode;
                        decimal priceCM = 0;
                        decimal priceFOB = 0;
                        decimal otherPrice = itemStyle?.OrderDetails?.FirstOrDefault()?.OtherPrice ?? 0;
                        if(otherPrice == 0)
                        {
                            var prices = partPrices.Where(x => x.Season == itemStyle.Season &&
                                                    x.GarmentColorCode == itemStyle.ColorCode)?.ToList();

                            priceCM = prices?.Where(x => x.ProductionType == "CMT")
                                        ?.OrderByDescending(x => x.EffectiveDate)?.FirstOrDefault()?.Price ?? 0;
                            priceFOB = prices?.Where(x => x.ProductionType == "FOB")
                                        ?.OrderByDescending(x => x.EffectiveDate)?.FirstOrDefault()?.Price ?? 0;
                        }
                        else
                        {
                            if(priceTermCode?.Trim()?.ToUpper() == "FOB")
                            {
                                priceFOB = itemStyle?.OrderDetails?.FirstOrDefault()?.Price ?? 0;
                                priceCM = itemStyle?.OrderDetails?.FirstOrDefault()?.OtherPrice ?? 0;
                            }
                            else
                            {
                                priceCM = itemStyle?.OrderDetails?.FirstOrDefault()?.Price ?? 0;
                                priceFOB = itemStyle?.OrderDetails?.FirstOrDefault()?.OtherPrice ?? 0;
                            }
                                    
                        }

                        var lines = packing?.PackingLines.Where(x => x.LSStyle == itemStyle.LSStyle).ToList();
                        //key = rsItemStyle.CustomerStyle + rsItemStyle.PurchaseOrderNumber + rsItemStyle.ColorCode;

                        InvoiceDetail invoiceDetail = new InvoiceDetail();
                        invoiceDetail.GarmentColorCode = itemStyle.ColorCode;
                        invoiceDetail.GarmentColorName = itemStyle.ColorName;
                        invoiceDetail.CustomerStyle = itemStyle.CustomerStyle;
                        invoiceDetail.CustomerPurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                        invoiceDetail.Description = itemStyle.Description;
                        invoiceDetail.UnitID = itemStyle.UnitID;
                        invoiceDetail.PriceCM = priceCM;
                        invoiceDetail.PriceFOB = priceFOB;
                        invoiceDetail.Quantity = lines.Sum(x => x.TotalQuantity);
                        if(priceTermCode?.Trim()?.ToUpper() == "FOB")
                        {
                            invoiceDetail.Amount = invoiceDetail.PriceFOB != null ?
                                                    invoiceDetail.Quantity * invoiceDetail.PriceFOB : null;
                        }
                        else
                        {
                            invoiceDetail.Amount = invoiceDetail.PriceCM != null ?
                                                    invoiceDetail.Quantity * invoiceDetail.PriceCM : null;
                        }
                        
                        if (dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out InvoiceDetail rsInvoiceDetail))
                        {
                            rsInvoiceDetail.Quantity += invoiceDetail.Quantity;
                            rsInvoiceDetail.Amount += invoiceDetail.Amount != null ? invoiceDetail.Amount : 0;
                            rsInvoiceDetail.SetUpdateAudit(userName);
                        }
                        else
                        {
                            invoiceDetail.SetCreateAudit(userName);
                            dicInvoiceDetail[invoiceDetail.KeyInvoice()] = invoiceDetail;
                        }
                    }

                    invoice.PackingList.Add(packing);
                }

                invoice.SetCreateAudit(userName);

                Dictionary<string, bool> dicDuplicateInvoiceDetail = new Dictionary<string, bool>();

                foreach (var invoiceDetail in invoice.InvoiceDetails)
                {
                    if (dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out InvoiceDetail rsInvoiceDetail))
                    {
                        invoiceDetail.Quantity = rsInvoiceDetail.Quantity;
                        dicDuplicateInvoiceDetail[invoiceDetail.KeyInvoice()] = true;
                    }
                }

                foreach (var keyValuePair in dicInvoiceDetail)
                {
                    if (!dicDuplicateInvoiceDetail.ContainsKey(keyValuePair.Value.KeyInvoice()))
                    {
                        invoice.InvoiceDetails.Add(keyValuePair.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " !!! Please check invoice details";
            }
        }

        public static void CreateInvoice_PU(Invoice invoice, List<PackingList> packingList, 
            List<PartPrice> partPrices, string userName, out string errorMessage)
        {
            Dictionary<string, InvoiceDetail> dicInvoiceDetail = new Dictionary<string, InvoiceDetail>();
            Dictionary<string, ItemStyle> dicItemStyle = new Dictionary<string, ItemStyle>();
            Dictionary<string, int> dicSize = new Dictionary<string, int>();
            errorMessage = string.Empty;

            try
            {
                dicInvoiceDetail = invoice?.InvoiceDetails?.ToDictionary(x => x.CustomerStyle + x.CustomerPurchaseOrderNumber + x.GarmentColorCode);

                if (dicInvoiceDetail == null)
                {
                    dicInvoiceDetail = new Dictionary<string, InvoiceDetail>();
                }

                foreach (var packing in packingList)
                {
                    foreach (var itemStyle in packing.ItemStyles)
                    {
                        var prices = partPrices.Where(x => x.Season == itemStyle.Season &&
                                                        x.StyleNO == itemStyle.CustomerStyle &&
                                                        x.GarmentColorCode == itemStyle.ColorCode)?.ToList();

                        var lines = packing?.PackingLines.Where(x => x.LSStyle == itemStyle.LSStyle).ToList();

                        InvoiceDetail invoiceDetail = new InvoiceDetail();
                        invoiceDetail.GarmentColorCode = itemStyle.ColorCode;
                        invoiceDetail.GarmentColorName = itemStyle.ColorName;
                        invoiceDetail.CustomerStyle = itemStyle.CustomerStyle;
                        invoiceDetail.CustomerPurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                        invoiceDetail.Description = itemStyle.Description;
                        invoiceDetail.UnitID = itemStyle?.UnitID;
                        invoiceDetail.Quantity = lines.Sum(x => x.QuantitySize * x.TotalCarton);
                        invoiceDetail.PriceCM = prices?.Where(x => x.ProductionType == "CM")?.
                                                OrderByDescending(x => x.EffectiveDate)?.FirstOrDefault()?.Price;
                        invoiceDetail.PriceFOB = prices?.Where(x => x.ProductionType == "FOB")?.
                                                OrderByDescending(x => x.EffectiveDate)?.FirstOrDefault()?.Price;
                        invoiceDetail.Amount = invoiceDetail.PriceCM != null ?
                                                    invoiceDetail.Quantity * invoiceDetail.PriceCM : null;

                        if (dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out InvoiceDetail rsInvoiceDetail))
                        {
                            rsInvoiceDetail.Quantity += invoiceDetail.Quantity;
                            rsInvoiceDetail.Amount += invoiceDetail.Amount != null ? invoiceDetail.Amount : 0;
                            rsInvoiceDetail.SetUpdateAudit(userName);
                        }
                        else
                        {
                            invoiceDetail.SetCreateAudit(userName);
                            dicInvoiceDetail[invoiceDetail.KeyInvoice()] = invoiceDetail;
                        }
                    }

                    invoice.PackingList.Add(packing);
                }

                invoice.SetCreateAudit(userName);

                Dictionary<string, bool> dicDuplicateInvoiceDetail = new Dictionary<string, bool>();

                if (invoice.InvoiceDetails != null)
                {
                    foreach (var invoiceDetail in invoice.InvoiceDetails)
                    {
                        if (dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out InvoiceDetail rsInvoiceDetail))
                        {
                            invoiceDetail.Quantity = rsInvoiceDetail.Quantity;
                            dicDuplicateInvoiceDetail[invoiceDetail.KeyInvoice()] = true;
                        }
                    }
                }

                foreach (var keyValuePair in dicInvoiceDetail)
                {
                    if (!dicDuplicateInvoiceDetail.ContainsKey(keyValuePair.Value.KeyInvoice()))
                    {
                        invoice.InvoiceDetails.Add(keyValuePair.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " !!! Please check invoice details";
            }
        }

        public static void DeleteInvoice_GA(Invoice invoice, List<PackingList> packingList, string userName,
            out string errorMessage)
        {
            Dictionary<string, decimal?> dicInvoiceDetail = new Dictionary<string, decimal?>();
            Dictionary<string, ItemStyle> dicItemStyle = new Dictionary<string, ItemStyle>();
            errorMessage = string.Empty;

            try
            {
                foreach (var packing in packingList)
                {
                    foreach (var itemStyle in packing.ItemStyles)
                    {
                        if (!dicItemStyle.ContainsKey(itemStyle.LSStyle))
                        {
                            dicItemStyle[itemStyle.LSStyle] = itemStyle;
                        }
                    }

                    foreach (var packingLine in packing.PackingLines)
                    {
                        if (dicItemStyle.TryGetValue(packingLine.LSStyle, out ItemStyle rsItemStyle))
                        {
                            string key = string.Empty;
                            var price = rsItemStyle.OrderDetails.FirstOrDefault().Price;

                            key = rsItemStyle.CustomerStyle + rsItemStyle.PurchaseOrderNumber + rsItemStyle.ColorCode;

                            decimal? quantity = packingLine.QuantitySize * packingLine.TotalCarton;

                            if (dicInvoiceDetail.TryGetValue(key, out decimal? rsQuantity))
                            {
                                quantity += (rsQuantity);
                            }

                            dicInvoiceDetail[key] = quantity;
                        }
                    }
                }

                List<InvoiceDetail> deleteInvoiceDetails = new List<InvoiceDetail>();

                foreach (var invoiceDetail in invoice.InvoiceDetails)
                {
                    if (dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out decimal? rsQuantity))
                    {
                        invoiceDetail.Quantity = invoiceDetail.Quantity - rsQuantity;

                        if (invoiceDetail.Quantity <= 0)
                        {
                            deleteInvoiceDetails.Add(invoiceDetail);
                        }
                        else
                        {
                            invoiceDetail.Amount = invoiceDetail.Quantity * invoiceDetail.UnitPrice;
                        }
                    }
                }

                foreach (var itemDelete in deleteInvoiceDetails)
                {
                    invoice.InvoiceDetails.Remove(itemDelete);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " !!! Please check invoice details";
            }
        }

        #region support function
        /// <summary>
        /// Short size when choose multi packing list
        /// </summary>
        /// <param name="key">itemStyle.CustomerStyle + itemStyle.PurchaseOrderNumber + itemStyle.ColorCode</param>
        /// <param name="itemStyle"></param>
        /// <param name="packing"></param>
        /// <param name="dicSizeMin"></param>
        /// <param name="dicSizeMax"></param>
        /// <param name="dicSize"></param>
        /// <returns></returns>
        private static string JoinSizeIFG(string key, ItemStyle itemStyle, PackingList packing,
            ref Dictionary<string, Dictionary<string, int>> dicSizeMin,
            ref Dictionary<string, Dictionary<string, int>> dicSizeMax,
            ref Dictionary<string, int> dicSize)
        {
            foreach (var orderDetail in itemStyle.OrderDetails)
            {
                if (!dicSize.TryGetValue(orderDetail.Size, out int rsNumber))
                {
                    dicSize[orderDetail.Size] = orderDetail.SizeSortIndex ?? 0;
                }
            }

            foreach (var packingLine in packing.PackingLines)
            {
                if (dicSizeMin.TryGetValue(key, out Dictionary<string, int> rsMin))
                {
                    if (dicSize.TryGetValue(packingLine.Size, out int rsIndex))
                    {
                        if (!rsMin.TryGetValue(packingLine.Size, out int rsIndexMin))
                        {
                            int minIndex = rsMin.Values.First();

                            if (rsIndex < minIndex)
                            {
                                rsMin = new Dictionary<string, int>();
                                rsMin[packingLine.Size] = rsIndex;
                                dicSizeMin[key] = rsMin;
                            }
                        }
                    }
                }
                else
                {
                    if (dicSize.TryGetValue(packingLine.Size, out int rsIndex))
                    {
                        Dictionary<string, int> dicMin = new Dictionary<string, int>();
                        dicMin[packingLine.Size] = rsIndex;
                        dicSizeMin[key] = dicMin;
                    }
                }

                if (dicSizeMax.TryGetValue(key, out Dictionary<string, int> rsMax))
                {
                    if (dicSize.TryGetValue(packingLine.Size, out int rsIndex))
                    {
                        if (!rsMax.TryGetValue(packingLine.Size, out int rsIndexMax))
                        {
                            int maxIndex = rsMin.Values.First();
                            if (rsIndex > maxIndex)
                            {
                                rsMax = new Dictionary<string, int>();
                                rsMax[packingLine.Size] = rsIndex;
                                dicSizeMax[key] = rsMax;
                            }
                        }
                    }
                }
                else
                {
                    if (dicSize.TryGetValue(packingLine.Size, out int rsIndex))
                    {
                        Dictionary<string, int> dicMax = new Dictionary<string, int>();
                        dicMax[packingLine.Size] = rsIndex;
                        dicSizeMax[key] = dicMax;
                    }
                }
            }

            string garmentSize = string.Empty;

            if (dicSizeMin.TryGetValue(key, out Dictionary<string, int> rsSizeMin))
            {
                if (rsSizeMin.Keys.Count > 0)
                {
                    garmentSize = rsSizeMin.Keys.First();
                }
            }

            if (dicSizeMax.TryGetValue(key, out Dictionary<string, int> rsSizeMax))
            {
                if (rsSizeMax.Keys.Count > 0)
                {
                    garmentSize += "-" + rsSizeMax.Keys.First();
                }
            }

            return garmentSize;
        }
        #endregion support function
    }
}
