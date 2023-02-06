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
    public class ImportSalesOrderProcess
    {
        public static SalesOrder Import(string filePath, string fileName,
            string userName, string customerID, IEnumerable<Size> sizes,
            IQueryable<SalesContractDetail> salesContractDetail,
            IQueryable<ItemStyle> itemStyles,
            List<PurchaseOrderType> purchaseOrderTypes,
            IQueryable<Part> parts,
            out List<Part> newParts, out List<PurchaseOrderType> newPurchaseOrderTypes,
            out List<SalesOrder> listSalesOrder, out string errorMessage, bool? isCompare = null)
        {
            var salesOrder = new SalesOrder();
            errorMessage = string.Empty;
            newParts = new List<Part>();
            newPurchaseOrderTypes = new List<PurchaseOrderType>();
            listSalesOrder = new List<SalesOrder>();

            if (!String.IsNullOrEmpty(customerID))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
                {
                    using (var package = new ExcelPackage(fileInfo))
                    {
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            switch (customerID)
                            {
                                case "PU":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();

                                        salesOrder = workSheet.ImportSalesOrderPU(userName,
                                            salesContractDetail, sizes, out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.ConfirmDate = DateTime.Now;
                                            salesOrder.PaymentTermCode = "15";
                                            salesOrder.DivisionID = "BU4";
                                            salesOrder.PriceTermCode = "CM";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.SaveFilePath = filePath;
                                            salesOrder.FileName = fileName;
                                        }
                                    }
                                    break;
                                case "GA":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();

                                        salesOrder = workSheet.ImportSalesOrderGA(userName, sizes,
                                            purchaseOrderTypes, itemStyles.ToList(), parts.ToList(),
                                            out newParts, out newPurchaseOrderTypes,
                                            out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.ConfirmDate = DateTime.Now;
                                            salesOrder.PaymentTermCode = "4";
                                            salesOrder.DivisionID = "BU1";
                                            salesOrder.PriceTermCode = "FOB";
                                            salesOrder.PriceTermDescription = "FOB";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.SaveFilePath = filePath;
                                            salesOrder.FileName = fileName;
                                        }
                                    }
                                    break;
                                case "CHA":
                                    {
                                        var workSheet = package.Workbook.Worksheets[2];

                                        salesOrder = workSheet.ImportSalesOrderCHA(userName, sizes,
                                            out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.ConfirmDate = DateTime.Now;
                                            salesOrder.DivisionID = "BU1";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.SaveFilePath = filePath;
                                            salesOrder.FileName = fileName;
                                        }
                                    }
                                    break;
                                case "DE":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();

                                        salesOrder = workSheet.ImportSalesOrderDE(userName, sizes,
                                            purchaseOrderTypes, itemStyles.ToList(),
                                            parts.ToList(), out newParts,
                                            out newPurchaseOrderTypes, out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.ConfirmDate = DateTime.Now;
                                            salesOrder.DivisionID = "BU3";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.SaveFilePath = filePath;
                                            salesOrder.FileName = fileName;
                                        }
                                    }
                                    break;
                                case "HA":
                                    {
                                        Dictionary<string, ItemStyle> oldItemStyleProduction = new Dictionary<string, ItemStyle>();

                                        for (int i = 1; i < package.Workbook.Worksheets.Count; i++)
                                        {
                                            //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>. " + package.Workbook.Worksheets[i].Name);
                                            var productionItemStyles = package.Workbook.Worksheets[i].ImportProduction_HA(out errorMessage);

                                            foreach (var productionItem in productionItemStyles)
                                            {
                                                if (!oldItemStyleProduction.TryGetValue(productionItem.PurchaseOrderNumber, out ItemStyle rsItemStyle))
                                                {
                                                    oldItemStyleProduction[productionItem.PurchaseOrderNumber] = productionItem;
                                                }
                                            }
                                        }

                                        Dictionary<string, Part> dicNewPartItemStyleProduction = new Dictionary<string, Part>();

                                        foreach (var itemStyle in oldItemStyleProduction)
                                        {
                                            var subFix = itemStyle.Value.LSStyle.Split("-");
                                            if (subFix != null && subFix.Length > 1)
                                            {
                                                //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>. " + itemStyle.Value.LSStyle);
                                                Part newPart = new Part();
                                                newPart.ID = string.Empty;
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

                                        List<Part> partList = parts.ToList();
                                        foreach (var newPart in dicNewPartItemStyleProduction)
                                        {
                                            partList.Add(newPart.Value);
                                        }

                                        var workSheet = package.Workbook.Worksheets.First();

                                        listSalesOrder = workSheet.ImportSalesOrderHA(userName, sizes, ref partList, oldItemStyleProduction,
                                            out newParts, out errorMessage, isCompare);

                                        if (listSalesOrder != null)
                                        {
                                            foreach (var saleOrder in listSalesOrder)
                                            {
                                                saleOrder.SetCreateAudit(userName);
                                                saleOrder.CustomerID = customerID;
                                                saleOrder.OrderDate = DateTime.Now;
                                                saleOrder.ConfirmDate = DateTime.Now;
                                                saleOrder.DivisionID = "BU1";
                                                saleOrder.CurrencyID = "USD";
                                                saleOrder.PaymentTermCode = "8";
                                                saleOrder.SalesOrderStatusCode = "Order";
                                                saleOrder.PriceTermCode = "FOB";
                                                saleOrder.PriceTermDescription = "FOB";
                                                saleOrder.FileName = fileName;
                                                saleOrder.SaveFilePath = filePath;
                                            }
                                        }
                                    }
                                    break;
                                case "KA":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();

                                        salesOrder = workSheet.ImportSalesOrderKA(userName, sizes, parts.ToList(),
                                            out newParts,
                                            out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.ConfirmDate = DateTime.Now;
                                            salesOrder.DivisionID = "BU1";
                                            salesOrder.CurrencyID = "USD";
                                            salesOrder.PaymentTermCode = "8";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.PriceTermCode = "FOB";
                                            salesOrder.PriceTermDescription = "FOB";
                                            salesOrder.FileName = fileName;
                                            salesOrder.SaveFilePath = filePath;
                                        }
                                    }
                                    break;
                                case "OS":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();

                                        salesOrder = workSheet.ImportSalesOrderOS(userName,
                                            out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.DivisionID = "BU1";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.FileName = fileName;
                                            salesOrder.SaveFilePath = filePath;
                                        }
                                    }
                                    break;
                                case "IFG":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();

                                        salesOrder = workSheet.ImportSalesOrderIFG(userName, sizes,
                                             purchaseOrderTypes, itemStyles.ToList(), out newPurchaseOrderTypes,
                                            out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.DivisionID = "BU1";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.FileName = fileName;
                                            salesOrder.SaveFilePath = filePath;
                                        }
                                    }
                                    break;
                                case "JA":
                                    break;
                                case "LTD":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();

                                        salesOrder = workSheet.ImportSalesOrderLTD(userName, sizes,
                                            out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.DivisionID = "BU1";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.FileName = fileName;
                                            salesOrder.SaveFilePath = filePath;
                                        }
                                    }
                                    break;
                                case "HM":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();

                                        salesOrder = workSheet.ImportSalesOrderHM(userName, sizes,
                                            purchaseOrderTypes, parts.ToList(),
                                            out newParts, out newPurchaseOrderTypes,
                                            out errorMessage);

                                        if (salesOrder != null)
                                        {
                                            salesOrder.SetCreateAudit(userName);
                                            salesOrder.CustomerID = customerID;
                                            salesOrder.OrderDate = DateTime.Now;
                                            salesOrder.ConfirmDate = DateTime.Now;
                                            salesOrder.DivisionID = "BU1";
                                            salesOrder.SalesOrderStatusCode = "Order";
                                            salesOrder.SaveFilePath = filePath;
                                            salesOrder.FileName = fileName;
                                        }
                                    }
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
            }

            return salesOrder;
        }


    }
}
