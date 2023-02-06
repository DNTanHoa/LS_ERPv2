using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class UpdateInvoiceProcessor
    {
        public static void UpdateInvoice(Invoice invoice, List<PackingList> packingList, string userName, Unit ifgUnit,
            List<PartPrice> partPrices, List<SalesOrder> salesOrders,
            out List<InvoiceDetail> deleteInvoiceDetails,
            out List<InvoiceDetail> newInvoiceDetails,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            deleteInvoiceDetails = new List<InvoiceDetail>();
            newInvoiceDetails = new List<InvoiceDetail>();
            Dictionary<string, InvoiceDetail> dicInvoiceDetail = new Dictionary<string, InvoiceDetail>();
            Dictionary<string, int> dicSize = new Dictionary<string, int>();
            Dictionary<string, Dictionary<string, int>> dicSizeMin = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, Dictionary<string, int>> dicSizeMax = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, ItemStyle> dicItemStyle = new Dictionary<string, ItemStyle>();

            try
            {
                if(invoice?.CustomerID == "IFG")
                {
                    foreach (var invoiceDetail in invoice.InvoiceDetails)
                    {
                        deleteInvoiceDetails.Add(invoiceDetail);
                    }

                    invoice.InvoiceDetails = new List<InvoiceDetail>();

                    foreach (var packing in packingList)
                    {
                        foreach (var itemStyle in packing.ItemStyles)
                        {
                            decimal? quantity = 0;
                            decimal? amount = 0;
                            foreach (var line in packing?.PackingLines?.
                                Where(x => x.LSStyle == itemStyle.LSStyle))
                            {
                                var sizeQty = line.QuantitySize * line.TotalCarton;
                                amount += sizeQty * itemStyle?.OrderDetails?.
                                    Where(o => o.ItemStyleNumber == itemStyle.Number &&
                                            o.Size == line.Size)?.FirstOrDefault()?.Price ?? 0;
                                quantity += sizeQty;

                            }

                            InvoiceDetail invoiceDetail = new InvoiceDetail();
                            invoiceDetail.GarmentColorCode = itemStyle.ColorCode;
                            invoiceDetail.GarmentColorName = itemStyle.ColorName;
                            invoiceDetail.CustomerStyle = itemStyle.CustomerStyle;
                            invoiceDetail.CustomerPurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                            invoiceDetail.Description = itemStyle.Description;
                            invoiceDetail.GarmentSize = JoinSizeIFG(invoiceDetail.KeyInvoice(), itemStyle, packing, ref dicSizeMin, ref dicSizeMax, ref dicSize);
                            invoiceDetail.UnitID = ifgUnit?.ID ?? "DZ";
                            invoiceDetail.Quantity = quantity / (ifgUnit?.Factor ?? 12);
                            invoiceDetail.Amount = amount;
                            invoiceDetail.UnitPrice = invoiceDetail.Amount / invoiceDetail.Quantity;

                            if (dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out InvoiceDetail rsInvoiceDetail))
                            {
                                rsInvoiceDetail.Quantity += invoiceDetail.Quantity;
                                rsInvoiceDetail.Amount += invoiceDetail.Amount;
                                rsInvoiceDetail.UnitPrice = rsInvoiceDetail.Amount / rsInvoiceDetail.Quantity;
                                rsInvoiceDetail.SetUpdateAudit(userName);
                            }
                            else
                            {
                                invoiceDetail.SetCreateAudit(userName);
                                dicInvoiceDetail[invoiceDetail.KeyInvoice()] = invoiceDetail;
                            }
                        }
                    }

                    invoice.SetUpdateAudit(userName);

                    foreach (var invoiceDetail in dicInvoiceDetail)
                    {
                        invoiceDetail.Value.InvoiceID = invoice.ID;
                        newInvoiceDetails.Add(invoiceDetail.Value);
                    }
                }
                else if (invoice?.CustomerID == "PU")
                {
                    foreach (var invoiceDetail in invoice.InvoiceDetails)
                    {
                        deleteInvoiceDetails.Add(invoiceDetail);
                    }

                    invoice.InvoiceDetails = new List<InvoiceDetail>();

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
                    }

                    invoice.SetUpdateAudit(userName);

                    foreach (var invoiceDetail in dicInvoiceDetail)
                    {
                        invoiceDetail.Value.InvoiceID = invoice.ID;
                        newInvoiceDetails.Add(invoiceDetail.Value);
                    }
                }
                else if (invoice?.CustomerID == "DE")
                {
                    foreach (var invoiceDetail in invoice.InvoiceDetails)
                    {
                        deleteInvoiceDetails.Add(invoiceDetail);
                    }

                    invoice.InvoiceDetails = new List<InvoiceDetail>();

                    foreach (var packing in packingList)
                    {
                        foreach (var itemStyle in packing.ItemStyles)
                        {
                            var priceTermCode = salesOrders?.FirstOrDefault(s => s.ID == itemStyle.SalesOrderID)?.PriceTermCode;
                            decimal priceCM = 0;
                            decimal priceFOB = 0;
                            decimal otherPrice = itemStyle?.OrderDetails?.FirstOrDefault()?.OtherPrice ?? 0;
                            if (otherPrice == 0)
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
                                if (priceTermCode?.Trim()?.ToUpper() == "FOB")
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
                            if (priceTermCode?.Trim()?.ToUpper() == "FOB")
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
                    }

                    invoice.SetUpdateAudit(userName);

                    foreach (var invoiceDetail in dicInvoiceDetail)
                    {
                        invoiceDetail.Value.InvoiceID = invoice.ID;
                        newInvoiceDetails.Add(invoiceDetail.Value);
                    }
                }
                else
                {
                    foreach (var invoiceDetail in invoice.InvoiceDetails)
                    {
                        deleteInvoiceDetails.Add(invoiceDetail);
                    }

                    invoice.InvoiceDetails = new List<InvoiceDetail>();

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
                                var price = rsItemStyle?.OrderDetails?.FirstOrDefault()?.Price;

                                //key = rsItemStyle.CustomerStyle + rsItemStyle.PurchaseOrderNumber + rsItemStyle.ColorCode;

                                InvoiceDetail invoiceDetail = new InvoiceDetail();
                                invoiceDetail.GarmentColorCode = rsItemStyle.ColorCode;
                                invoiceDetail.GarmentColorName = rsItemStyle.ColorName;
                                invoiceDetail.CustomerStyle = rsItemStyle.CustomerStyle;
                                invoiceDetail.CustomerPurchaseOrderNumber = rsItemStyle.PurchaseOrderNumber;
                                invoiceDetail.Description = rsItemStyle.Description;
                                //invoiceDetail.GarmentSize = JoinSizeIFG(invoiceDetail.KeyInvoice(), rsItemStyle, packing, ref dicSizeMin, ref dicSizeMax, ref dicSize);
                                invoiceDetail.UnitID = rsItemStyle?.UnitID;
                                invoiceDetail.UnitPrice = price;
                                invoiceDetail.Quantity = packingLine.QuantitySize * packingLine.TotalCarton;
                                invoiceDetail.Amount = invoiceDetail.Quantity * invoiceDetail.UnitPrice;
                                if (dicInvoiceDetail.TryGetValue(invoiceDetail.KeyInvoice(), out InvoiceDetail rsInvoiceDetail))
                                {
                                    invoiceDetail.Quantity += (rsInvoiceDetail.Quantity);
                                    invoiceDetail.Amount += (rsInvoiceDetail.Amount);
                                }
                                
                                invoiceDetail.SetUpdateAudit(userName);
                                dicInvoiceDetail[invoiceDetail.KeyInvoice()] = invoiceDetail;
                            }
                        }

                        //invoice.PackingList.Add(packing);
                    }

                    invoice.SetUpdateAudit(userName);

                    foreach (var invoiceDetail in dicInvoiceDetail)
                    {
                        invoiceDetail.Value.InvoiceID = invoice.ID;
                        newInvoiceDetails.Add(invoiceDetail.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

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
    }
}
