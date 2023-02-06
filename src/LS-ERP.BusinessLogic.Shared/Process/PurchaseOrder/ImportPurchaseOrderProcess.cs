using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class ImportPurchaseOrderProcess
    {
        public static void Import(string filePath, string fileName,
            string userName, string customerID,
            List<Vendor> vendors, TypeImportPurchaseOrder type,
            out List<PurchaseOrderType> newPurchaseOrderTypes,
            out List<string> contractNos,
            out List<string> purchaseOrderNumbers,
            out List<Vendor> newVendors,
            out Dictionary<string, List<PurchaseOrderLine>> dicTrackingPO,
            out Dictionary<string, List<PurchaseOrderLine>> dicDetailPO,
            out Dictionary<string, List<PurchaseOrderLine>> dicInternationalOCL,
            out Dictionary<string, List<PurchaseOrderLine>> dicCareLabelPO,
            out string errorMessage)
        {
            //List<PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();
            //var purchaseOrder = new PurchaseOrder();
            errorMessage = string.Empty;
            newPurchaseOrderTypes = new List<PurchaseOrderType>();
            contractNos = new List<string>();
            newVendors = new List<Vendor>();
            purchaseOrderNumbers = new List<string>();
            dicTrackingPO = new Dictionary<string, List<PurchaseOrderLine>>();
            dicDetailPO = new Dictionary<string, List<PurchaseOrderLine>>();
            dicInternationalOCL = new Dictionary<string, List<PurchaseOrderLine>>();
            dicCareLabelPO = new Dictionary<string, List<PurchaseOrderLine>>();

            var dicOldVendors = vendors.ToDictionary(x => x.ID);

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
                                        switch (type)
                                        {
                                            case TypeImportPurchaseOrder.FabricTracking:
                                                {
                                                    var workSheet = package.Workbook.Worksheets.First();
                                                    dicTrackingPO = workSheet.ImportFB_PurchaseOrderGroupLine_PU(userName, dicOldVendors, out purchaseOrderNumbers, out newVendors, out errorMessage);
                                                }
                                                break;
                                            case TypeImportPurchaseOrder.TrimTracking:
                                                {
                                                    var workSheet = package.Workbook.Worksheets.First();
                                                    dicTrackingPO = workSheet.ImportPurchaseOrderGroupLine_PU(userName, dicOldVendors, out purchaseOrderNumbers, out newVendors, out errorMessage);

                                                }
                                                break;
                                            case TypeImportPurchaseOrder.PurchaseDetail:
                                                {
                                                    var workSheetPOLine = package.Workbook.Worksheets.First();
                                                    dicDetailPO = workSheetPOLine.ImportPurchaseOrderLine_PU(userName, dicOldVendors, ref purchaseOrderNumbers, out newVendors, out contractNos, out errorMessage);

                                                    var workSheetPO_InternationalOCL = package.Workbook.Worksheets[1];
                                                    dicInternationalOCL = workSheetPO_InternationalOCL.ImportPurchaseOrderLineInternational_PU(userName, ref purchaseOrderNumbers, out errorMessage);
                                                }
                                                break;
                                            case TypeImportPurchaseOrder.Carelabel:
                                                {
                                                    var workSheetPO_WebsitePO = package.Workbook.Worksheets.First();
                                                    dicCareLabelPO = workSheetPO_WebsitePO.ImportPurchaseOrderCareLabel_PU(userName, out errorMessage);
                                                }
                                                break;
                                        }
                                    }
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

            //return purchaseOrders;
        }

        public static void CreateOrUpdatePurchaseOrder(string userName,
         Dictionary<string, PurchaseOrder> dicOldPurchaseOrders,
         Dictionary<string, List<PurchaseOrderLine>> dicTrackingPO,
         List<PurchaseOrderGroupLine> oldPurchaseOrderGroupLines,
         out List<PurchaseOrder> newPurchaseOrders,
         out List<PurchaseOrderGroupLine> updateOldPurchaseOrderGroupLines,
         out List<PurchaseOrderGroupLine> newOldPurchaseOrderGroupLines,
         out List<PurchaseOrderGroupLine> newPurchaseOrderGroupLines,
         out string errorMessage)
        {
            errorMessage = string.Empty;
            newPurchaseOrders = new List<PurchaseOrder>();
            updateOldPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            newOldPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            newPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();

            try
            {
                var dicOldPurchaseOrderGroupLine = oldPurchaseOrderGroupLines.ToDictionary(x => x.CustomerPurchaseOrderNumber + x.ItemID + x.ItemColorName + x.Specify);

                var config = new MapperConfiguration(
                    cfg => cfg.CreateMap<PurchaseOrderLine, PurchaseOrderGroupLine>()
                    .ForMember(d => d.ID, o => o.Ignore())
                    .ForMember(x => x.CreatedAt, y => y.Ignore())
                    .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                    .ForMember(x => x.CreatedBy, y => y.Ignore())
                    .ForMember(x => x.LastUpdatedBy, y => y.Ignore())
                    );

                var mapper = new Mapper(config);

                foreach (var itemTracking in dicTrackingPO)
                {
                    if (dicOldPurchaseOrders.TryGetValue(itemTracking.Key, out PurchaseOrder rsPO))
                    {
                        foreach (var line in itemTracking.Value)
                        {
                            var key = line.CustomerPurchaseOrderNumber + line.ItemID + line.ItemColorName + line.Specify;

                            if (dicOldPurchaseOrderGroupLine.TryGetValue(key, out PurchaseOrderGroupLine oldGroupLine))
                            {
                                oldGroupLine.Quantity = line.Quantity;
                                oldGroupLine.WareHouseQuantity = oldGroupLine.Quantity;

                                oldGroupLine.SetUpdateAudit(userName);
                                updateOldPurchaseOrderGroupLines.Add(oldGroupLine);
                            }
                            else
                            {
                                PurchaseOrderGroupLine newGroupLine = new PurchaseOrderGroupLine();
                                mapper.Map(line, newGroupLine);
                                newGroupLine.WareHouseUnitID = line.UnitID;
                                newGroupLine.SetCreateAudit(userName);
                                newGroupLine.PurchaseOrderID = rsPO.ID;

                                newOldPurchaseOrderGroupLines.Add(newGroupLine);
                            }
                        }
                    }
                    else
                    {
                        PurchaseOrder purchaseOrder = new PurchaseOrder();

                        purchaseOrder.Number = itemTracking.Key;
                        purchaseOrder.OrderDate = DateTime.Now;
                        purchaseOrder.CustomerID = "PU";
                        //purchaseOrder.VendorID = "PU";
                        purchaseOrder.ProductionMethodCode = "CMT";
                        purchaseOrder.CurrencyID = "USD";
                        purchaseOrder.CompanyCode = "LS";
                        purchaseOrder.SetCreateAudit(userName);
                        purchaseOrder.Description = itemTracking.Value.First()?.Remarks;
                        purchaseOrder.ShipDate = itemTracking.Value.First()?.ShipDate;
                        purchaseOrder.VendorID = itemTracking.Value.First()?.CustomerSupplier;

                        foreach (var itemLine in itemTracking.Value)
                        {
                            PurchaseOrderGroupLine newGroupLine = new PurchaseOrderGroupLine();
                            mapper.Map(itemLine, newGroupLine);
                            newGroupLine.WareHouseUnitID = itemLine.UnitID;
                            newGroupLine.SetCreateAudit(userName);
                            newGroupLine.PurchaseOrderID = purchaseOrder.ID;
                            newPurchaseOrderGroupLines.Add(newGroupLine);
                        }

                        newPurchaseOrders.Add(purchaseOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public static void CreateOrUpdatePurchaseOrderLine(string userName,
         Dictionary<string, PurchaseOrder> dicOldPurchaseOrders,
         Dictionary<string, List<PurchaseOrderLine>> dicDetailPO,
         Dictionary<string, List<PurchaseOrderLine>> dicInternationalOCL,
         List<PurchaseOrderGroupLine> oldPurchaseOrderGroupLines,
         List<PurchaseOrderLine> oldPurchaseOrderLines,
         List<ItemStyle> itemStyles,
         List<PartMaterial> partMaterials,
         out List<PurchaseOrderGroupLine> updateOldPurchaseOrderGroupLines,
         out List<PurchaseOrderLine> updateOldPurchaseOrderLines,
         out List<PurchaseOrderLine> newOldPurchaseOrderLines,
         out string errorMessage)
        {
            errorMessage = string.Empty;
            updateOldPurchaseOrderLines = new List<PurchaseOrderLine>();
            updateOldPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            newOldPurchaseOrderLines = new List<PurchaseOrderLine>();

            try
            {
                var dicOldPurchaseOrderLine = oldPurchaseOrderLines.ToDictionary(x => x.CustomerPurchaseOrderNumber + x.ContractNo + x.ItemID + x.ItemColorName);
                var dicOldPurchaseOrderGroupLines = new Dictionary<string, PurchaseOrderGroupLine>();

                foreach (var item in oldPurchaseOrderGroupLines)
                {
                    var keyGroup = item.CustomerPurchaseOrderNumber + item.ItemID + item.ItemColorName;
                    dicOldPurchaseOrderGroupLines[keyGroup] = item;
                }

                Dictionary<string, ItemStyle> dicContractNoItemStyle = itemStyles.ToDictionary(x => x.ContractNo);
                Dictionary<string, PartMaterial> dicItemCode = new Dictionary<string, PartMaterial>();

                foreach (var item in partMaterials)
                {
                    if (!dicItemCode.ContainsKey(item.PartNumber + item.ItemStyleNumber + item.ItemID + item.ItemColorName))
                    {
                        dicItemCode[item.PartNumber + item.ItemStyleNumber + item.ItemID + item.ItemColorName] = item;
                    }
                }

                var config = new MapperConfiguration(
                    cfg => cfg.CreateMap<PurchaseOrderLine, PurchaseOrderGroupLine>()
                    .ForMember(d => d.ID, o => o.Ignore())
                    .ForMember(x => x.CreatedAt, y => y.Ignore())
                    .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                    .ForMember(x => x.CreatedBy, y => y.Ignore())
                    .ForMember(x => x.LastUpdatedBy, y => y.Ignore())
                    );

                var mapper = new Mapper(config);

                var configLine = new MapperConfiguration(
                    cfg => cfg.CreateMap<PurchaseOrderLine, PurchaseOrderLine>()
                    .ForMember(d => d.ID, o => o.Ignore())
                    .ForMember(x => x.CreatedAt, y => y.Ignore())
                    .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                    .ForMember(x => x.CreatedBy, y => y.Ignore())
                    .ForMember(x => x.LastUpdatedBy, y => y.Ignore())
                    );

                var mapperLine = new Mapper(configLine);

                foreach (var itemTracking in dicDetailPO)
                {
                    var dicItemNo = new Dictionary<string, int>();
                    if (dicOldPurchaseOrders.TryGetValue(itemTracking.Key, out PurchaseOrder rsPO))
                    {
                        foreach (var line in itemTracking.Value)
                        {
                            var key = line.CustomerPurchaseOrderNumber + line.ContractNo + line.ItemID + line.ItemColorName;

                            if (dicOldPurchaseOrderLine.TryGetValue(key, out PurchaseOrderLine oldLine))
                            {
                                oldLine.Quantity = line.Quantity;
                                oldLine.WareHouseQuantity = oldLine.Quantity;

                                oldLine.SetUpdateAudit(userName);
                                updateOldPurchaseOrderLines.Add(oldLine);
                            }
                            else
                            {
                                PurchaseOrderLine newLine = new PurchaseOrderLine();
                                mapperLine.Map(line, newLine);
                                newLine.SetCreateAudit(userName);
                                newLine.PurchaseOrderID = rsPO.ID;
                                newLine.WareHouseQuantity = line.Quantity;

                                var keyGroup = newLine.CustomerPurchaseOrderNumber + newLine.ItemID + newLine.ItemColorName;

                                if (dicItemCode.TryGetValue(newLine.ContractNo + newLine.CustomerStyle + newLine.ItemID + newLine.ItemColorName, out PartMaterial rsMaterial))
                                {
                                    newLine.ItemCode = rsMaterial.ItemCode;
                                }

                                if (dicContractNoItemStyle.TryGetValue(newLine.ContractNo, out ItemStyle rsItemStyle))
                                {
                                    newLine.LSStyle = rsItemStyle.LSStyle;
                                    newLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                    newLine.GarmentColorCode = rsItemStyle.ColorCode;
                                    newLine.GarmentColorName = rsItemStyle.ColorName;
                                    newLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                    newLine.Season = rsItemStyle.Season;
                                }

                                //if (dicOldPurchaseOrderGroupLines.TryGetValue(keyGroup, out PurchaseOrderGroupLine rsGroupLine))
                                //{
                                //    newLine.PurchaseOrderGroupLineID = rsGroupLine.ID;
                                //    rsGroupLine.GarmentColorName = newLine.GarmentColorName;
                                //    rsGroupLine.GarmentColorCode = newLine.GarmentColorCode;
                                //    rsGroupLine.CustomerStyle = newLine.CustomerStyle;

                                //    dicOldPurchaseOrderGroupLines[keyGroup] = rsGroupLine;
                                //}

                                string keyItemNo = newLine.ItemID + newLine.CustomerPurchaseOrderNumber;
                                if (!dicItemNo.ContainsKey(keyItemNo))
                                {
                                    var number = dicItemNo.Keys.Count + 1;
                                    dicItemNo[keyItemNo] = number;

                                    newLine.ItemNo = number.ToString();
                                }
                                else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                                {
                                    newLine.ItemNo = itemNoNumber.ToString();
                                }

                                newOldPurchaseOrderLines.Add(newLine);
                            }
                        }
                    }
                }

                foreach (var itemTracking in dicInternationalOCL)
                {
                    var dicItemNo = new Dictionary<string, int>();
                    if (dicOldPurchaseOrders.TryGetValue(itemTracking.Key, out PurchaseOrder rsPO))
                    {
                        foreach (var line in itemTracking.Value)
                        {
                            var key = line.CustomerPurchaseOrderNumber + line.ContractNo + line.ItemID;

                            if (dicOldPurchaseOrderLine.TryGetValue(key, out PurchaseOrderLine oldLine))
                            {
                                oldLine.Quantity = line.Quantity;
                                oldLine.WareHouseQuantity = oldLine.Quantity;

                                oldLine.SetUpdateAudit(userName);
                                updateOldPurchaseOrderLines.Add(oldLine);
                            }
                            else
                            {
                                PurchaseOrderLine newLine = new PurchaseOrderLine();
                                mapperLine.Map(line, newLine);
                                newLine.SetCreateAudit(userName);
                                newLine.PurchaseOrderID = rsPO.ID;
                                newLine.WareHouseQuantity = newLine.Quantity;

                                var keyGroup = newLine.CustomerPurchaseOrderNumber + newLine.ItemID + newLine.ItemColorName;


                                if (dicItemCode.TryGetValue(newLine.ContractNo + newLine.CustomerStyle + newLine.ItemID + newLine.ItemColorName, out PartMaterial rsMaterial))
                                {
                                    newLine.ItemCode = rsMaterial.ItemCode;
                                }

                                if (dicContractNoItemStyle.TryGetValue(newLine.ContractNo, out ItemStyle rsItemStyle))
                                {
                                    newLine.LSStyle = rsItemStyle.LSStyle;
                                    newLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                    newLine.GarmentColorCode = rsItemStyle.ColorCode;
                                    newLine.GarmentColorName = rsItemStyle.ColorName;
                                    newLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                    newLine.Season = rsItemStyle.Season;
                                }

                                //if (dicOldPurchaseOrderGroupLines.TryGetValue(keyGroup, out PurchaseOrderGroupLine rsGroupLine))
                                //{
                                //    newLine.PurchaseOrderGroupLineID = rsGroupLine.ID;
                                //    rsGroupLine.GarmentColorName = newLine.GarmentColorName;
                                //    rsGroupLine.GarmentColorCode = newLine.GarmentColorCode;
                                //    rsGroupLine.CustomerStyle = newLine.CustomerStyle;

                                //    dicOldPurchaseOrderGroupLines[keyGroup] = rsGroupLine;
                                //}

                                string keyItemNo = newLine.ItemID + newLine.CustomerPurchaseOrderNumber;
                                if (!dicItemNo.ContainsKey(keyItemNo))
                                {
                                    var number = dicItemNo.Keys.Count + 1;
                                    dicItemNo[keyItemNo] = number;

                                    newLine.ItemNo = number.ToString();
                                }
                                else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                                {
                                    newLine.ItemNo = itemNoNumber.ToString();
                                }

                                newOldPurchaseOrderLines.Add(newLine);
                            }
                        }
                    }
                }

                updateOldPurchaseOrderGroupLines = dicOldPurchaseOrderGroupLines.Select(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }


        public static void CreateOrUpdatePurchaseOrder_CareLabel(string userName,
         Dictionary<string, PurchaseOrder> dicOldPurchaseOrders,
         Dictionary<string, List<PurchaseOrderLine>> dicCareLabelPO,
         List<PurchaseOrderGroupLine> oldPurchaseOrderGroupLines,
         List<PurchaseOrderLine> oldPurchaseOrderLines,
         List<ItemStyle> itemStyles,
         List<PartMaterial> partMaterials,
         out List<PurchaseOrder> newPurchaseOrders,
         out List<PurchaseOrderGroupLine> updateOldPurchaseOrderGroupLines,
         out List<PurchaseOrderGroupLine> newOldPurchaseOrderGroupLines,
         out List<PurchaseOrderGroupLine> newPurchaseOrderGroupLines,
         out List<PurchaseOrderLine> updateOldPurchaseOrderLines,
         out List<PurchaseOrderLine> newOldPurchaseOrderLines,
         //out List<PurchaseOrderLine> newPurchaseOrderLines,
         out string errorMessage)
        {
            errorMessage = string.Empty;
            newPurchaseOrders = new List<PurchaseOrder>();
            updateOldPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            newOldPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            newPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            updateOldPurchaseOrderLines = new List<PurchaseOrderLine>();
            newOldPurchaseOrderLines = new List<PurchaseOrderLine>();
            //newPurchaseOrderLines = new List<PurchaseOrderLine>();

            try
            {
                var dicOldPurchaseOrderGroupLine = oldPurchaseOrderGroupLines.ToDictionary(x => x.CustomerPurchaseOrderNumber + x.ItemID + x.OrderNo + x.ContractNo);
                var dicOldPurchaseOrderLine = oldPurchaseOrderLines.ToDictionary(x => x.CustomerPurchaseOrderNumber + x.ItemID + x.OrderNo + x.ContractNo);

                Dictionary<string, ItemStyle> dicContractNoItemStyle = itemStyles.ToDictionary(x => x.ContractNo);

                var config = new MapperConfiguration(
                    cfg => cfg.CreateMap<PurchaseOrderLine, PurchaseOrderGroupLine>()
                    .ForMember(d => d.ID, o => o.Ignore())
                    .ForMember(x => x.CreatedAt, y => y.Ignore())
                    .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                    .ForMember(x => x.CreatedBy, y => y.Ignore())
                    .ForMember(x => x.LastUpdatedBy, y => y.Ignore())
                    );

                var mapper = new Mapper(config);

                var configPOLine = new MapperConfiguration(
                   cfg => cfg.CreateMap<PurchaseOrderLine, PurchaseOrderLine>()
                   .ForMember(d => d.ID, o => o.Ignore())
                   .ForMember(x => x.CreatedAt, y => y.Ignore())
                   .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                   .ForMember(x => x.CreatedBy, y => y.Ignore())
                   .ForMember(x => x.LastUpdatedBy, y => y.Ignore())
                   );

                var mapperPOLine = new Mapper(configPOLine);

                foreach (var itemTracking in dicCareLabelPO)
                {
                    if (dicOldPurchaseOrders.TryGetValue(itemTracking.Key, out PurchaseOrder rsPO))
                    {
                        foreach (var line in itemTracking.Value)
                        {
                            var key = line.CustomerPurchaseOrderNumber + line.ItemID + line.OrderNo + line.ContractNo;

                            if (dicOldPurchaseOrderGroupLine.TryGetValue(key, out PurchaseOrderGroupLine oldGroupLine))
                            {
                                oldGroupLine.Quantity = line.Quantity;
                                oldGroupLine.WareHouseQuantity = oldGroupLine.Quantity;
                                oldGroupLine.SetUpdateAudit(userName);

                                if (dicOldPurchaseOrderLine.TryGetValue(key, out PurchaseOrderLine oldLine))
                                {
                                    oldLine.SetUpdateAudit(userName);
                                    oldLine.Quantity = line.Quantity;
                                    oldLine.WareHouseQuantity = oldGroupLine.Quantity;

                                    updateOldPurchaseOrderLines.Add(oldLine);
                                }
                                else
                                {
                                    PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                    mapperPOLine.Map(line, purchaseOrderLine);

                                    purchaseOrderLine.WareHouseQuantity = purchaseOrderLine.Quantity;
                                    purchaseOrderLine.SetCreateAudit(userName);
                                    purchaseOrderLine.PurchaseOrderGroupLineID = oldGroupLine.ID;
                                    purchaseOrderLine.PurchaseOrderID = oldGroupLine.PurchaseOrderID;

                                    if (dicContractNoItemStyle.TryGetValue(purchaseOrderLine.ContractNo, out ItemStyle rsItemStyle))
                                    {
                                        purchaseOrderLine.LSStyle = rsItemStyle.LSStyle;
                                        purchaseOrderLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                        purchaseOrderLine.GarmentColorCode = rsItemStyle.ColorCode;
                                        purchaseOrderLine.GarmentColorName = rsItemStyle.ColorName;
                                        purchaseOrderLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                        purchaseOrderLine.Season = rsItemStyle.Season;
                                    }

                                    newOldPurchaseOrderLines.Add(purchaseOrderLine);
                                }
                                updateOldPurchaseOrderGroupLines.Add(oldGroupLine);
                            }
                            else
                            {
                                PurchaseOrderGroupLine newGroupLine = new PurchaseOrderGroupLine();
                                mapper.Map(line, newGroupLine);
                                newGroupLine.SetCreateAudit(userName);
                                newGroupLine.PurchaseOrderID = rsPO.ID;
                                newGroupLine.WareHouseUnitID = line.UnitID;
                                newOldPurchaseOrderGroupLines.Add(newGroupLine);

                                PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                mapperPOLine.Map(line, purchaseOrderLine);

                                purchaseOrderLine.SetCreateAudit(userName);
                                purchaseOrderLine.PurchaseOrderID = rsPO.ID;

                                if (dicContractNoItemStyle.TryGetValue(purchaseOrderLine.ContractNo, out ItemStyle rsItemStyle))
                                {
                                    purchaseOrderLine.LSStyle = rsItemStyle.LSStyle;
                                    purchaseOrderLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                    purchaseOrderLine.GarmentColorCode = rsItemStyle.ColorCode;
                                    purchaseOrderLine.GarmentColorName = rsItemStyle.ColorName;
                                    purchaseOrderLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                    purchaseOrderLine.Season = rsItemStyle.Season;
                                }

                                if (newGroupLine.PurchaseOrderLines == null)
                                {
                                    newGroupLine.PurchaseOrderLines = new List<PurchaseOrderLine>();
                                }

                                newGroupLine.PurchaseOrderLines.Add(purchaseOrderLine);

                                newOldPurchaseOrderGroupLines.Add(newGroupLine);
                            }
                        }
                    }
                    else
                    {
                        PurchaseOrder purchaseOrder = new PurchaseOrder();

                        purchaseOrder.Number = itemTracking.Key;
                        purchaseOrder.OrderDate = itemTracking.Value.First()?.OrderDate;
                        purchaseOrder.CustomerID = "PU";
                        //purchaseOrder.VendorID = "PU";
                        purchaseOrder.CurrencyID = "USD";
                        purchaseOrder.CompanyCode = "LS";
                        purchaseOrder.SetCreateAudit(userName);
                        purchaseOrder.Description = itemTracking.Value.First()?.Remarks;
                        purchaseOrder.ShipDate = itemTracking.Value.First()?.ShipDate;
                        purchaseOrder.VendorID = itemTracking.Value.First()?.CustomerSupplier;

                        var dicNewGroupLine = new Dictionary<string, PurchaseOrderGroupLine>();

                        foreach (var itemLine in itemTracking.Value)
                        {
                            var keyGroup = itemLine.CustomerPurchaseOrderNumber + itemLine.ItemID + itemLine.OrderNo;

                            if (dicNewGroupLine.TryGetValue(keyGroup, out PurchaseOrderGroupLine rsGroupLine))
                            {
                                PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                mapperPOLine.Map(itemLine, purchaseOrderLine);

                                purchaseOrderLine.SetCreateAudit(userName);
                                if (rsGroupLine.PurchaseOrderLines == null)
                                {
                                    rsGroupLine.PurchaseOrderLines = new List<PurchaseOrderLine>();
                                }

                                rsGroupLine.PurchaseOrderLines.Add(purchaseOrderLine);
                                dicNewGroupLine[keyGroup] = rsGroupLine;
                            }
                            else
                            {
                                PurchaseOrderGroupLine newGroupLine = new PurchaseOrderGroupLine();
                                mapper.Map(itemLine, newGroupLine);
                                newGroupLine.SetCreateAudit(userName);
                                newGroupLine.WareHouseUnitID = itemLine.UnitID;

                                PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                mapperPOLine.Map(itemLine, purchaseOrderLine);

                                purchaseOrderLine.SetCreateAudit(userName);
                                if (newGroupLine.PurchaseOrderLines == null)
                                {
                                    newGroupLine.PurchaseOrderLines = new List<PurchaseOrderLine>();
                                }

                                newGroupLine.PurchaseOrderLines.Add(purchaseOrderLine);
                                dicNewGroupLine[keyGroup] = newGroupLine;
                            }
                        }

                        foreach (var item in dicNewGroupLine)
                        {
                            newPurchaseOrderGroupLines.Add(item.Value);
                        }

                        newPurchaseOrders.Add(purchaseOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public static void CreatePurchaseOrder(string userName,
         Dictionary<string, List<PurchaseOrderLine>> dicTrackingPO,
         Dictionary<string, List<PurchaseOrderLine>> dicDetailPO,
         Dictionary<string, List<PurchaseOrderLine>> dicInternationalOCL,
         Dictionary<string, PurchaseOrder> dicPurchaseOrders,
         List<ItemStyle> itemStyle,
         List<PartMaterial> partMaterials,
         List<PurchaseOrderGroupLine> oldPurchaseOrderGroupLines,
         List<PurchaseOrderLine> oldPurchaseOrderLines,
         out List<PurchaseOrder> newPurchaseOrders,
         out List<PurchaseOrder> oldPurchaseOrders,
         out List<PurchaseOrderLine> purchaseOrderLines,
         out List<PurchaseOrderLine> newOldPurchaseOrderLines,
         out List<PurchaseOrderGroupLine> newPurchaseOrderGroupLines,
         out List<PurchaseOrderGroupLine> newOldPurchaseOrderGroupLines,
         out List<PurchaseOrderGroupLine> updatePurchaseOrderGroupLines,
         out string errorMessage)
        {
            errorMessage = string.Empty;
            newPurchaseOrders = new List<PurchaseOrder>();
            oldPurchaseOrders = new List<PurchaseOrder>();
            purchaseOrderLines = new List<PurchaseOrderLine>();
            newOldPurchaseOrderLines = new List<PurchaseOrderLine>();
            newPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            newOldPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            updatePurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            var newDataPurchaseOrderLines = new List<PurchaseOrderLine>();

            Dictionary<string, ItemStyle> dicContractNoItemStyle = itemStyle.ToDictionary(x => x.ContractNo);
            Dictionary<string, PartMaterial> dicItemCode = new Dictionary<string, PartMaterial>();

            foreach (var item in partMaterials)
            {
                if (!dicItemCode.ContainsKey(item.PartNumber + item.ItemStyleNumber + item.ItemID + item.ItemColorName))
                {
                    dicItemCode[item.PartNumber + item.ItemStyleNumber + item.ItemID + item.ItemColorName] = item;
                }
            }
            //var purchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();

            var config = new MapperConfiguration(
                cfg => cfg.CreateMap<PurchaseOrderLine, PurchaseOrderLine>()
                .ForMember(d => d.ID, o => o.Ignore())
                .ForMember(x => x.CreatedAt, y => y.Ignore())
                .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                .ForMember(x => x.CreatedBy, y => y.Ignore())
                .ForMember(x => x.LastUpdatedBy, y => y.Ignore())
                );

            var mapper = new Mapper(config);

            // loop for tracking           
            foreach (var trackingPO in dicTrackingPO)
            {
                if (dicPurchaseOrders.TryGetValue(trackingPO.Key, out PurchaseOrder rsPurchaseOrders))
                {
                    if (dicDetailPO.TryGetValue(trackingPO.Key, out List<PurchaseOrderLine> rsDetailPO))
                    {
                        bool checkPO = false;

                        var dicItemNo = new Dictionary<string, int>();

                        var dicCheckGroupPurchaseOrderLine = new Dictionary<string, PurchaseOrderLine>();

                        foreach (var item in oldPurchaseOrderLines)
                        {
                            var key = item.CustomerPurchaseOrderNumber + item.ItemID + item.ItemColorName + item.ContractNo;
                            if (!dicCheckGroupPurchaseOrderLine.ContainsKey(key))
                            {
                                dicCheckGroupPurchaseOrderLine[key] = item;
                            }
                        }

                        foreach (PurchaseOrderLine itemDetail in rsDetailPO)
                        {
                            string checkDupItem = itemDetail.CustomerPurchaseOrderNumber + itemDetail.ItemID + itemDetail.ItemColorName + itemDetail.ContractNo;

                            if (!dicCheckGroupPurchaseOrderLine.TryGetValue(checkDupItem, out PurchaseOrderLine rsPurchaseOrderLine))
                            {
                                foreach (var tracking in trackingPO.Value)
                                {
                                    if (!String.IsNullOrEmpty(tracking.GarmentSize))
                                    {
                                        PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                        mapper.Map(itemDetail, purchaseOrderLine);

                                        string keyItemNo = purchaseOrderLine.ItemID + purchaseOrderLine.CustomerPurchaseOrderNumber;
                                        if (!dicItemNo.ContainsKey(keyItemNo))
                                        {
                                            var number = dicItemNo.Keys.Count + 1;
                                            dicItemNo[keyItemNo] = number;

                                            purchaseOrderLine.ItemNo = number.ToString();
                                        }
                                        else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                                        {
                                            purchaseOrderLine.ItemNo = itemNoNumber.ToString();
                                        }

                                        if (dicItemCode.TryGetValue(purchaseOrderLine.ContractNo + purchaseOrderLine.CustomerStyle + purchaseOrderLine.ItemID + purchaseOrderLine.ItemColorName, out PartMaterial rsMaterial))
                                        {
                                            purchaseOrderLine.ItemCode = rsMaterial.ItemCode;
                                        }

                                        if (dicContractNoItemStyle.TryGetValue(purchaseOrderLine.ContractNo, out ItemStyle rsItemStyle))
                                        {
                                            purchaseOrderLine.LSStyle = rsItemStyle.LSStyle;
                                            purchaseOrderLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                            purchaseOrderLine.GarmentColorCode = rsItemStyle.ColorCode;
                                            purchaseOrderLine.GarmentColorName = rsItemStyle.ColorName;
                                            purchaseOrderLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                        }

                                        purchaseOrderLine.WareHouseQuantity = purchaseOrderLine.Quantity;
                                        purchaseOrderLine.VendorConfirmDate = tracking.VendorConfirmDate;
                                        purchaseOrderLine.ShipDate = tracking.ShipDate;
                                        purchaseOrderLine.EstimateShipDate = tracking.EstimateShipDate;
                                        purchaseOrderLine.InvoiceNo = tracking.InvoiceNo;
                                        purchaseOrderLine.Remarks = tracking.Remarks;
                                        purchaseOrderLine.OrderQuantityTracking = tracking.Quantity;
                                        purchaseOrderLine.CustomerPurchaseOrderNumber = rsPurchaseOrders.Number;

                                        newDataPurchaseOrderLines.Add(purchaseOrderLine);
                                        checkPO = true;
                                        break;
                                    }
                                    else if (String.IsNullOrEmpty(tracking.GarmentSize) &&
                                        !String.IsNullOrEmpty(tracking.GarmentColorCode) &&
                                        tracking.GarmentColorCode.Equals(itemDetail.ItemColorName) &&
                                        tracking.ItemID.Equals(itemDetail.ItemID))
                                    {
                                        PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                        mapper.Map(itemDetail, purchaseOrderLine);

                                        string keyItemNo = purchaseOrderLine.ItemID + purchaseOrderLine.CustomerPurchaseOrderNumber;
                                        if (!dicItemNo.ContainsKey(keyItemNo))
                                        {
                                            var number = dicItemNo.Keys.Count + 1;
                                            dicItemNo[keyItemNo] = number;

                                            purchaseOrderLine.ItemNo = number.ToString();
                                        }
                                        else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                                        {
                                            purchaseOrderLine.ItemNo = itemNoNumber.ToString();
                                        }

                                        if (dicItemCode.TryGetValue(purchaseOrderLine.ContractNo + purchaseOrderLine.CustomerStyle + purchaseOrderLine.ItemID + purchaseOrderLine.ItemColorName, out PartMaterial rsMaterial))
                                        {
                                            purchaseOrderLine.ItemCode = rsMaterial.ItemCode;
                                        }

                                        if (dicContractNoItemStyle.TryGetValue(purchaseOrderLine.ContractNo, out ItemStyle rsItemStyle))
                                        {
                                            purchaseOrderLine.LSStyle = rsItemStyle.LSStyle;
                                            purchaseOrderLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                            purchaseOrderLine.GarmentColorCode = rsItemStyle.ColorCode;
                                            purchaseOrderLine.GarmentColorName = rsItemStyle.ColorName;
                                            purchaseOrderLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                        }

                                        //purchaseOrderLine.Quantity = productionBOM.RequiredQuantity;
                                        //purchaseOrderLine.ReservedQuantity = productionBOM.RequiredQuantity;
                                        //purchaseOrderLine.WastageQuantity = productionBOM.WastageQuantity;
                                        purchaseOrderLine.WareHouseQuantity = purchaseOrderLine.Quantity;
                                        purchaseOrderLine.VendorConfirmDate = tracking.VendorConfirmDate;
                                        purchaseOrderLine.ShipDate = tracking.ShipDate;
                                        purchaseOrderLine.EstimateShipDate = tracking.EstimateShipDate;
                                        purchaseOrderLine.InvoiceNo = tracking.InvoiceNo;
                                        purchaseOrderLine.Remarks = tracking.Remarks;
                                        purchaseOrderLine.OrderQuantityTracking = tracking.Quantity;

                                        purchaseOrderLine.CustomerPurchaseOrderNumber = rsPurchaseOrders.Number;

                                        newDataPurchaseOrderLines.Add(purchaseOrderLine);
                                        checkPO = true;
                                        break;
                                    }
                                    else if (String.IsNullOrEmpty(tracking.GarmentSize) && String.IsNullOrEmpty(tracking.GarmentColorCode))
                                    {
                                        PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                        mapper.Map(itemDetail, purchaseOrderLine);

                                        string keyItemNo = purchaseOrderLine.ItemID + purchaseOrderLine.CustomerPurchaseOrderNumber;
                                        if (!dicItemNo.ContainsKey(keyItemNo))
                                        {
                                            var number = dicItemNo.Keys.Count + 1;
                                            dicItemNo[keyItemNo] = number;

                                            purchaseOrderLine.ItemNo = number.ToString();
                                        }
                                        else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                                        {
                                            purchaseOrderLine.ItemNo = itemNoNumber.ToString();
                                        }

                                        if (dicItemCode.TryGetValue(purchaseOrderLine.ContractNo + purchaseOrderLine.CustomerStyle + purchaseOrderLine.ItemID + purchaseOrderLine.ItemColorName, out PartMaterial rsMaterial))
                                        {
                                            purchaseOrderLine.ItemCode = rsMaterial.ItemCode;
                                        }

                                        if (dicContractNoItemStyle.TryGetValue(purchaseOrderLine.ContractNo, out ItemStyle rsItemStyle))
                                        {
                                            purchaseOrderLine.LSStyle = rsItemStyle.LSStyle;
                                            purchaseOrderLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                            purchaseOrderLine.GarmentColorCode = rsItemStyle.ColorCode;
                                            purchaseOrderLine.GarmentColorName = rsItemStyle.ColorName;
                                            purchaseOrderLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                        }

                                        //purchaseOrderLine.Quantity = productionBOM.RequiredQuantity;
                                        //purchaseOrderLine.ReservedQuantity = productionBOM.RequiredQuantity;
                                        //purchaseOrderLine.WastageQuantity = productionBOM.WastageQuantity;
                                        purchaseOrderLine.WareHouseQuantity = purchaseOrderLine.Quantity;
                                        purchaseOrderLine.VendorConfirmDate = tracking.VendorConfirmDate;
                                        purchaseOrderLine.ShipDate = tracking.ShipDate;
                                        purchaseOrderLine.EstimateShipDate = tracking.EstimateShipDate;
                                        purchaseOrderLine.InvoiceNo = tracking.InvoiceNo;
                                        purchaseOrderLine.Remarks = tracking.Remarks;
                                        purchaseOrderLine.OrderQuantityTracking = tracking.Quantity;

                                        purchaseOrderLine.CustomerPurchaseOrderNumber = rsPurchaseOrders.Number;


                                        newDataPurchaseOrderLines.Add(purchaseOrderLine);
                                        checkPO = true;
                                        break;
                                    }
                                }
                            }
                            //else
                            //{

                            //}

                        }

                        if (checkPO)
                        {
                            oldPurchaseOrders.Add(rsPurchaseOrders);
                        }

                    }
                }
                else
                {
                    if (dicDetailPO.TryGetValue(trackingPO.Key, out List<PurchaseOrderLine> rsDetailPO))
                    {
                        PurchaseOrder purchaseOrder = new PurchaseOrder();
                        purchaseOrder.Number = trackingPO.Key;
                        purchaseOrder.OrderDate = DateTime.Now;
                        purchaseOrder.CustomerID = "PU";
                        //purchaseOrder.VendorID = "PU";
                        purchaseOrder.CurrencyID = "VND";
                        purchaseOrder.CompanyCode = "LS";
                        purchaseOrder.SetCreateAudit(userName);
                        purchaseOrder.Description = trackingPO.Value.First()?.Remarks;
                        purchaseOrder.ShipDate = trackingPO.Value.First()?.ShipDate;

                        bool checkPO = false;

                        var dicItemNo = new Dictionary<string, int>();

                        foreach (PurchaseOrderLine itemDetail in rsDetailPO)
                        {
                            purchaseOrder.VendorID = itemDetail.CustomerSupplier;

                            foreach (var tracking in trackingPO.Value)
                            {
                                if (!String.IsNullOrEmpty(tracking.GarmentSize))
                                {
                                    PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                    mapper.Map(itemDetail, purchaseOrderLine);

                                    string keyItemNo = purchaseOrderLine.ItemID + purchaseOrderLine.CustomerPurchaseOrderNumber;
                                    if (!dicItemNo.ContainsKey(keyItemNo))
                                    {
                                        var number = dicItemNo.Keys.Count + 1;
                                        dicItemNo[keyItemNo] = number;

                                        purchaseOrderLine.ItemNo = number.ToString();
                                    }
                                    else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                                    {
                                        purchaseOrderLine.ItemNo = itemNoNumber.ToString();
                                    }

                                    if (dicItemCode.TryGetValue(purchaseOrderLine.ContractNo + purchaseOrderLine.CustomerStyle + purchaseOrderLine.ItemID + purchaseOrderLine.ItemColorName, out PartMaterial rsMaterial))
                                    {
                                        purchaseOrderLine.ItemCode = rsMaterial.ItemCode;
                                    }

                                    if (dicContractNoItemStyle.TryGetValue(purchaseOrderLine.ContractNo, out ItemStyle rsItemStyle))
                                    {
                                        purchaseOrderLine.LSStyle = rsItemStyle.LSStyle;
                                        purchaseOrderLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                        purchaseOrderLine.GarmentColorCode = rsItemStyle.ColorCode;
                                        purchaseOrderLine.GarmentColorName = rsItemStyle.ColorName;
                                        purchaseOrderLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                    }

                                    purchaseOrderLine.WareHouseQuantity = purchaseOrderLine.Quantity;
                                    purchaseOrderLine.VendorConfirmDate = tracking.VendorConfirmDate;
                                    purchaseOrderLine.ShipDate = tracking.ShipDate;
                                    purchaseOrderLine.EstimateShipDate = tracking.EstimateShipDate;
                                    purchaseOrderLine.InvoiceNo = tracking.InvoiceNo;
                                    purchaseOrderLine.Remarks = tracking.Remarks;
                                    purchaseOrderLine.OrderQuantityTracking = tracking.Quantity;
                                    purchaseOrderLine.CustomerPurchaseOrderNumber = purchaseOrder.Number;

                                    purchaseOrderLines.Add(purchaseOrderLine);
                                    checkPO = true;
                                    break;
                                }
                                else if (String.IsNullOrEmpty(tracking.GarmentSize) &&
                                    !String.IsNullOrEmpty(tracking.GarmentColorCode) &&
                                    tracking.GarmentColorCode.Equals(itemDetail.ItemColorName) &&
                                    tracking.ItemID.Equals(itemDetail.ItemID))
                                {
                                    PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                    mapper.Map(itemDetail, purchaseOrderLine);

                                    string keyItemNo = purchaseOrderLine.ItemID + purchaseOrderLine.CustomerPurchaseOrderNumber;
                                    if (!dicItemNo.ContainsKey(keyItemNo))
                                    {
                                        var number = dicItemNo.Keys.Count + 1;
                                        dicItemNo[keyItemNo] = number;

                                        purchaseOrderLine.ItemNo = number.ToString();
                                    }
                                    else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                                    {
                                        purchaseOrderLine.ItemNo = itemNoNumber.ToString();
                                    }

                                    if (dicItemCode.TryGetValue(purchaseOrderLine.ContractNo + purchaseOrderLine.CustomerStyle + purchaseOrderLine.ItemID + purchaseOrderLine.ItemColorName, out PartMaterial rsMaterial))
                                    {
                                        purchaseOrderLine.ItemCode = rsMaterial.ItemCode;
                                    }

                                    if (dicContractNoItemStyle.TryGetValue(purchaseOrderLine.ContractNo, out ItemStyle rsItemStyle))
                                    {
                                        purchaseOrderLine.LSStyle = rsItemStyle.LSStyle;
                                        purchaseOrderLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                        purchaseOrderLine.GarmentColorCode = rsItemStyle.ColorCode;
                                        purchaseOrderLine.GarmentColorName = rsItemStyle.ColorName;
                                        purchaseOrderLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                    }

                                    //purchaseOrderLine.Quantity = productionBOM.RequiredQuantity;
                                    //purchaseOrderLine.ReservedQuantity = productionBOM.RequiredQuantity;
                                    //purchaseOrderLine.WastageQuantity = productionBOM.WastageQuantity;
                                    purchaseOrderLine.WareHouseQuantity = purchaseOrderLine.Quantity;
                                    purchaseOrderLine.VendorConfirmDate = tracking.VendorConfirmDate;
                                    purchaseOrderLine.ShipDate = tracking.ShipDate;
                                    purchaseOrderLine.EstimateShipDate = tracking.EstimateShipDate;
                                    purchaseOrderLine.InvoiceNo = tracking.InvoiceNo;
                                    purchaseOrderLine.Remarks = tracking.Remarks;
                                    purchaseOrderLine.OrderQuantityTracking = tracking.Quantity;

                                    purchaseOrderLine.CustomerPurchaseOrderNumber = purchaseOrder.Number;

                                    purchaseOrderLines.Add(purchaseOrderLine);
                                    checkPO = true;
                                    break;
                                }
                                else if (String.IsNullOrEmpty(tracking.GarmentSize) && String.IsNullOrEmpty(tracking.GarmentColorCode))
                                {
                                    PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                    mapper.Map(itemDetail, purchaseOrderLine);

                                    string keyItemNo = purchaseOrderLine.ItemID + purchaseOrderLine.CustomerPurchaseOrderNumber;
                                    if (!dicItemNo.ContainsKey(keyItemNo))
                                    {
                                        var number = dicItemNo.Keys.Count + 1;
                                        dicItemNo[keyItemNo] = number;

                                        purchaseOrderLine.ItemNo = number.ToString();
                                    }
                                    else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                                    {
                                        purchaseOrderLine.ItemNo = itemNoNumber.ToString();
                                    }

                                    if (dicItemCode.TryGetValue(purchaseOrderLine.ContractNo + purchaseOrderLine.CustomerStyle + purchaseOrderLine.ItemID + purchaseOrderLine.ItemColorName, out PartMaterial rsMaterial))
                                    {
                                        purchaseOrderLine.ItemCode = rsMaterial.ItemCode;
                                    }

                                    if (dicContractNoItemStyle.TryGetValue(purchaseOrderLine.ContractNo, out ItemStyle rsItemStyle))
                                    {
                                        purchaseOrderLine.LSStyle = rsItemStyle.LSStyle;
                                        purchaseOrderLine.SalesOrderID = rsItemStyle.SalesOrderID;
                                        purchaseOrderLine.GarmentColorCode = rsItemStyle.ColorCode;
                                        purchaseOrderLine.GarmentColorName = rsItemStyle.ColorName;
                                        purchaseOrderLine.CustomerStyle = rsItemStyle.CustomerStyle;
                                    }

                                    //purchaseOrderLine.Quantity = productionBOM.RequiredQuantity;
                                    //purchaseOrderLine.ReservedQuantity = productionBOM.RequiredQuantity;
                                    //purchaseOrderLine.WastageQuantity = productionBOM.WastageQuantity;
                                    purchaseOrderLine.WareHouseQuantity = purchaseOrderLine.Quantity;
                                    purchaseOrderLine.VendorConfirmDate = tracking.VendorConfirmDate;
                                    purchaseOrderLine.ShipDate = tracking.ShipDate;
                                    purchaseOrderLine.EstimateShipDate = tracking.EstimateShipDate;
                                    purchaseOrderLine.InvoiceNo = tracking.InvoiceNo;
                                    purchaseOrderLine.Remarks = tracking.Remarks;
                                    purchaseOrderLine.OrderQuantityTracking = tracking.Quantity;

                                    purchaseOrderLine.CustomerPurchaseOrderNumber = purchaseOrder.Number;

                                    purchaseOrderLines.Add(purchaseOrderLine);
                                    checkPO = true;
                                    break;
                                }
                            }
                        }

                        if (checkPO)
                        {
                            newPurchaseOrders.Add(purchaseOrder);
                        }

                    }
                }
            }

            if (oldPurchaseOrderGroupLines.Any())
            {
                GroupOldPurchaseOrderLines(ref newDataPurchaseOrderLines, oldPurchaseOrderGroupLines, dicPurchaseOrders,
                                out newOldPurchaseOrderGroupLines,
                                out updatePurchaseOrderGroupLines,
                                out newOldPurchaseOrderLines,
                                out List<PurchaseOrderLine> newPurchaseOrderLines);
            }

            // loop for InternationalOCL          
            foreach (var international in dicInternationalOCL)
            {
                if (dicPurchaseOrders.TryGetValue(international.Key, out PurchaseOrder rsPurchaseOrder))
                {
                    //if (dicTrackingPO.TryGetValue(international.Key, out List<PurchaseOrderLine> rsPurchaseOrders))
                    //{
                    rsPurchaseOrder.SetUpdateAudit(userName);
                    var dicItemNo = new Dictionary<string, int>();
                    bool checkPO = false;
                    foreach (PurchaseOrderLine itemInternational in international.Value)
                    {
                        //PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                        //mapper.Map(rsProductionBOMs, purchaseOrderLine);

                        string keyItemNo = itemInternational.ItemID + itemInternational.CustomerPurchaseOrderNumber;
                        if (!dicItemNo.ContainsKey(keyItemNo))
                        {
                            var number = dicItemNo.Keys.Count + 1;
                            dicItemNo[keyItemNo] = number;

                            itemInternational.ItemNo = number.ToString();
                        }
                        else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                        {
                            itemInternational.ItemNo = itemNoNumber.ToString();
                        }

                        if (dicContractNoItemStyle.TryGetValue(itemInternational.ContractNo, out ItemStyle rsItemStyle))
                        {
                            itemInternational.LSStyle = rsItemStyle.LSStyle;
                            itemInternational.SalesOrderID = rsItemStyle.SalesOrderID;
                            itemInternational.GarmentColorCode = rsItemStyle.ColorCode;
                            itemInternational.GarmentColorName = rsItemStyle.ColorName;
                            itemInternational.CustomerStyle = rsItemStyle.CustomerStyle;
                        }

                        if (dicItemCode.TryGetValue(itemInternational.ContractNo + itemInternational.CustomerStyle + itemInternational.ItemID + itemInternational.ItemColorName, out PartMaterial rsMaterial))
                        {
                            itemInternational.ItemCode = rsMaterial.ItemCode;
                        }

                        if (itemInternational.Quantity > 0)
                        {
                            itemInternational.CustomerPurchaseOrderNumber = rsPurchaseOrder.Number;
                            itemInternational.WareHouseQuantity = itemInternational.Quantity;

                            purchaseOrderLines.Add(itemInternational);
                            checkPO = true;
                        }
                    }

                    if (checkPO)
                    {
                        oldPurchaseOrders.Add(rsPurchaseOrder);
                    }
                }
                else
                {
                    PurchaseOrder purchaseOrder = new PurchaseOrder();
                    purchaseOrder.Number = international.Key;
                    purchaseOrder.OrderDate = DateTime.Now;
                    purchaseOrder.CustomerID = "PU";
                    purchaseOrder.VendorID = "PU";
                    purchaseOrder.CurrencyID = "VND";
                    purchaseOrder.CompanyCode = "LS";
                    purchaseOrder.Remark = "OCL";
                    purchaseOrder.SetCreateAudit(userName);
                    var dicItemNo = new Dictionary<string, int>();
                    bool checkPO = false;
                    foreach (PurchaseOrderLine itemInternational in international.Value)
                    {
                        //PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                        //mapper.Map(rsProductionBOMs, purchaseOrderLine);

                        string keyItemNo = itemInternational.ItemID + itemInternational.CustomerPurchaseOrderNumber;
                        if (!dicItemNo.ContainsKey(keyItemNo))
                        {
                            var number = dicItemNo.Keys.Count + 1;
                            dicItemNo[keyItemNo] = number;

                            itemInternational.ItemNo = number.ToString();
                        }
                        else if (dicItemNo.TryGetValue(keyItemNo, out int itemNoNumber))
                        {
                            itemInternational.ItemNo = itemNoNumber.ToString();
                        }

                        if (dicContractNoItemStyle.TryGetValue(itemInternational.ContractNo, out ItemStyle rsItemStyle))
                        {
                            itemInternational.LSStyle = rsItemStyle.LSStyle;
                            itemInternational.SalesOrderID = rsItemStyle.SalesOrderID;
                            itemInternational.GarmentColorCode = rsItemStyle.ColorCode;
                            itemInternational.GarmentColorName = rsItemStyle.ColorName;
                            itemInternational.CustomerStyle = rsItemStyle.CustomerStyle;
                        }

                        if (dicItemCode.TryGetValue(itemInternational.ContractNo + itemInternational.CustomerStyle + itemInternational.ItemID + itemInternational.ItemColorName, out PartMaterial rsMaterial))
                        {
                            itemInternational.ItemCode = rsMaterial.ItemCode;
                        }

                        if (itemInternational.Quantity > 0)
                        {
                            itemInternational.CustomerPurchaseOrderNumber = purchaseOrder.Number;
                            itemInternational.WareHouseQuantity = itemInternational.Quantity;

                            purchaseOrderLines.Add(itemInternational);
                            checkPO = true;
                        }
                    }

                    if (checkPO)
                    {
                        newPurchaseOrders.Add(purchaseOrder);
                    }
                }
            }

            GroupPurchaseOrderLines(purchaseOrderLines,
                out newPurchaseOrderGroupLines);

        }

        public static void GroupPurchaseOrderLines(List<PurchaseOrderLine> purchaseOrderLines,
            out List<PurchaseOrderGroupLine> purchaseOrderGroupLines)
        {
            purchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();

            //var config = new MapperConfiguration(
            //   cfg => cfg.CreateMap<ProductionBOM, PurchaseOrderLine>()
            //   .ForMember(d => d.ID, o => o.Ignore())
            //   );

            //var mapper = new Mapper(config);

            var configPOLine = new MapperConfiguration(
               cfg => cfg.CreateMap<PurchaseOrderLine, PurchaseOrderGroupLine>()
               .ForMember(d => d.ID, o => o.Ignore())
               );

            var mapperPOLine = new Mapper(configPOLine);

            if (purchaseOrderLines != null)
            {
                var newLines = purchaseOrderLines.Where(x => x.ID == 0).ToList();

                var dicPurchaseGroupLine = new Dictionary<string, PurchaseOrderGroupLine>();

                foreach (var purchaseOrderLine in newLines)
                {
                    string key = purchaseOrderLine.ItemID + purchaseOrderLine.ItemName + purchaseOrderLine.ItemColorCode + purchaseOrderLine.ItemColorName
                                + purchaseOrderLine.Specify + purchaseOrderLine.Position + purchaseOrderLine.CustomerStyle + purchaseOrderLine.GarmentColorCode
                                + purchaseOrderLine.GarmentColorName + purchaseOrderLine.CustomerPurchaseOrderNumber + purchaseOrderLine.GarmentSize;

                    //var groupLine = purchaseOrderGroupLines?
                    //    .FirstOrDefault(x => x.ItemID == purchaseOrderLine.ItemID &&
                    //                         x.ItemName == purchaseOrderLine.ItemName &&
                    //                         x.ItemColorCode == purchaseOrderLine.ItemColorCode &&
                    //                         x.ItemColorName == purchaseOrderLine.ItemColorName &&
                    //                         x.Specify == purchaseOrderLine.Specify &&
                    //                         x.Position == purchaseOrderLine.Position &&
                    //                         x.CustomerStyle == purchaseOrderLine.CustomerStyle &&
                    //                         x.GarmentColorName == purchaseOrderLine.GarmentColorName &&
                    //                         x.GarmentColorCode == purchaseOrderLine.GarmentColorCode &&
                    //                         x.CustomerPurchaseOrderNumber == purchaseOrderLine.CustomerPurchaseOrderNumber &&
                    //                         x.GarmentSize == purchaseOrderLine.GarmentSize);

                    if (dicPurchaseGroupLine.TryGetValue(key, out PurchaseOrderGroupLine rsValue))
                    {
                        rsValue.Quantity += purchaseOrderLine.Quantity;
                        rsValue.WastageQuantity += purchaseOrderLine.WastageQuantity;
                        rsValue.WareHouseQuantity += purchaseOrderLine.WareHouseQuantity;
                        rsValue.CustomerPurchaseOrderNumber = purchaseOrderLine.CustomerPurchaseOrderNumber;
                        rsValue.PurchaseOrderLines.Add(purchaseOrderLine);

                        dicPurchaseGroupLine[key] = rsValue;
                    }
                    else
                    {
                        var newGroupLine = mapperPOLine.Map<PurchaseOrderGroupLine>(purchaseOrderLine);
                        newGroupLine.WareHouseUnitID = purchaseOrderLine.SecondUnitID;
                        newGroupLine.CustomerPurchaseOrderNumber = purchaseOrderLine.CustomerPurchaseOrderNumber;

                        if (purchaseOrderGroupLines == null)
                            purchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();

                        newGroupLine.PurchaseOrderLines.Add(purchaseOrderLine);
                        dicPurchaseGroupLine[key] = newGroupLine;
                    }
                }

                foreach (var item in dicPurchaseGroupLine)
                {
                    purchaseOrderGroupLines.Add(item.Value);
                }
            }


        }

        public static void GroupOldPurchaseOrderLines(ref List<PurchaseOrderLine> purchaseOrderLines,
            List<PurchaseOrderGroupLine> oldPurchaseOrderGroupLines,
            Dictionary<string, PurchaseOrder> dicPurchaseOrders,
            out List<PurchaseOrderGroupLine> newPurchaseOrderGroupLines,
            out List<PurchaseOrderGroupLine> updatePurchaseOrderGroupLines,
            out List<PurchaseOrderLine> newOldPurchaseOrderLines,
            out List<PurchaseOrderLine> newPurchaseOrderLines)
        {
            newPurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            updatePurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
            newPurchaseOrderLines = new List<PurchaseOrderLine>();
            newOldPurchaseOrderLines = new List<PurchaseOrderLine>();
            var dicUpdatePurhaseGroupLine = new Dictionary<long, PurchaseOrderGroupLine>();

            var configPOLine = new MapperConfiguration(
               cfg => cfg.CreateMap<PurchaseOrderLine, PurchaseOrderGroupLine>()
               .ForMember(d => d.ID, o => o.Ignore())
               );

            var mapperPOLine = new Mapper(configPOLine);

            if (purchaseOrderLines != null)
            {
                var newLines = purchaseOrderLines.Where(x => x.ID == 0).ToList();

                var dicOldPurchaseGroupLine = new Dictionary<string, PurchaseOrderGroupLine>();
                var dicNewPurchaseGroupLine = new Dictionary<string, PurchaseOrderGroupLine>();

                foreach (var itemGroup in oldPurchaseOrderGroupLines)
                {
                    string key = itemGroup.ItemID + itemGroup.ItemName + itemGroup.ItemColorCode + itemGroup.ItemColorName
                                + itemGroup.Specify + itemGroup.Position + itemGroup.CustomerStyle + itemGroup.GarmentColorCode
                                + itemGroup.GarmentColorName + itemGroup.CustomerPurchaseOrderNumber + itemGroup.GarmentSize;

                    if (dicOldPurchaseGroupLine.TryGetValue(key, out PurchaseOrderGroupLine rs))
                    {
                        rs.Quantity += itemGroup.Quantity;
                        rs.WastageQuantity += itemGroup.WastageQuantity;
                        rs.WareHouseQuantity += itemGroup.WareHouseQuantity;
                        rs.CustomerPurchaseOrderNumber = itemGroup.CustomerPurchaseOrderNumber;

                        dicOldPurchaseGroupLine[key] = rs;
                    }
                    else
                    {
                        dicOldPurchaseGroupLine[key] = itemGroup;
                    }
                }

                foreach (var purchaseOrderLine in newLines)
                {
                    //var groupLine = oldPurchaseOrderGroupLines?
                    //    .FirstOrDefault(x => x.ItemID == purchaseOrderLine.ItemID &&
                    //                         x.ItemName == purchaseOrderLine.ItemName &&
                    //                         x.ItemColorCode == purchaseOrderLine.ItemColorCode &&
                    //                         x.ItemColorName == purchaseOrderLine.ItemColorName &&
                    //                         x.Specify == purchaseOrderLine.Specify &&
                    //                         x.Position == purchaseOrderLine.Position &&
                    //                         x.CustomerStyle == purchaseOrderLine.CustomerStyle &&
                    //                         x.GarmentColorName == purchaseOrderLine.GarmentColorName &&
                    //                         x.GarmentColorCode == purchaseOrderLine.GarmentColorCode &&
                    //                         x.CustomerPurchaseOrderNumber == purchaseOrderLine.CustomerPurchaseOrderNumber &&
                    //                         x.GarmentSize == purchaseOrderLine.GarmentSize &&
                    //                         x.ID > 0);

                    string key = purchaseOrderLine.ItemID + purchaseOrderLine.ItemName + purchaseOrderLine.ItemColorCode + purchaseOrderLine.ItemColorName
                                + purchaseOrderLine.Specify + purchaseOrderLine.Position + purchaseOrderLine.CustomerStyle + purchaseOrderLine.GarmentColorCode
                                + purchaseOrderLine.GarmentColorName + purchaseOrderLine.CustomerPurchaseOrderNumber + purchaseOrderLine.GarmentSize;

                    if (dicPurchaseOrders.TryGetValue(purchaseOrderLine.CustomerPurchaseOrderNumber, out PurchaseOrder rsPO))
                    {
                        purchaseOrderLine.PurchaseOrderID = rsPO.ID;
                    }

                    if (dicOldPurchaseGroupLine.TryGetValue(key, out PurchaseOrderGroupLine rsValue))
                    {
                        rsValue.Quantity += purchaseOrderLine.Quantity;
                        rsValue.WastageQuantity += purchaseOrderLine.WastageQuantity;
                        rsValue.WareHouseQuantity += purchaseOrderLine.WareHouseQuantity;
                        rsValue.CustomerPurchaseOrderNumber = purchaseOrderLine.CustomerPurchaseOrderNumber;

                        purchaseOrderLine.PurchaseOrderGroupLineID = rsValue.ID;

                        newOldPurchaseOrderLines.Add(purchaseOrderLine);
                        dicOldPurchaseGroupLine[key] = rsValue;
                    }
                    else
                    {

                        //var newGroupLine = newPurchaseOrderGroupLines?
                        //.FirstOrDefault(x => x.ItemID == purchaseOrderLine.ItemID &&
                        //                     x.ItemName == purchaseOrderLine.ItemName &&
                        //                     x.ItemColorCode == purchaseOrderLine.ItemColorCode &&
                        //                     x.ItemColorName == purchaseOrderLine.ItemColorName &&
                        //                     x.Specify == purchaseOrderLine.Specify &&
                        //                     x.Position == purchaseOrderLine.Position &&
                        //                     x.CustomerStyle == purchaseOrderLine.CustomerStyle &&
                        //                     x.GarmentColorName == purchaseOrderLine.GarmentColorName &&
                        //                     x.GarmentColorCode == purchaseOrderLine.GarmentColorCode &&
                        //                     x.CustomerPurchaseOrderNumber == purchaseOrderLine.CustomerPurchaseOrderNumber &&
                        //                     x.GarmentSize == purchaseOrderLine.GarmentSize &&
                        //                     x.ID == 0);

                        if (dicNewPurchaseGroupLine.TryGetValue(key, out PurchaseOrderGroupLine rsNewGroup))
                        {
                            rsNewGroup.Quantity += purchaseOrderLine.Quantity;
                            rsNewGroup.WastageQuantity += purchaseOrderLine.WastageQuantity;
                            rsNewGroup.WareHouseQuantity += purchaseOrderLine.WareHouseQuantity;
                            rsNewGroup.CustomerPurchaseOrderNumber = purchaseOrderLine.CustomerPurchaseOrderNumber;

                            newPurchaseOrderLines.Add(purchaseOrderLine);
                            rsNewGroup.PurchaseOrderLines.Add(purchaseOrderLine);

                            dicNewPurchaseGroupLine[key] = rsNewGroup;
                        }
                        else
                        {
                            var newGroup = mapperPOLine.Map<PurchaseOrderGroupLine>(purchaseOrderLine);
                            newGroup.WareHouseUnitID = purchaseOrderLine.SecondUnitID;
                            newGroup.CustomerPurchaseOrderNumber = purchaseOrderLine.CustomerPurchaseOrderNumber;

                            if (dicPurchaseOrders.TryGetValue(newGroup.CustomerPurchaseOrderNumber, out PurchaseOrder rsPurchase))
                            {
                                newGroup.PurchaseOrderID = rsPurchase.ID;
                            }

                            newPurchaseOrderLines.Add(purchaseOrderLine);
                            newGroup.PurchaseOrderLines.Add(purchaseOrderLine);
                            //newPurchaseOrderGroupLines.Add(newGroup);
                            dicNewPurchaseGroupLine[key] = newGroup;
                        }

                    }
                }

                foreach (var itemGroup in dicOldPurchaseGroupLine)
                {
                    updatePurchaseOrderGroupLines.Add(itemGroup.Value);
                }

                foreach (var itemGroup in dicNewPurchaseGroupLine)
                {
                    newPurchaseOrderGroupLines.Add(itemGroup.Value);
                }
            }
        }
    }
}
