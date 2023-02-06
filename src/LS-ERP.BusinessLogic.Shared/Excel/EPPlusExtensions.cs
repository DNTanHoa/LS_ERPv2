using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Helpers;
using LS_ERP.XAF.Module.Dtos.SalesOrder;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OfficeOpenXml.ExcelWorksheet;

namespace LS_ERP.BusinessLogic.Shared
{
    public static class EPPlusExtensions
    {
        private static string DE = "DE";
        private static string GA = "GA";
        private static string IFG = "IFG";
        private static string OS = "OS";
        private static string KA = "KA";
        private static string HA = "HA";
        private static string PU = "PU";
        private static string SOPU = "SOPU"; // Sales order PU
        private static string SCPU = "SCPU"; // Sales contract PU
        private static string BOM_IFG_GA_HA = "BOM_IFG_GA_HA";

        private static Dictionary<string, string> ImportTemplates = new Dictionary<string, string>
        {
            {DE, "Order creation date;Order; Production Line;From to stock;WO DO WoDo;Begin production Date;End production Date;" +
                 "Estimated supplier handover;Contractual Supplier Handover;Order Type;Sub-status;Model;Designation;Item;Size;PCB;UE;" +
                 "Packaging;IMAN Code;Ordered qty;Unit Price;Total Price;Delivery Place;Order status;Contractual Delivery Date;" +
                 "Shipment Type;Via;Season;Production Type" },
            {GA, "STYLE DESCRIPTION;ORDER NO; ORDER TYPE;WALMART STYLE#;GARAN STYLE#;COLOR #;COLOR NAME;ORDER QUANTITY;UNIT;Unit price;" +
                 "Fabric content;SHIP COLOR;SHIPPING STYLE#;Ship pack;ORDER QTY/SIZE;Shipment date" },
            {IFG,"BRAND;SEASON;Description;STYLE#;LS code;Color name;Color Code;GENDER;PO#;SHIP MODE;DISTRIBUTION;" +
                 "SHIP TO WH/DESTINATION;PACKING WAY;SHIP DATE;MSRP;Unit;Same Price" },
            {OS, "Customer Style;LS Style;PO;Description;Season;Gender;Contract Date;Delivery Place;Delivery Date;Color;Color Name;" +
                 "Unit;Same Price;Item No" },
            {KA, "OrderWebId;Order Date;Licensee Ord Id;Status;Sourcing;Licensee;Currency;CollectionId;CollectionName;StyleCode;" +
                 "TeamCode;ColorCode;SizeCode;SizeOrderId;SizeGroupName;StyleName;Gender;StyleGroup;StyleDescriptionName;ColorName;" +
                 "EANCode;Qty;Price;RequestedShipmentDate;Orderd (Val);Confirmed (Qty);Confirmed (Val);ConfirmedShipmentDate;" +
                 "FactoryName;Country of Origin;Country Of Shipment" },
            {HA, "Division;Season;Year;Ref#;Style;Style Name;Color;Label;Label Name;Fashion Color;Cost ($);Extension ($);PO#;" +
                 "DF/PO Placed;Original ETD;Ship Date;Ship Month;Days Late;FTY Date;ETA Date;Due Date;PO Qty;AIR/BOAT;Freight Type;" +
                 "Contractor;MSRP($);Brand(G1);Product(G2);Product Description;SubBrand(G3);Size(G4);Size Range;Size Configuration;" +
                 "Pack Ratio;Master Box Quantity;Number Of Bundles;Pieces In Bundle;Hang/Flat;Agent;Country" },
            {BOM_IFG_GA_HA, "Item No; Style; Color Garment Code; Color Garment Name; Material Code; Description;Color Code; Color Name; " +
                 "Garment Size; Garment Deben; Specification; Label; Division; Consumption; Unit (BOM); Position; " +
                 "Material Class; Price; Unit (Price); Currency; Vendor Code; Lead Time; Wastage (%); Less (%); Over (%); " +
                 "Fabrics Weight (g/m2); Fabrics Width (inch); Fabrics Width Cut (inch); Material Class Type; Free Percent" },
            {SCPU,"Year;Brand;Season;Division;Product(Top);Product(Bottom);PO No.;Cust. PO No.;Ultimate Code/PO No.;Style No.;Col Code;" +
                 "ColorExt;QTY;unit;FTY;Country Name;Destination;(Ult)Cust. Code;Contract No.;MRQDate;Order Placed Date;" +
                 "Factory Date(LCHD);Req Ship Mth;Job Order Ready Date;No of Emb / EMBOSS/DEBOSS(XE,XB);No of TRANSFER PRINT (TF);" +
                 "No of SCREEN PRINT(XS); No of WELDING/BONDING(XW); No of PAD PRINT(XP); No of LAZER/ATI PRODUCTION(XL); " +
                 "Gmt Lead Time;Mfr Lead Time;LongestMatl Lead time;Transit Lead time;Shipping Mark; Remarks;Updates" },
            {SOPU, "Generated Order No; PO NO;CUST CODE;CUST CO NO;UCUST CODE;UCUST CO NO;Cust;STYLE NO;STYLE DESC; COLOR; Col Description;" +
                   "Chinese;Country;ShipMode;CHD;Ship Date;DateGroup;FACTORY CODE;Cu_Season;SEASON;Season_Code;Factory_name;Prefix;	" +
                   "Bundled MRQ;Order_Type;ProgramCode;Shipping Mark;factory;Currency;SIZ;QTY;PRC"}
        };

        /// <summary>
        /// read data and generate LSStyle sales contract PU
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="userName"></param>
        /// <param name="existSalesContractDetails"></param>
        /// <param name="parts"></param>
        /// <param name="newParts"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static SalesContract ImportSalesContractPU(this ExcelWorksheet worksheet, string userName,
            List<SalesContractDetail> existSalesContractDetails,
            List<Part> parts, ref List<Part> newParts, out string errorMessage)
        {

            var salesContract = ReadDataSalesContractPU(worksheet, existSalesContractDetails, userName, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return null;
            }

            GenerateLSStyle(parts, salesContract.ContractDetails.ToList(), PU, out newParts);

            return salesContract;
        }

        public static SalesContract UpdateSalesContractPU(this ExcelWorksheet worksheet, SalesContract currentSalesContract,
            string userName, List<SalesContractDetail> existSalesContractDetails,
            List<Part> parts, ref List<Part> newParts, out List<SalesContractDetail> newSalesContractDetail,
            out string errorMessage)
        {
            newSalesContractDetail = new List<SalesContractDetail>();
            var newsalesContract = ReadDataSalesContractPU(worksheet, existSalesContractDetails, userName, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return null;
            }

            Dictionary<string, SalesContractDetail> dicSalesContractDetail = new Dictionary<string, SalesContractDetail>();
            try
            {
                dicSalesContractDetail = newsalesContract.ContractDetails.ToDictionary(x => x.ContractNo);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            var config = new MapperConfiguration(
                cfg => cfg.CreateMap<SalesContractDetail, SalesContractDetail>()
                .ForMember(d => d.ID, o => o.Ignore())
                .ForMember(d => d.SalesContractID, o => o.Ignore())
                .ForMember(d => d.LSStyle, o => o.Ignore())
                .ForMember(d => d.GarmentColorCode, o => o.Ignore())
                .ForMember(d => d.CustomerStyle, o => o.Ignore())
                .ForMember(d => d.Season, o => o.Ignore())
                .ForMember(d => d.SalesContract, o => o.Ignore()));

            var mapper = new Mapper(config);

            string checkContract = string.Empty;

            // update data current sales contract detail
            foreach (var contractDetail in currentSalesContract.ContractDetails)
            {
                if (dicSalesContractDetail.TryGetValue(contractDetail.ContractNo, out SalesContractDetail salesContractDetail))
                {
                    if (!string.IsNullOrEmpty(salesContractDetail.Updates) &&
                       !salesContractDetail.Updates.Replace(" ", "").ToUpper().Equals("NEWORDER"))
                    {
                        mapper.Map(salesContractDetail, contractDetail);
                        contractDetail.SetUpdateAudit(userName);
                        checkContract = checkContract + contractDetail.ContractNo + ",";
                    }
                }
            }

            // add new sale contract detail
            foreach (var keyValuePair in dicSalesContractDetail)
            {
                var existSalesContractDetail = existSalesContractDetails
                      .FirstOrDefault(x => x.ContractNo == keyValuePair.Value.ContractNo);

                if (existSalesContractDetail == null &&
                    !checkContract.Contains(keyValuePair.Key + ",") &&
                    !string.IsNullOrEmpty(keyValuePair.Value.Updates) &&
                    keyValuePair.Value.Updates.Replace(" ", "").ToUpper().Equals("NEWORDER"))
                {
                    keyValuePair.Value.SalesContractID = currentSalesContract.Number;
                    keyValuePair.Value.SalesContract = currentSalesContract;
                    keyValuePair.Value.SetCreateAudit(userName);
                    newSalesContractDetail.Add(keyValuePair.Value);
                }
            }

            GenerateLSStyle(parts, newSalesContractDetail, PU, out newParts);

            return currentSalesContract;
        }

        /// <summary>
        /// Read data excel sales contract for PU
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="existSalesContractDetails"></param>
        /// <param name="userName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static SalesContract ReadDataSalesContractPU(this ExcelWorksheet worksheet,
            List<SalesContractDetail> existSalesContractDetails, string userName, out string errorMessage)
        {
            var salesContract = new SalesContract();
            salesContract.ContractDetails = new List<SalesContractDetail>();
            errorMessage = string.Empty;

            if (!IsValidTemplate(SCPU, out errorMessage, worksheet))
            {
                return null;
            }

            int row = 0;
            if (worksheet.Dimension != null)
            {
                row = worksheet.Dimension.End.Row;
            }

            for (int i = 2; i <= row; i++)
            {
                int column = 1;
                try
                {
                    SalesContractDetail salesContractDetail = new SalesContractDetail();

                    salesContractDetail.Year = worksheet.Cells[i, column++].GetValue<int>(); // A
                    salesContractDetail.Brand = worksheet.Cells[i, column++].GetValue<string>(); // B
                    salesContractDetail.Season = worksheet.Cells[i, column++].GetValue<string>(); // C
                    salesContractDetail.Division = worksheet.Cells[i, column++].GetValue<string>(); // D
                    salesContractDetail.ProductTop = worksheet.Cells[i, column++].GetValue<string>(); // E

                    salesContractDetail.ProductBottom = worksheet.Cells[i, column++].GetValue<string>(); // F
                    salesContractDetail.PurchaseOrderNumber = worksheet.Cells[i, column++].GetValue<string>(); // G
                    salesContractDetail.CustomerPO = worksheet.Cells[i, column++].GetValue<string>(); // H
                    salesContractDetail.UltimateCode = worksheet.Cells[i, column++].GetValue<string>(); // I
                    salesContractDetail.CustomerStyle = worksheet.Cells[i, column++].GetValue<string>(); // J

                    salesContractDetail.GarmentColorCode = worksheet.Cells[i, column++].GetValue<string>(); // K
                    salesContractDetail.GarmentColorName = worksheet.Cells[i, column++].GetValue<string>(); // L
                    salesContractDetail.Quantity = worksheet.Cells[i, column++].GetValue<decimal>(); // M
                    salesContractDetail.UnitID = worksheet.Cells[i, column++].GetValue<string>(); // N
                    salesContractDetail.Factory = worksheet.Cells[i, column++].GetValue<string>(); // O

                    salesContractDetail.CountryName = worksheet.Cells[i, column++].GetValue<string>(); // O
                    salesContractDetail.Destination = worksheet.Cells[i, column++].GetValue<string>(); // Q
                    salesContractDetail.UCustomterCode = worksheet.Cells[i, column++].GetValue<string>(); // R
                    salesContractDetail.ContractNo = worksheet.Cells[i, column++].GetValue<string>().Trim(); // S
                    salesContractDetail.MRQDate = worksheet.Cells[i, column++].GetValue<DateTime>(); // T

                    salesContractDetail.OrderPlacedDate = worksheet.Cells[i, column++].GetValue<DateTime>(); // U
                    salesContractDetail.FactoryDate = worksheet.Cells[i, column++].GetValue<DateTime>(); // V
                    salesContractDetail.ReqShipMonth = worksheet.Cells[i, column++].GetValue<string>(); // W
                    salesContractDetail.OrderReadyDate = worksheet.Cells[i, column++].GetValue<string>();// X

                    salesContractDetail.Emboss = worksheet.Cells[i, column++].GetValue<int>(); // Y
                    salesContractDetail.Transfer = worksheet.Cells[i, column++].GetValue<int>(); // Z
                    salesContractDetail.Screen = worksheet.Cells[i, column++].GetValue<int>(); // AA
                    salesContractDetail.Bonding = worksheet.Cells[i, column++].GetValue<int>(); // AB
                    salesContractDetail.Pad = worksheet.Cells[i, column++].GetValue<int>(); // AC
                    salesContractDetail.Lazer = worksheet.Cells[i, column++].GetValue<int>(); // AD

                    salesContractDetail.GmtLeadTime = worksheet.Cells[i, column++].GetValue<int>(); // AE
                    salesContractDetail.MfrLeadTime = worksheet.Cells[i, column++].GetValue<int>(); // AF
                    salesContractDetail.LongestMaterialLeadTime = worksheet.Cells[i, column++].GetValue<int>(); // AG
                    salesContractDetail.TransitLeadTime = worksheet.Cells[i, column++].GetValue<int>(); // AH

                    salesContractDetail.ShippingMark = worksheet.Cells[i, column++].GetValue<string>(); // AI
                    salesContractDetail.Remark = worksheet.Cells[i, column++].GetValue<string>(); // AJ
                    salesContractDetail.Updates = worksheet.Cells[i, column++].GetValue<string>(); // AK

                    var existSalesContractDetail = existSalesContractDetails
                        .FirstOrDefault(x => x.ContractNo == salesContractDetail.ContractNo);

                    if (existSalesContractDetail != null &&
                        string.IsNullOrEmpty(salesContractDetail.Updates))
                    {
                        errorMessage = "Contract No " + salesContractDetail.ContractNo + " has exist";
                        return null;
                    }
                    salesContractDetail.SetCreateAudit(userName);
                    salesContract.ContractDetails.Add(salesContractDetail);
                }
                catch (Exception ex)
                {

                    Fail(ex, worksheet, column, out errorMessage);

                }
            }

            return salesContract;
        }

        /// <summary>
        /// Read data update purchase order number PO
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="existSalesContractDetails"></param>
        /// <param name="userName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static List<SalesContractDetail> ReadDataUpdatePONumberSalesContractPU(this ExcelWorksheet worksheet,
            IQueryable<SalesContractDetail> currentSaleContractDetail,
            IQueryable<ItemStyle> itemStyles,
            string userName,
            out List<ItemStyle> updateItemStyles,
            out string errorMessage)
        {
            var salesContract = new SalesContract();
            var salesContractDetails = new List<SalesContractDetail>();
            updateItemStyles = new List<ItemStyle>();
            errorMessage = string.Empty;
            var dicCurrentSalesContractDetail = currentSaleContractDetail.ToDictionary(x => x.ContractNo);
            var dicCurrentItemStyle = new Dictionary<string, List<ItemStyle>>();

            foreach (var item in itemStyles)
            {
                if (dicCurrentItemStyle.TryGetValue(item.ContractNo, out List<ItemStyle> ItemStyles))
                {
                    ItemStyles.Add(item);
                }
                else
                {
                    var newListItem = new List<ItemStyle>();
                    newListItem.Add(item);
                    dicCurrentItemStyle.Add(item.ContractNo, newListItem);
                }
            }

            int row = 0;
            if (worksheet.Dimension != null)
            {
                row = worksheet.Dimension.End.Row;
            }

            for (int i = 2; i <= row; i++)
            {
                int column = 1;
                try
                {
                    SalesContractDetail salesContractDetail = new SalesContractDetail();

                    column += 3;
                    salesContractDetail.PurchaseOrderNumber = worksheet.Cells[i, column++].GetValue<string>(); // D
                    column += 10;
                    salesContractDetail.ContractNo = worksheet.Cells[i, column++].GetValue<string>(); // O

                    if (dicCurrentSalesContractDetail.TryGetValue(salesContractDetail.ContractNo,
                        out SalesContractDetail rsSalesContractDetail))
                    {
                        rsSalesContractDetail.PurchaseOrderNumber = salesContractDetail.PurchaseOrderNumber;
                        rsSalesContractDetail.SetUpdateAudit(userName);
                        salesContractDetails.Add(rsSalesContractDetail);
                    }

                    if (dicCurrentItemStyle.TryGetValue(salesContractDetail.ContractNo,
                        out List<ItemStyle> rsItemStyle))
                    {
                        foreach (var item in rsItemStyle)
                        {
                            item.PurchaseOrderNumber = salesContractDetail.PurchaseOrderNumber;
                            item.SetUpdateAudit(userName);
                            updateItemStyles.Add(item);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Fail(ex, worksheet, column, out errorMessage);
                }
            }

            return salesContractDetails;
        }

        /// <summary>
        /// Read data Sales order of Puma
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="userName"></param>
        /// <param name="salesContractDetails"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static SalesOrder ImportSalesOrderPU(this ExcelWorksheet worksheet, string userName,
            IQueryable<SalesContractDetail> salesContractDetails,
            IEnumerable<Size> sizes, out string errorMessage)
        {
            var salesOrder = new SalesOrder();

            Dictionary<string, SalesContractDetail> dicSalesContractDetail = new Dictionary<string, SalesContractDetail>();
            try
            {
                dicSalesContractDetail = salesContractDetails.ToDictionary(x => x.ContractNo.Trim());
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            Dictionary<string, Size> dicSizes = new Dictionary<string, Size>();
            try
            {
                dicSizes = sizes.ToDictionary(x => x.Code);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            errorMessage = string.Empty;

            if (!IsValidTemplate(SOPU, out errorMessage, worksheet))
            {
                return null;
            }

            int row = 0;
            if (worksheet.Dimension != null)
            {
                row = worksheet.Dimension.End.Row;
            }

            ItemStyle itemStyle = new ItemStyle();
            decimal totalQuantityContractStyle = 0;
            decimal totalQuantityImport = 0;
            itemStyle.ContractNo = string.Empty;
            itemStyle.SetCreateAudit(userName);
            for (int i = 2; i <= row; i++)
            {
                int column = 1;
                try
                {
                    string contractNo = worksheet.Cells[i, column++].GetValue<string>().Trim(); // A

                    if (dicSalesContractDetail.TryGetValue(contractNo, out SalesContractDetail salesContractDetail))
                    {

                        if (itemStyle.ContractNo != contractNo)
                        {
                            if (itemStyle.OrderDetails != null)
                            {
                                if (itemStyle.OrderDetails.Count > 0)
                                {
                                    if (totalQuantityContractStyle != totalQuantityImport)
                                    {
                                        errorMessage = "Contract " + itemStyle.ContractNo + " has incorrect quantity";
                                        break;
                                    }

                                    if (salesOrder.ItemStyles == null)
                                    {
                                        salesOrder.ItemStyles = new List<ItemStyle>();
                                    }

                                    itemStyle.TotalQuantity = totalQuantityContractStyle;
                                    salesOrder.ItemStyles.Add(itemStyle);
                                    itemStyle = new ItemStyle();
                                    itemStyle.UnitID = salesContractDetail.UnitID?.ToUpper().Trim();
                                    itemStyle.SetCreateAudit(userName);
                                    itemStyle.OrderDetails = new List<OrderDetail>();

                                }
                            }
                            else
                            {
                                itemStyle = new ItemStyle();
                                itemStyle.SetCreateAudit(userName);
                                itemStyle.UnitID = salesContractDetail.UnitID?.ToUpper().Trim();
                                itemStyle.OrderDetails = new List<OrderDetail>();
                            }

                            itemStyle.ContractNo = contractNo;
                            totalQuantityContractStyle = (decimal)salesContractDetail.Quantity;
                            itemStyle.LSStyle = salesContractDetail.LSStyle;
                            itemStyle.UnitID = salesContractDetail.UnitID?.ToUpper().Trim();
                            totalQuantityImport = 0;
                        }

                        itemStyle.ContractDate = salesContractDetail.OrderPlacedDate;

                        itemStyle.PurchaseOrderNumber = worksheet.Cells[i, column++].GetValue<string>(); // B
                        itemStyle.CustomerCode = worksheet.Cells[i, column++].GetValue<string>(); // C
                        itemStyle.CustomerCodeNo = worksheet.Cells[i, column++].GetValue<string>(); // D 
                        itemStyle.UCustomerCode = worksheet.Cells[i, column++].GetValue<string>(); // E
                        itemStyle.UCustomerCodeNo = worksheet.Cells[i, column++].GetValue<string>(); // F

                        column++; // G

                        itemStyle.CustomerStyle = worksheet.Cells[i, column++].GetValue<string>(); // H
                        itemStyle.Description = worksheet.Cells[i, column++].GetValue<string>(); // I
                        itemStyle.ColorCode = worksheet.Cells[i, column++].GetValue<string>(); // J
                        itemStyle.ColorName = worksheet.Cells[i, column++].GetValue<string>(); // K
                        itemStyle.ColorName = itemStyle.ColorName.Replace(itemStyle.ColorCode, "").Trim();

                        column++; // L
                        itemStyle.DeliveryPlace = worksheet.Cells[i, column++].GetValue<string>(); // M
                        itemStyle.ShipMode = worksheet.Cells[i, column++].GetValue<string>(); // N
                        column++; // O

                        itemStyle.DeliveryDate = worksheet.Cells[i, column++].GetValue<DateTime>(); // P

                        column++; // Q
                        column++; // R
                        column++; // S

                        itemStyle.Season = worksheet.Cells[i, column++].GetValue<string>(); // T

                        column++; // U
                        column++; // V
                        column++; // W
                        column++; // X
                        column++; // Y
                        column++; // Z

                        itemStyle.ShippingMark = worksheet.Cells[i, column++].GetValue<string>(); // AA
                        itemStyle.PriceTermCode = "CM";
                        column++; // AB

                        if (itemStyle.PurchaseOrderNumber != salesContractDetail.PurchaseOrderNumber)
                        {
                            errorMessage = "Contract " + itemStyle.ContractNo + " has PO No. incorrect";
                            return null;
                        }

                        if (itemStyle.CustomerStyle != salesContractDetail.CustomerStyle)
                        {
                            errorMessage = "Contract " + itemStyle.ContractNo + " has Style No incorrect";
                            return null;
                        }

                        string colorCode = itemStyle.ColorCode + " " + itemStyle.ColorName;

                        if (colorCode.Replace(" ", "") != salesContractDetail.GarmentColorCode.Replace(" ", ""))
                        {
                            errorMessage = "Contract " + itemStyle.ContractNo + " has Color Code incorrect";
                            return null;
                        }


                        salesOrder.CurrencyID = worksheet.Cells[i, column++].GetValue<string>(); // AC

                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.Size = worksheet.Cells[i, column++].GetValue<string>(); // AD
                        orderDetail.Quantity = worksheet.Cells[i, column++].GetValue<decimal>(); // AE
                        orderDetail.Price = worksheet.Cells[i, column++].GetValue<decimal>(); // AF

                        if (!string.IsNullOrEmpty(orderDetail.Size))
                        {
                            if (dicSizes.TryGetValue(orderDetail.Size, out Size size))
                            {
                                orderDetail.SizeSortIndex = size.SequeneceNumber;
                            }
                        }

                        totalQuantityImport += (decimal)orderDetail.Quantity;

                        itemStyle.OrderDetails?.Add(orderDetail);

                        if (itemStyle.OrderDetails.Count > 0 && i == row)
                        {
                            if (totalQuantityContractStyle != totalQuantityImport)
                            {
                                errorMessage = "Contract " + itemStyle.ContractNo + " has incorrect quantity ";
                                break;
                            }

                            if (salesOrder.ItemStyles == null)
                            {
                                salesOrder.ItemStyles = new List<ItemStyle>();
                            }

                            itemStyle.TotalQuantity = totalQuantityImport;
                            itemStyle.UnitID = salesContractDetail.UnitID?.ToUpper().Trim();
                            itemStyle.SetCreateAudit(userName);
                            salesOrder.ItemStyles.Add(itemStyle);
                        }
                    }
                    else
                    {
                        errorMessage = "Contract " + contractNo + " has incorrect quantity";
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Fail(ex, worksheet, column, out errorMessage);
                }
            }

            return salesOrder;
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
            else
            {
                var val = worksheet.Cells[2, column];
                if (val != null && !String.IsNullOrEmpty(val.Text))
                {

                    errorMessage = "Column " + val.Text + " have value incorrect format";
                }
            }
        }

        /// <summary>
        /// Import SaleOrder for customer KA
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="userName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static SalesOrder ImportSalesOrderKA(this ExcelWorksheet worksheet, string userName,
            IEnumerable<Size> sizes,
            List<Part> parts, out List<Part> newParts,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            newParts = new List<Part>();

            var table = worksheet.ToDataTable();

            string currentStyle = string.Empty;
            string currentColorCode = string.Empty;
            string currentSeason = string.Empty;
            ItemStyle itemStyle = null;

            SalesOrder salesOrder = new SalesOrder();

            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var size = GetCorrectSize(ref sizes, row["SizeCode"]?.ToString());

                    if (size == null)
                    {
                        errorMessage = "Can't not find size " + row["Size"]?.ToString();
                        return null;
                    }

                    if (row["StyleCode"]?.ToString() != currentStyle ||
                        row["ColorCode"]?.ToString() != currentColorCode)
                    {

                        if (itemStyle != null)
                            salesOrder.ItemStyles.Add(itemStyle);

                        itemStyle = new ItemStyle()
                        {
                            CustomerStyle = row["StyleCode"]?.ToString(),
                            ColorCode = row["ColorCode"]?.ToString(),
                            ColorName = row["ColorName"]?.ToString(),
                            Description = row["StyleName"]?.ToString(),
                            Gender = row["Gender"]?.ToString(),
                            DeliveryDate = DateTime.Parse(row["RequestedShipmentDate"]?.ToString()),
                            PurchaseOrderNumber = row["OrderWebId"]?.ToString(),
                            ItemStyleStatusCode = "4"
                        };

                        var orderDetail = new OrderDetail()
                        {
                            ItemStyleNumber = itemStyle?.Number,
                            Size = row["SizeCode"]?.ToString(),
                            Quantity = decimal.Parse(row["Qty"]?.ToString()),
                            SizeSortIndex = int.Parse(row["SizeOrderId"]?.ToString()),
                            Price = decimal.Parse(row["Price"]?.ToString())
                        };

                        var itemStyleBarcode = new ItemStyleBarCode()
                        {
                            ItemStyleNumber = itemStyle?.Number,
                            Size = row["sizeCode"]?.ToString(),
                            Color = row["ColorCode"]?.ToString(),
                            Quantity = decimal.Parse(row["Qty"]?.ToString()),
                            BarCode = row["EANCode"]?.ToString(),
                        };

                        itemStyle.TotalQuantity += orderDetail.Quantity;
                        itemStyle.OrderDetails.Add(orderDetail);
                        itemStyle.Barcodes.Add(itemStyleBarcode);
                        salesOrder.ItemStyles.Add(itemStyle);
                    }
                    else
                    {
                        var orderDetail = new OrderDetail()
                        {
                            ItemStyleNumber = itemStyle?.Number,
                            Size = row["SizeCode"]?.ToString(),
                            Quantity = decimal.Parse(row["Qty"]?.ToString()),
                            SizeSortIndex = int.Parse(row["SizeOrderId"]?.ToString()),
                            Price = decimal.Parse(row["Price"]?.ToString())
                        };

                        var itemStyleBarcode = new ItemStyleBarCode()
                        {
                            ItemStyleNumber = itemStyle?.Number,
                            Size = row["sizeCode"]?.ToString(),
                            Color = row["ColorCode"]?.ToString(),
                            Quantity = decimal.Parse(row["Qty"]?.ToString()),
                            BarCode = row["EANCode"]?.ToString(),
                        };

                        itemStyle.TotalQuantity += orderDetail.Quantity;
                        itemStyle.OrderDetails.Add(orderDetail);
                        itemStyle.Barcodes.Add(itemStyleBarcode);
                    }

                    GenerateLSStyle(parts, salesOrder.ItemStyles.ToList(), "KA", out newParts);

                    return salesOrder;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.InnerException?.Message;
                    LogHelper.Instance.Error("{@Datetime} ---Import sales order for KA error with message {@message}",
                        DateTime.Now.ToString(), ex.Message);
                    return null;
                }
            }

            return salesOrder;
        }

        public static SalesOrder ImportSalesOrderBS(this ExcelWorksheet worksheet, string userName,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            return null;
        }

        public static SalesOrder ImportSalesOrderCHA(this ExcelWorksheet worksheet, string userName,
            IEnumerable<Size> sizes,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            SalesOrder salesOrder = new SalesOrder();
            var table = worksheet.ToDataTable(5, true, 1);
            var sizeTable = worksheet.ToDataTable(startHeader: 5, startColumn: 16,
                endColumn: 50, startRow: 5);

            try
            {
                foreach (DataRow row in table.Rows)
                {
                    if (string.IsNullOrEmpty(row["STYLE#"]?.ToString()))
                        break;

                    var itemStyle = new ItemStyle
                    {
                        CustomerStyle = row["STYLE#"]?.ToString(),
                        LSStyle = row["LS STYLE"]?.ToString(),
                        ColorName = row["COLOR / SIZE"]?.ToString(),
                        LabelCode = row["LABEL"]?.ToString(),
                        PurchaseOrderNumber = row["PO#"]?.ToString(),
                        Gender = row["SIZES"]?.ToString().Split(' ')[0],
                        Brand = row["ACCOUNT"]?.ToString(),
                        HangFlat = row["H/F"]?.ToString(),
                        // = DateTime.Parse(row["PSDD"]?.ToString()),
                        //Remark = row["Remark"]?.ToString(),
                    };

                    foreach (DataColumn column in sizeTable.Columns)
                    {
                        if (column.ColumnName.Contains("Column"))
                            continue;

                        var quatityRow = sizeTable.Rows[table.Rows.IndexOf(row)];

                        if (string.IsNullOrEmpty(quatityRow[sizeTable.Columns.IndexOf(column)]?
                                    .ToString().Replace(".", "").Replace(",", "").Trim()))
                            continue;

                        var size = GetCorrectSize(ref sizes, column.ColumnName);
                        if (size == null)
                        {
                            errorMessage = "Can't not find size " + row["Size"]?.ToString();
                            return null;
                        }

                        var orderDetail = new OrderDetail()
                        {
                            Size = size.Code,
                            SizeSortIndex = size.SequeneceNumber,
                            Quantity = int.Parse(quatityRow[sizeTable.Columns.IndexOf(column)]?
                                    .ToString().Replace(".", "").Replace(",", "")),
                        };

                        itemStyle.OrderDetails.Add(orderDetail);
                    }

                    salesOrder.ItemStyles.Add(itemStyle);
                }

                return salesOrder;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                LogHelper.Instance.Error("{@Datetime} ---Import sales order for CHA error with message {@message}",
                    DateTime.Now.ToString(), ex.Message);
                return null;
            }
        }

        public static SalesOrder ImportSalesOrderHM(this ExcelWorksheet worksheet, string userName,
            IEnumerable<Size> sizes, List<PurchaseOrderType> purchaseOrderTypes,
            List<Part> parts, out List<Part> newParts, out List<PurchaseOrderType> newPurchaseOrderTypes,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            newParts = new List<Part>();
            newPurchaseOrderTypes = new List<PurchaseOrderType>();
            SalesOrder salesOrder = new SalesOrder();

            var table = worksheet.ToDataTable(startHeader: 2, true, 1);
            var sizeTable = worksheet.ToDataTable(startHeader: 2, startColumn: 15,
                endColumn: 50, startRow: 2);

            try
            {
                var index = 1;
                foreach (DataRow row in table.Rows)
                {
                    if (string.IsNullOrEmpty(row["STYLE#"]?.ToString()) &&
                        index > 1)
                        break;
                    else if (string.IsNullOrEmpty(row["STYLE#"]?.ToString()) &&
                        index == 1)
                    {
                        continue;
                    }

                    index += 1;

                    var parseShipDate = DateTime.TryParse(row["Shipment date"]?.ToString().Trim(),
                        out DateTime shipDate);

                    var itemStyle = new ItemStyle()
                    {
                        CustomerStyle = row["STYLE#"]?.ToString().Trim(),
                        ColorCode = row["COLOR #"]?.ToString().Trim(),
                        ColorName = row["COLOR NAME"]?.ToString().Trim(),
                        Description = row["STYLE DESCRIPTION"]?.ToString().Trim(),
                        PurchaseOrderNumber = row["ORDER NO"]?.ToString().Trim(),
                        LSStyle = row["LS STYLE#"]?.ToString().Trim(),
                        FabricContent = row["Fabric content"]?.ToString().Trim(),
                        UnitID = row["UNIT"]?.ToString().Trim(),
                        ShipDate = shipDate,
                        Remark = row["Remark"]?.ToString().Trim(),
                        Season = row["Season"]?.ToString().Trim(),
                        ExternalPurchaseOrderTypeCode = row["ORDER TYPE CODE"]?.ToString().Trim(),
                        ExternalPurchaseOrderTypeName = row["ORDER TYPE"]?.ToString().Trim(),
                    };

                    if (!string.IsNullOrEmpty(row["PSDD"]?.ToString()))
                    {
                        itemStyle.ProductionSkedDeliveryDate = DateTime.Parse(row["PSDD"]?.ToString());
                    }

                    decimal? price = null;
                    if (!string.IsNullOrEmpty(row["Unit price"]?.ToString().Trim()))
                        price = decimal.Parse(row["Unit price"]?.ToString().Trim());

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

                        var orderDetail = new OrderDetail()
                        {
                            Size = size.Code,
                            Quantity = int.Parse(quatityRow[sizeTable.Columns.IndexOf(column)]?
                                    .ToString().Replace(".", "").Replace(",", "")),
                            SizeSortIndex = size.SequeneceNumber,
                            Price = price
                        };

                        itemStyle.OrderDetails.Add(orderDetail);
                    }

                    salesOrder.ItemStyles.Add(itemStyle);
                }

                return salesOrder;
            }
            catch (Exception ex)
            {
                newParts = null;
                LogHelper.Instance.Error("{@Datetime} ---Import sales order for HM error with message {@message}",
                    DateTime.Now.ToString(), ex.Message);
                return null;
            }
        }

        public static SalesOrder ImportSalesOrderDE(this ExcelWorksheet worksheet, string userName,
            IEnumerable<Size> sizes, List<PurchaseOrderType> purchaseOrderTypes, List<ItemStyle> itemStyles,
            List<Part> parts, out List<Part> newParts, out List<PurchaseOrderType> newPurchaseOrderTypes,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            SalesOrder salesOrder = new SalesOrder();
            newPurchaseOrderTypes = new List<PurchaseOrderType>();
            newParts = new List<Part>();

            var table = worksheet.ToDataTable(7);
            string currentStyle = string.Empty;
            string currentColorCode = string.Empty;
            string currentSeason = string.Empty;
            string currentPurchaseOrderNumber = string.Empty;

            ItemStyle itemStyle = null;

            try
            {
                foreach (DataRow row in table.Rows)
                {
                    if (string.IsNullOrEmpty(row["IMAN Code"]?.ToString()))
                    {
                        if (itemStyle != null)
                            salesOrder.ItemStyles.Add(itemStyle);
                        break;
                    }

                    var size = GetCorrectSize(ref sizes, row["Size"]?.ToString());

                    if (size == null)
                    {
                        errorMessage = "Can't not find size " + row["Size"]?.ToString();
                        return null;
                    }

                    if (currentStyle != row["IMAN Code"]?.ToString() ||
                       currentColorCode != row["Model"]?.ToString() ||
                       currentSeason != row["Season"]?.ToString() ||
                       currentPurchaseOrderNumber != row["Order"]?.ToString())
                    {
                        currentStyle = row["IMAN Code"]?.ToString();
                        currentColorCode = row["Model"]?.ToString();
                        currentSeason = row["Season"]?.ToString();
                        currentPurchaseOrderNumber = row["Order"]?.ToString();

                        if (itemStyle != null)
                            salesOrder.ItemStyles.Add(itemStyle);

                        var existedOldCustomerPurchaseOrder = itemStyles
                            .FirstOrDefault(x => x.PurchaseOrderNumber == currentPurchaseOrderNumber);

                        if (existedOldCustomerPurchaseOrder != null)
                        {
                            errorMessage = "Order with number " + currentPurchaseOrderNumber + " has exist in system." +
                                " Please do not import duplicate";
                            return null;
                        }

                        itemStyle = new ItemStyle()
                        {
                            CustomerStyle = int.Parse(row["IMAN Code"]?.ToString()).ToString(),
                            PurchaseOrderNumber = row["Order"]?.ToString(),
                            ColorCode = row["Model"]?.ToString(),
                            ColorName = row["Model"]?.ToString(),
                            Description = row["Designation"]?.ToString(),
                            ContractDate = DateTime.Parse(row["Contractual Supplier Handover"]?.ToString()),
                            EstimatedSupplierHandOver = DateTime.Parse(row["Estimated supplier handover"]?.ToString()),
                            ProductionSkedDeliveryDate = DateTime.Parse(row["Estimated supplier handover"]?.ToString()),
                            PurchaseOrderDate = DateTime.Parse(row["Order creation date"]?.ToString()),
                            PCB = row["PCB"]?.ToString(),
                            UE = row["UE"]?.ToString(),
                            DeliveryPlace = row["Delivery Place"]?.ToString(),
                            Season = row["Season"]?.ToString(),
                            PriceTermCode = row["Production Type"]?.ToString().Trim(),
                            ProductionDescription = row["Production Line"]?.ToString(),
                            ShipMode = row["Shipment Type"]?.ToString(),
                            DeliveryDate = DateTime.Parse(row["Contractual Delivery Date"]?.ToString())
                        };

                        var purchaseOrderType = purchaseOrderTypes
                            .FirstOrDefault(x => x.Name == row["Order Type"]?.ToString());

                        if (purchaseOrderType != null)
                        {
                            itemStyle.PurchaseOrderTypeCode = purchaseOrderType.Code;
                        }
                        else
                        {
                            purchaseOrderType = new PurchaseOrderType()
                            {
                                Code = Nanoid.Nanoid.Generate("12345467890", 3),
                                Name = row["Order Type"]?.ToString()
                            };
                            purchaseOrderType.SetCreateAudit(userName);

                            newPurchaseOrderTypes.Add(purchaseOrderType);
                            purchaseOrderTypes.Add(purchaseOrderType);

                            itemStyle.PurchaseOrderTypeCode = purchaseOrderType.Code;
                        }

                        if (string.IsNullOrEmpty(itemStyle.PriceTermCode))
                            itemStyle.PriceTermCode = "CM";

                        var itemStyleBarcode = new ItemStyleBarCode()
                        {
                            BarCode = row["Item"]?.ToString(),
                            PCB = row["PCB"]?.ToString(),
                            UE = row["UE"]?.ToString(),
                            Packing = row["Packaging"]?.ToString(),
                            Size = row["Size"]?.ToString(),
                            Quantity = decimal.Parse(row["Ordered qty"]?.ToString())
                        };

                        itemStyle.Barcodes.Add(itemStyleBarcode);

                        var orderDetail = new OrderDetail()
                        {
                            Size = size.Code,
                            SizeSortIndex = size.SequeneceNumber,
                            Quantity = decimal.Parse(row["Ordered qty"]?.ToString()),
                            Price = decimal.Parse(row["Unit Price"]?.ToString().Replace("USD", "").Trim())
                        };

                        itemStyle.OrderDetails.Add(orderDetail);
                    }
                    else
                    {
                        var itemStyleBarcode = new ItemStyleBarCode()
                        {
                            BarCode = row["Item"]?.ToString(),
                            PCB = row["PCB"]?.ToString(),
                            UE = row["UE"]?.ToString(),
                            Packing = row["Packaging"]?.ToString(),
                            Size = row["Size"]?.ToString(),
                            Quantity = decimal.Parse(row["Ordered qty"]?.ToString())
                        };

                        itemStyle.Barcodes.Add(itemStyleBarcode);

                        var orderDetail = new OrderDetail()
                        {
                            Size = size.Code,
                            SizeSortIndex = size.SequeneceNumber,
                            Quantity = decimal.Parse(row["Ordered qty"]?.ToString()),
                            Price = decimal.Parse(row["Unit Price"]?.ToString().Replace("USD", "").Trim())
                        };

                        itemStyle.OrderDetails.Add(orderDetail);

                        itemStyle.TotalQuantity = itemStyle.OrderDetails.Sum(x => x.Quantity);
                    }

                    if (table.Rows.IndexOf(row) == table.Rows.Count - 1)
                    {
                        if (itemStyle != null)
                            salesOrder.ItemStyles.Add(itemStyle);
                    }
                }

                GenerateLSStyle(parts, itemStyles: salesOrder.ItemStyles.ToList(), "DE", out newParts);

                return salesOrder;
            }
            catch (Exception ex)
            {
                errorMessage = ex.InnerException?.Message;
                LogHelper.Instance.Error("{@Datetime} ---Import sales order for DE error with message {@message}",
                    DateTime.Now.ToString(), ex.Message);
                return null;
            }
        }

        public static SalesOrder ImportSalesOrderGA(this ExcelWorksheet worksheet, string userName,
            IEnumerable<Size> sizes, List<PurchaseOrderType> purchaseOrderTypes, List<ItemStyle> itemStyles,
            List<Part> parts,
            out List<Part> newParts, out List<PurchaseOrderType> newPurchaseOrderTypes,
            out string errorMessage, bool isUpdate = false)
        {
            errorMessage = string.Empty;

            newParts = new List<Part>();
            newPurchaseOrderTypes = new List<PurchaseOrderType>();
            SalesOrder salesOrder = new SalesOrder();

            var table = worksheet.ToDataTable(2, true, 1);
            var sizeTable = worksheet.ToDataTable(startHeader: 2, startColumn: 18,
                endColumn: 50, startRow: 2);

            try
            {
                var dicLsstyle = itemStyles.ToDictionary(x => x.LSStyle);

                foreach (DataRow row in table.Rows)
                {
                    if (string.IsNullOrEmpty(row["STYLE#"]?.ToString()))
                        break;

                    var itemStyle = new ItemStyle()
                    {
                        CustomerStyle = row["STYLE#"]?.ToString().Trim(),
                        ColorCode = row["COLOR #"]?.ToString().Trim(),
                        ColorName = row["COLOR NAME"]?.ToString().Trim(),
                        Description = row["STYLE DESCRIPTION"]?.ToString().Trim(),
                        LSStyle = row["LS STYLE#"]?.ToString().Trim(),
                        ContractNo = row["WALMART STYLE#"]?.ToString().Trim(),
                        FabricContent = row["Fabric content"]?.ToString().Trim(),
                        UnitID = row["UNIT"]?.ToString().Trim(),
                        ShipDate = DateTime.Parse(row["Shipment date"]?.ToString().Trim()),
                        Remark = row["Remark"]?.ToString().Trim(),
                        Season = row["Season"]?.ToString().Trim(),
                        PurchaseOrderNumber = row["ORDER NO"]?.ToString().Trim(),
                        ExternalPurchaseOrderTypeCode = row["ORDER TYPE CODE"]?.ToString().Trim(),
                        ExternalPurchaseOrderTypeName = row["ORDER TYPE"]?.ToString().Trim(),
                        ShipColor = row["SHIP COLOR"]?.ToString().Trim(),
                        ShippingStyle = row["SHIPPING STYLE#"]?.ToString(),
                    };

                    if (!string.IsNullOrEmpty(row["PSDD"]?.ToString()))
                    {
                        itemStyle.ProductionSkedDeliveryDate = DateTime.Parse(row["PSDD"]?.ToString());
                    }

                    if (!isUpdate && dicLsstyle.ContainsKey(itemStyle.LSStyle))
                    {
                        errorMessage = itemStyle.LSStyle + " is duplicate, please re-check";
                        return null;
                    }
                    else
                    {
                        dicLsstyle[itemStyle.LSStyle] = itemStyle;
                    }

                    decimal? price = null;
                    if (!string.IsNullOrEmpty(row["Unit price"]?.ToString().Trim()))
                        price = decimal.Parse(row["Unit price"]?.ToString().Trim());

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

                        var orderDetail = new OrderDetail()
                        {
                            Size = size.Code,
                            Quantity = int.Parse(quatityRow[sizeTable.Columns.IndexOf(column)]?
                                    .ToString().Replace(".", "").Replace(",", "")),
                            SizeSortIndex = size.SequeneceNumber,
                            Price = price
                        };
                        itemStyle.SetCreateAudit(userName);
                        itemStyle.OrderDetails.Add(orderDetail);
                    }

                    salesOrder.ItemStyles.Add(itemStyle);
                }

                //GenerateLSStyle(parts, itemStyles: salesOrder.ItemStyles.ToList(), "GA", out newParts);

                return salesOrder;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                newParts = null;
                LogHelper.Instance.Error("{@Datetime} ---Import sales order for GA error with message {@message}",
                    DateTime.Now.ToString(), ex.Message);

                return null;
            }
        }

        public static SalesOrder ImportSalesOrderGP(this ExcelWorksheet worksheet, string userName,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            return null;
        }

        public static List<SalesOrder> ImportSalesOrderHA(this ExcelWorksheet worksheet, string userName,
            IEnumerable<Size> sizes,
            ref List<Part> parts,
            Dictionary<string, ItemStyle> dicItemStyleProductions,
            out List<Part> newParts,
            out string errorMessage,
            bool? isCompare = null)
        {
            errorMessage = string.Empty;
            List<SalesOrder> salesOrders = new List<SalesOrder>();
            Dictionary<string, List<ItemStyle>> dicItemStyle = new Dictionary<string, List<ItemStyle>>();
            var dicSizes = sizes.ToDictionary(x => x.Code.ToUpper().Replace(" ", "").Trim());
            List<ItemStyle> listItemStyle = new List<ItemStyle>();
            newParts = new List<Part>();
            Dictionary<string, string> dicCheckDupPONumber = new Dictionary<string, string>();
            var table = worksheet.ToDataTable(2);

            try
            {
                int i = 1;
                List<ItemStyle> itemStyleGenarate = new List<ItemStyle>();

                foreach (DataRow row in table.Rows)
                {
                    if (string.IsNullOrEmpty(row["Style"]?.ToString()))
                        break;

                    string PONumber = row["PO#"]?.ToString().Trim();
                    string POIndex = row["PO Index"]?.ToString().Trim();

                    if (!string.IsNullOrEmpty(POIndex) && !string.IsNullOrEmpty(PONumber) && dicCheckDupPONumber.TryGetValue(PONumber + POIndex, out string rsPONumber))
                    {
                        errorMessage = "PO Number (PO#): " + rsPONumber + " and PO Index " + POIndex + " be duplicated, please re-check";
                        break;
                    }

                    var itemStyle = new ItemStyle()
                    {
                        CustomerStyle = row["Style"]?.ToString(),
                        Division = row["Division"]?.ToString().Trim(),
                        Season = row["Season"]?.ToString().Trim(),
                        ContractNo = row["Ref#"]?.ToString().Trim(),
                        Description = row["Style Name"]?.ToString().Trim(),
                        ColorCode = row["Color"]?.ToString().Trim(),
                        ColorName = row["Fashion Color"]?.ToString().Trim(),
                        LabelCode = row["Label"]?.ToString().Trim(),
                        LabelName = row["Label Name"]?.ToString().Trim(),
                        PurchaseOrderNumber = PONumber,
                        DeliveryDate = DateTime.Parse(row["Original ETD"]?.ToString()),
                        ShipDate = DateTime.Parse(row["Ship Date"]?.ToString()),
                        ShipMode = row["AIR/BOAT"]?.ToString().Trim(),
                        Brand = row["SubBrand(G3)"]?.ToString().Trim(),
                        ProductionDescription = row["Product Description "]?.ToString().Trim(),
                        HangFlat = row["Hang/Flat"]?.ToString().Trim(),
                        Packing = row["Pack Ratio"]?.ToString().Trim(),
                        UE = row["Size Configuration"]?.ToString().Trim().Trim(),
                        ContractDate = DateTime.Now,
                        Year = row["Year"]?.ToString().Trim()
                    };

                    itemStyle.SetCreateAudit(userName);
                    itemStyle.ItemStyleStatusCode = ((int)SalesOrderCompareDto.StatusCompare.New).ToString();

                    if (dicItemStyleProductions.TryGetValue(itemStyle.PurchaseOrderNumber, out ItemStyle rsItemStyle))
                    {
                        itemStyle.LSStyle = rsItemStyle.LSStyle;
                        itemStyle.ItemStyleStatusCode = ((int)SalesOrderCompareDto.StatusCompare.Update).ToString();

                        itemStyle.SetUpdateAudit(userName);
                    }

                    if (!string.IsNullOrEmpty(itemStyle.ProductionDescription))
                    {
                        if (itemStyle.ProductionDescription.Contains("SETS") || itemStyle.ProductionDescription.Contains("SET")
                            || itemStyle.ProductionDescription.Contains("2PK")
                            || itemStyle.ProductionDescription.Contains("3PK")
                            || itemStyle.ProductionDescription.Contains("5PCS"))
                        {
                            itemStyle.UnitID = "SET";
                        }
                        else
                        {
                            itemStyle.UnitID = "PCS";
                        }
                    }

                    if (!string.IsNullOrEmpty(POIndex))
                    {
                        itemStyle.PurchaseOrderNumberIndex = int.Parse(POIndex);
                    }
                    else
                    {
                        itemStyle.PurchaseOrderNumberIndex = 0;
                    }

                    if (!string.IsNullOrEmpty(row["MSRP($)"]?.ToString()))
                    {
                        itemStyle.MSRP = decimal.Parse(row["MSRP($)"]?.ToString());
                    }
                    else
                    {
                        itemStyle.MSRP = 0;
                    }

                    var readSizes = row["Size Configuration"]?.ToString().Split(" - ").ToList();
                    var price = decimal.Parse(row["Cost ($)"]?.ToString());
                    var sizeRatios = itemStyle.Packing.Split("-")
                        .Select(x => int.Parse((!string.IsNullOrEmpty(x.Trim())) == true ? x.Trim() : "0")).ToList();
                    var totalRatio = sizeRatios.Sum(x => x);

                    itemStyle.PCB = totalRatio.ToString();

                    var totalQuantity = (int)(decimal.Parse(row["PO Qty"]?.ToString().Replace(",", "")));
                    itemStyle.TotalQuantity = totalQuantity;
                    int jj = 0;
                    foreach (var size in readSizes)
                    {
                        //var sizeRationIndex = readSizes.IndexOf(size) > readSizes.Count - 1 ?
                        //    readSizes.IndexOf(size) : readSizes.Count - 1;

                        var ratio = sizeRatios[jj];
                        if (ratio == 0)
                        {
                            ratio = totalRatio;
                        }

                        int sortIndex = 0;
                        if (dicSizes.TryGetValue(size.Replace(" ", "").ToUpper().Trim(), out Size rsSize))
                        {
                            sortIndex = (int)rsSize.SequeneceNumber;
                        }

                        var orderDetail = new OrderDetail()
                        {
                            Size = size.Trim(),
                            Price = price,
                            Quantity = totalQuantity * ratio / totalRatio,
                            SizeSortIndex = sortIndex

                        };

                        if (itemStyle.OrderDetails == null)
                        {
                            itemStyle.OrderDetails = new List<OrderDetail>();
                        }
                        itemStyle.OrderDetails.Add(orderDetail);
                        jj++;
                    }

                    string keySONumber = "SO.HADDAD." + itemStyle.ShipDate.Value.Year;

                    if (dicItemStyle.TryGetValue(keySONumber, out List<ItemStyle> rs))
                    {
                        rs.Add(itemStyle);
                        itemStyleGenarate.Add(itemStyle);
                    }
                    else
                    {
                        listItemStyle = new List<ItemStyle>();
                        itemStyleGenarate.Add(itemStyle);
                        listItemStyle.Add(itemStyle);
                        dicItemStyle.Add(keySONumber, listItemStyle);
                    }
                }

                itemStyleGenarate = itemStyleGenarate.OrderBy(x => x.CustomerStyle).ToList();

                if (isCompare == null)
                {
                    EPPlusExtensions.GenerateLSStyleNonSeason("HA", userName, ref parts, itemStyleGenarate, out newParts, out errorMessage);
                }

                foreach (var itemStyles in dicItemStyle)
                {
                    SalesOrder salesOrder = new SalesOrder();
                    salesOrder.ID = itemStyles.Key;
                    salesOrder.ItemStyles = new List<ItemStyle>();

                    foreach (var itemStyle in itemStyleGenarate)
                    {
                        string keySONumber = "SO.HADDAD." + itemStyle.ShipDate.Value.Year;

                        if (itemStyles.Key.Equals(keySONumber))
                        {
                            salesOrder.ItemStyles.Add(itemStyle);
                        }

                    }

                    //salesOrder.ItemStyles = genCodeItemStyles;
                    salesOrders.Add(salesOrder);

                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error("{@Datetime} ---Compare sales order for HA error with message {@message}",
                    DateTime.Now.ToString(), ex.Message);
                errorMessage = ex.Message;
            }

            return salesOrders;
        }

        public static List<ItemStyle> ImportProduction_HA(this ExcelWorksheet worksheet,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            List<ItemStyle> itemStyles = new List<ItemStyle>();
            Dictionary<string, ItemStyle> dicItemStyles = new Dictionary<string, ItemStyle>();

            var table = worksheet.ToDataTable(startHeader: 3, startColumn: 1, endColumn: 21, startRow: 3);

            try
            {
                List<ItemStyle> itemStyleGenarate = new List<ItemStyle>();

                foreach (DataRow row in table.Rows)
                {
                    if (string.IsNullOrEmpty(row["LS Style"]?.ToString()))
                        continue;

                    if (string.IsNullOrEmpty(row["Division"]?.ToString().Trim()))
                    {
                        break;
                    }

                    var itemStyle = new ItemStyle()
                    {
                        CustomerStyle = row["STYLE"]?.ToString(),
                        Division = row["Division"]?.ToString().Trim(),
                        Season = row["Season"]?.ToString().Trim(),
                        ContractNo = row["Ref#"]?.ToString().Trim(),
                        ColorCode = row["Color"]?.ToString().Trim(),
                        ColorName = row["Fashion Color"]?.ToString().Trim(),
                        PurchaseOrderNumber = row["PO#"]?.ToString().Trim(),
                        ShipDate = DateTime.Parse(row["Ship Date"]?.ToString()),
                        LSStyle = row["LS Style"]?.ToString().Trim()
                    };

                    if (!dicItemStyles.TryGetValue(itemStyle.PurchaseOrderNumber, out ItemStyle rsItemStyle))
                    {
                        itemStyles.Add(itemStyle);
                        dicItemStyles[itemStyle.PurchaseOrderNumber] = itemStyle;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                LogHelper.Instance.Error("{@Datetime} --- Import sales order for HA error with message {@message}",
                    DateTime.Now.ToString(), ex.Message);

            }

            return itemStyles;
        }

        public static List<ItemStyle> ImportProductionItemStyle_HA(this ExcelWorksheet worksheet,
            ref Dictionary<string, List<ItemStyle>> dicAllItemStyles,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            List<ItemStyle> itemStyles = new List<ItemStyle>();
            Dictionary<string, ItemStyle> dicItemStyles = new Dictionary<string, ItemStyle>();
            //dicOneItemStyle = new Dictionary<string, ItemStyle>();
            //dicMultiItemStyle = new Dictionary<string, List<ItemStyle>>();

            var table = worksheet.ToDataTable(startHeader: 3, startColumn: 1, endColumn: 21, startRow: 3);

            try
            {
                List<ItemStyle> itemStyleGenarate = new List<ItemStyle>();


                int i = 3;

                foreach (DataRow row in table.Rows)
                {
                    i++;

                    if (string.IsNullOrEmpty(row["LS Style"]?.ToString()))
                        continue;

                    if (string.IsNullOrEmpty(row["Division"]?.ToString().Trim()))
                    {
                        break;
                    }

                    var itemStyle = new ItemStyle()
                    {
                        CustomerStyle = row["STYLE"]?.ToString(),
                        Division = row["Division"]?.ToString().Trim(),
                        Season = row["Season"]?.ToString().Trim(),
                        ContractNo = row["Ref#"]?.ToString().Trim(),
                        ColorCode = row["Color"]?.ToString().Trim(),
                        ColorName = row["Fashion Color"]?.ToString().Trim(),
                        PurchaseOrderNumber = row["PO#"]?.ToString().Trim(),
                        LSStyle = row["LS Style"]?.ToString().Replace("---", "-").Replace("--", "-").Trim(),
                        TotalQuantity = decimal.Parse(row["PO Qty"]?.ToString() == "" ? "0" : row["PO Qty"]?.ToString()),
                    };

                    if (DateTime.TryParse(row["Ship Date"]?.ToString(), out DateTime rsShipDate))
                    {
                        itemStyle.ShipDate = rsShipDate;
                    }

                    if (!dicItemStyles.ContainsKey(itemStyle.LSStyle))
                    {
                        itemStyles.Add(itemStyle);
                        dicItemStyles[itemStyle.LSStyle] = itemStyle;
                    }

                    if (dicAllItemStyles.TryGetValue(itemStyle.PurchaseOrderNumber, out List<ItemStyle> rsItemStyles))
                    {
                        rsItemStyles.Add(itemStyle);
                        dicAllItemStyles[itemStyle.PurchaseOrderNumber] = rsItemStyles;
                    }
                    else
                    {
                        List<ItemStyle> newItemStyles = new List<ItemStyle>();
                        newItemStyles.Add(itemStyle);

                        dicAllItemStyles[itemStyle.PurchaseOrderNumber] = newItemStyles;
                    }
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                LogHelper.Instance.Error("{@Datetime} --- Import sales order for HA error with message {@message}",
                    DateTime.Now.ToString(), ex.Message);

            }

            return itemStyles;
        }

        public static SalesOrder ImportSalesOrderJA(this ExcelWorksheet worksheet, string userName,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            SalesOrder salesOrder = new SalesOrder();

            var table = worksheet.ToDataTable(2);

            foreach (DataRow row in table.Rows)
            {
                if (string.IsNullOrEmpty(row["Customer Style"]?.ToString()))
                    break;

                try
                {
                    var itemStyle = new ItemStyle
                    {
                        CustomerStyle = row["Customer Style"]?.ToString(),
                        LSStyle = row["LSStyle"]?.ToString(),
                        PurchaseOrderNumber = row["PO"]?.ToString(),
                        Description = row["Description"]?.ToString(),
                        Season = row["Season"]?.ToString(),
                        Gender = row["Gender"]?.ToString(),
                        ContractDate = DateTime.Parse(row["Contract Date"]?.ToString()),
                        DeliveryDate = DateTime.Parse(row["Delivery Date"]?.ToString()),
                        DeliveryPlace = row["Delivery Place"]?.ToString(),
                        ColorCode = row["Color"]?.ToString(),
                        ColorName = row["Color Name"]?.ToString(),
                    };
                    salesOrder.ItemStyles.Add(itemStyle);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error("{@Datetime} ---Import sales order for JA error with message {@message}",
                       DateTime.Now.ToString(), ex.Message);
                }
            }

            return null;
        }

        public static SalesOrder ImportSalesOrderLTD(this ExcelWorksheet worksheet, string userName,
            IEnumerable<Size> sizes,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            SalesOrder salesOrder = new SalesOrder();

            var table = worksheet.ToDataTable(2);
            var sizeTable = worksheet.ToDataTable(startHeader: 2, startColumn: 15,
                endColumn: 50, startRow: 2);

            foreach (DataRow row in table.Rows)
            {
                if (string.IsNullOrEmpty(row["Customer Style"]?.ToString()))
                    break;

                try
                {
                    var itemStyle = new ItemStyle
                    {
                        CustomerStyle = row["Customer Style"]?.ToString(),
                        LSStyle = row["LS Style"]?.ToString(),
                        PurchaseOrderNumber = row["PO"]?.ToString(),
                        Description = row["Description"]?.ToString(),
                        Season = row["Season"]?.ToString(),
                        Gender = row["Gender"]?.ToString(),
                        ContractDate = DateTime.Parse(row["Contract Date"]?.ToString()),
                        DeliveryDate = DateTime.Parse(row["Delivery Date"]?.ToString()),
                        DeliveryPlace = row["Delivery Place"]?.ToString(),
                        ColorCode = row["Color"]?.ToString(),
                        ColorName = row["Color Name"]?.ToString(),
                    };

                    foreach (DataColumn column in sizeTable.Columns)
                    {
                        if (column.ColumnName.Contains("Column"))
                            continue;

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

                        var orderDetail = new OrderDetail()
                        {
                            Size = size.Code,
                            Quantity = int.Parse(quatityRow[sizeTable.Columns.IndexOf(column)]?
                                    .ToString().Replace(".", "").Replace(",", "")),
                            SizeSortIndex = size.SequeneceNumber,
                        };

                        itemStyle.OrderDetails.Add(orderDetail);
                    }

                    salesOrder.ItemStyles.Add(itemStyle);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error("{@Datetime} ---Import sales order for LTD error with message {@message}",
                       DateTime.Now.ToString(), ex.Message);
                }
            }

            return salesOrder;
        }

        public static SalesOrder ImportSalesOrderOS(this ExcelWorksheet worksheet, string userName,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            SalesOrder salesOrder = new SalesOrder();

            var table = worksheet.ToDataTable(2);

            foreach (DataRow row in table.Rows)
            {
                if (string.IsNullOrEmpty(row["Customer Style"]?.ToString()))
                    break;

                try
                {
                    var itemStyle = new ItemStyle
                    {
                        CustomerStyle = row["Customer Style"]?.ToString(),
                        LSStyle = row["LSStyle"]?.ToString(),
                        PurchaseOrderNumber = row["PO"]?.ToString(),
                        Description = row["Description"]?.ToString(),
                        Season = row["Season"]?.ToString(),
                        Gender = row["Gender"]?.ToString(),
                        ContractDate = DateTime.Parse(row["Contract Date"]?.ToString()),
                        DeliveryDate = DateTime.Parse(row["Delivery Date"]?.ToString()),
                        DeliveryPlace = row["Delivery Place"]?.ToString(),
                        ColorCode = row["Color"]?.ToString(),
                        ColorName = row["Color Name"]?.ToString(),
                    };
                    salesOrder.ItemStyles.Add(itemStyle);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Error("{@Datetime} ---Import sales order for LTD error with message {@message}",
                       DateTime.Now.ToString(), ex.Message);
                }
            }

            return null;
        }

        public static SalesOrder ImportSalesOrderIFG(this ExcelWorksheet worksheet, string userName,
            IEnumerable<Size> sizes, List<PurchaseOrderType> purchaseOrderTypes, List<ItemStyle> itemStyles,
            out List<PurchaseOrderType> newPurchaseOrderTypes,
            out string errorMessage, bool isUpdate = false)
        {
            errorMessage = string.Empty;
            SalesOrder salesOrder = new SalesOrder();
            newPurchaseOrderTypes = new List<PurchaseOrderType>();
            var table = worksheet.ToDataTable(2);
            var sizeTable = worksheet.ToDataTable(startHeader: 2, startColumn: 19,
                endColumn: 50, startRow: 2);
            try
            {
                var dicLsstyle = itemStyles.ToDictionary(x => x.LSStyle);

                foreach (DataRow row in table.Rows)
                {
                    if (string.IsNullOrEmpty(row["STYLE#"]?.ToString()))
                        break;

                    var itemStyle = new ItemStyle()
                    {
                        CustomerStyle = row["STYLE#"]?.ToString(),
                        ContractNo = row["GROUP STYLE"]?.ToString().Trim(),
                        LSStyle = row["LS code"]?.ToString(),
                        ColorCode = row["Color Code"]?.ToString().Trim(),
                        ColorName = row["Color name"]?.ToString().Trim(),
                        Description = row["Description"]?.ToString(),
                        PurchaseOrderNumber = row["PO#"]?.ToString(),
                        ShipDate = DateTime.Parse(row["SHIP DATE"]?.ToString()),
                        Season = row["SEASON"]?.ToString(),
                        Gender = row["GENDER"]?.ToString(),
                        MSRP = decimal.Parse(row["MSRP"]?.ToString().Replace("$", "") == "" ? "0" : row["MSRP"]?.ToString().Replace("$", "")),
                        ShipMode = row["SHIP MODE"]?.ToString(),
                        ShipTo = row["SHIP TO WH/DESTINATION "]?.ToString(),
                        Packing = row["PACKING WAY "]?.ToString(),
                        Brand = row["BRAND"]?.ToString()
                    };

                    if (!isUpdate && dicLsstyle.ContainsKey(itemStyle.LSStyle))
                    {
                        errorMessage = itemStyle.LSStyle + " is duplicate, please re-check";
                        return null;
                    }
                    else
                    {
                        dicLsstyle[itemStyle.LSStyle] = itemStyle;
                    }

                    var purchaseOrderType = purchaseOrderTypes
                                .FirstOrDefault(x => x.Name == row["DISTRIBUTION"]?.ToString().Trim());

                    if (purchaseOrderType != null)
                    {
                        itemStyle.PurchaseOrderTypeCode = purchaseOrderType.Code;
                    }
                    else
                    {
                        purchaseOrderType = new PurchaseOrderType()
                        {
                            Code = Nanoid.Nanoid.Generate("12345467890", 3),
                            Name = row["DISTRIBUTION"]?.ToString().Trim()
                        };
                        purchaseOrderType.SetCreateAudit(userName);

                        newPurchaseOrderTypes.Add(purchaseOrderType);
                        purchaseOrderTypes.Add(purchaseOrderType);

                        itemStyle.PurchaseOrderTypeCode = purchaseOrderType.Code;
                    }


                    decimal? price = null;
                    if (!string.IsNullOrEmpty(row["Same Price"]?.ToString().Trim()))
                        price = decimal.Parse(row["Same Price"]?.ToString().Trim());

                    foreach (DataColumn column in sizeTable.Columns)
                    {
                        if (column.ColumnName.Contains("Column"))
                            continue;
                        if (column.ColumnName.Contains("Same Price"))
                            continue;

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

                        //var orderDetail = new OrderDetail()
                        //{
                        //    Size = size.Code,
                        //    Quantity = int.Parse(quatityRow[sizeTable.Columns.IndexOf(column)]?
                        //            .ToString().Replace(".", "").Replace(",", "")),
                        //    SizeSortIndex = size.SequeneceNumber,
                        //    Price = price
                        //};

                        //// PRICE OVER SIZE - UPDATE 2022-12-15
                        var orderDetail = new OrderDetail();
                        orderDetail.Size = size.Code;
                        orderDetail.Quantity = int.Parse(quatityRow[sizeTable.Columns.IndexOf(column)]?
                                .ToString().Replace(".", "").Replace(",", ""));
                        orderDetail.SizeSortIndex = size.SequeneceNumber;
                        if (price == null &&
                            decimal.TryParse(quatityRow[sizeTable.Columns.IndexOf(column) + 1]?
                            .ToString().Replace(",", ""), out decimal sizePrice))
                        {
                            orderDetail.Price = sizePrice;
                        }
                        else
                            orderDetail.Price = price;
                        //// END UPDATE

                        itemStyle.SetCreateAudit(userName);
                        itemStyle.OrderDetails.Add(orderDetail);
                    }

                    salesOrder.ItemStyles.Add(itemStyle);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }


            return salesOrder;
        }

        public static SalesOrder ImportSalesOrderVG(this ExcelWorksheet worksheet, string userName,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            return null;
        }

        /// <summary>
        /// get list purchase order group line, get sheet tracking
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="userName"></param>
        /// <param name="errorMessage"></param>
        /// <returns> dictionary <PONum, List<POLine>> </returns>
        public static Dictionary<string, List<PurchaseOrderLine>> ImportPurchaseOrderGroupLine_PU(this ExcelWorksheet worksheet, string userName,
            Dictionary<string, Vendor> dicOldVendors,
            out List<string> purchaseOrderNumbers,
            out List<Vendor> newVendors,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            var table = worksheet.ToDataTable(startHeader: 3, startColumn: 2, endColumn: 42, startRow: 3);
            newVendors = new List<Vendor>();
            purchaseOrderNumbers = new List<string>();

            Dictionary<string, List<PurchaseOrderLine>> dicPurchaseOrderLines = new Dictionary<string, List<PurchaseOrderLine>>();
            Dictionary<string, PurchaseOrderLine> dicCheckDuplicatePurchaseOrderLines = new Dictionary<string, PurchaseOrderLine>();
            Dictionary<string, string> dicPurchaseOrderNumbers = new Dictionary<string, string>();
            List<PurchaseOrderLine> purchaseOrderLines = new List<PurchaseOrderLine>();
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var statusPO = row["Fabric Status"].ToString();
                    if (!string.IsNullOrEmpty(statusPO) && !statusPO.ToUpper().Trim().Contains("CANCELLED"))
                    {
                        var PONumber = row["PO No"].ToString().Trim();
                        var mtrCode = row["MatrCode"].ToString();
                        var color = row["Color"].ToString();
                        var size = row["Size"].ToString();
                        var key = PONumber + mtrCode + color + size;
                        var unit = row["Unit"].ToString();

                        if (!string.IsNullOrEmpty(unit) && unit.ToUpper().Trim().Equals("BAG"))
                        {
                            unit = "";
                        }

                        if (!dicPurchaseOrderNumbers.ContainsKey(PONumber))
                        {
                            dicPurchaseOrderNumbers[PONumber] = PONumber;
                        }

                        if (dicCheckDuplicatePurchaseOrderLines.TryGetValue(key, out PurchaseOrderLine rsPurchaseOrderLine))
                        {
                            rsPurchaseOrderLine.Quantity += decimal.Parse(row["Order Qty"]?.ToString());
                            dicCheckDuplicatePurchaseOrderLines[key] = rsPurchaseOrderLine;
                        }
                        else
                        {
                            PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();

                            purchaseOrderLine.CustomerPurchaseOrderNumber = PONumber;
                            purchaseOrderLine.ItemID = mtrCode;
                            purchaseOrderLine.Quantity = decimal.Parse(row["Order Qty"]?.ToString());
                            purchaseOrderLine.Specify = size;
                            purchaseOrderLine.Remarks = statusPO + " + " + row["Buyer Ref."].ToString();
                            purchaseOrderLine.InvoiceNo = row["Invoice No"].ToString();
                            purchaseOrderLine.ItemColorName = color;
                            purchaseOrderLine.CustomerSupplier = row["T2 Name"].ToString();
                            purchaseOrderLine.UnitID = unit;
                            purchaseOrderLine.SecondUnitID = unit;
                            purchaseOrderLine.MaterialTypeClass = row["MatrClass"].ToString();

                            if (!dicOldVendors.ContainsKey(purchaseOrderLine.CustomerSupplier))
                            {
                                Vendor newVendor = new Vendor();
                                newVendor.ID = purchaseOrderLine.CustomerSupplier;
                                newVendor.Name = purchaseOrderLine.CustomerSupplier;
                                newVendor.SetCreateAudit(userName);

                                newVendors.Add(newVendor);

                                dicOldVendors[purchaseOrderLine.CustomerSupplier] = newVendor;
                            }

                            if (DateTime.TryParse(row["PI Delivery"].ToString(), out DateTime vendorConfirmDate))
                            {
                                purchaseOrderLine.VendorConfirmDate = vendorConfirmDate;
                            }

                            if (DateTime.TryParse(row["Ship Date"].ToString(), out DateTime shipDate))
                            {
                                purchaseOrderLine.ShipDate = shipDate;
                            }

                            if (DateTime.TryParse(row["in-fty Date"].ToString(), out DateTime estimateDate))
                            {
                                purchaseOrderLine.EstimateShipDate = estimateDate;
                            }

                            if (dicPurchaseOrderLines.TryGetValue(purchaseOrderLine.CustomerPurchaseOrderNumber,
                                out List<PurchaseOrderLine> rsListPO))
                            {
                                purchaseOrderLines = rsListPO;
                            }
                            dicCheckDuplicatePurchaseOrderLines[key] = purchaseOrderLine;
                        }

                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.InnerException?.Message;
                    LogHelper.Instance.Error("{@Datetime} ---Import purchase order for PU error with message {@message}",
                        DateTime.Now.ToString(), ex.Message);
                    return null;
                }
            }

            foreach (var itemLine in dicCheckDuplicatePurchaseOrderLines)
            {
                List<PurchaseOrderLine> lstLine = new List<PurchaseOrderLine>();

                if (dicPurchaseOrderLines.TryGetValue(itemLine.Value.CustomerPurchaseOrderNumber, out List<PurchaseOrderLine> rsLines))
                {
                    lstLine = rsLines;
                }

                lstLine.Add(itemLine.Value);
                dicPurchaseOrderLines[itemLine.Value.CustomerPurchaseOrderNumber] = lstLine;
            }

            if (dicPurchaseOrderNumbers.Any())
            {
                foreach (var item in dicPurchaseOrderNumbers)
                {
                    purchaseOrderNumbers.Add(item.Key);
                }
            }

            return dicPurchaseOrderLines;
        }


        public static Dictionary<string, List<PurchaseOrderLine>> ImportFB_PurchaseOrderGroupLine_PU(this ExcelWorksheet worksheet, string userName,
            Dictionary<string, Vendor> dicOldVendors,
            out List<string> purchaseOrderNumbers,
            out List<Vendor> newVendors,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            var table = worksheet.ToDataTable(startHeader: 2, startColumn: 2, endColumn: 42, startRow: 3);

            purchaseOrderNumbers = new List<string>();
            newVendors = new List<Vendor>();
            Dictionary<string, List<PurchaseOrderLine>> dicPurchaseOrderLines = new Dictionary<string, List<PurchaseOrderLine>>();
            Dictionary<string, PurchaseOrderLine> dicCheckDuplicatePurchaseOrderLines = new Dictionary<string, PurchaseOrderLine>();
            Dictionary<string, string> dicPurchaseOrderNumbers = new Dictionary<string, string>();
            List<PurchaseOrderLine> purchaseOrderLines = new List<PurchaseOrderLine>();
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var statusPO = row["Fabric Status"].ToString();
                    if (!string.IsNullOrEmpty(statusPO) && !statusPO.ToUpper().Trim().Contains("CANCELLED"))
                    {
                        var PONumber = row["PO No"].ToString().Trim();
                        var mtrCode = row["MatrCode"].ToString();
                        var color = row["Color"].ToString();
                        var size = row["Size"].ToString();
                        var key = PONumber + mtrCode + color + size;

                        if (!dicPurchaseOrderNumbers.ContainsKey(PONumber))
                        {
                            dicPurchaseOrderNumbers[PONumber] = PONumber;
                        }

                        if (dicCheckDuplicatePurchaseOrderLines.TryGetValue(key, out PurchaseOrderLine rsPurchaseOrderLine))
                        {
                            rsPurchaseOrderLine.Quantity += decimal.Parse(row["Order Qty"]?.ToString());
                            dicCheckDuplicatePurchaseOrderLines[key] = rsPurchaseOrderLine;
                        }
                        else
                        {
                            PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();

                            purchaseOrderLine.CustomerPurchaseOrderNumber = PONumber;
                            purchaseOrderLine.ItemID = mtrCode;
                            purchaseOrderLine.Quantity = decimal.Parse(row["Order Qty"]?.ToString());
                            purchaseOrderLine.Specify = size;
                            purchaseOrderLine.Remarks = statusPO + " + " + row["Buyer Ref."].ToString();
                            purchaseOrderLine.InvoiceNo = row["Invoice No"].ToString();
                            purchaseOrderLine.ItemColorName = color;
                            purchaseOrderLine.CustomerSupplier = row["T2 Name"].ToString();
                            purchaseOrderLine.UnitID = row["Unit"].ToString();
                            purchaseOrderLine.SecondUnitID = row["Unit"].ToString();
                            purchaseOrderLine.MaterialTypeClass = row["MatrClass"].ToString();

                            if (!dicOldVendors.ContainsKey(purchaseOrderLine.CustomerSupplier))
                            {
                                Vendor newVendor = new Vendor();
                                newVendor.ID = purchaseOrderLine.CustomerSupplier;
                                newVendor.Name = purchaseOrderLine.CustomerSupplier;
                                newVendor.SetCreateAudit(userName);

                                newVendors.Add(newVendor);

                                dicOldVendors[purchaseOrderLine.CustomerSupplier] = newVendor;
                            }

                            if (DateTime.TryParse(row["PI Delivery"].ToString(), out DateTime vendorConfirmDate))
                            {
                                purchaseOrderLine.VendorConfirmDate = vendorConfirmDate;
                            }

                            if (DateTime.TryParse(row["Ship Date"].ToString(), out DateTime shipDate))
                            {
                                purchaseOrderLine.ShipDate = shipDate;
                            }

                            if (DateTime.TryParse(row["in-fty Date"].ToString(), out DateTime estimateDate))
                            {
                                purchaseOrderLine.EstimateShipDate = estimateDate;
                            }

                            if (dicPurchaseOrderLines.TryGetValue(purchaseOrderLine.CustomerPurchaseOrderNumber,
                                out List<PurchaseOrderLine> rsListPO))
                            {
                                purchaseOrderLines = rsListPO;
                            }
                            dicCheckDuplicatePurchaseOrderLines[key] = purchaseOrderLine;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.InnerException?.Message;
                    LogHelper.Instance.Error("{@Datetime} ---Import purchase order for PU error with message {@message}",
                        DateTime.Now.ToString(), ex.Message);
                    return null;
                }
            }

            foreach (var itemLine in dicCheckDuplicatePurchaseOrderLines)
            {
                List<PurchaseOrderLine> lstLine = new List<PurchaseOrderLine>();

                if (dicPurchaseOrderLines.TryGetValue(itemLine.Value.CustomerPurchaseOrderNumber, out List<PurchaseOrderLine> rsLines))
                {
                    lstLine = rsLines;
                }

                lstLine.Add(itemLine.Value);
                dicPurchaseOrderLines[itemLine.Value.CustomerPurchaseOrderNumber] = lstLine;
            }

            if (dicPurchaseOrderNumbers.Any())
            {
                foreach (var item in dicPurchaseOrderNumbers)
                {
                    purchaseOrderNumbers.Add(item.Key);
                }
            }

            return dicPurchaseOrderLines;
        }


        /// <summary>
        /// get list purchase order group line, get sheet allocation detail chart
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="userName"></param>
        /// <param name="errorMessage"></param>
        /// <returns> dictionary <PONum, List<POLine>> </returns>
        public static Dictionary<string, List<PurchaseOrderLine>> ImportPurchaseOrderLine_PU(this ExcelWorksheet worksheet, string userName,
            Dictionary<string, Vendor> dicOldVendors,
            ref List<string> purchaseOrderNumbers,
            out List<Vendor> newVendors,
            out List<string> contractNos,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            var table = worksheet.ToDataTable(startHeader: 1, startColumn: 1, endColumn: 26, startRow: 1);

            contractNos = new List<string>();
            newVendors = new List<Vendor>();
            Dictionary<string, List<PurchaseOrderLine>> dicPurchaseOrderLines = new Dictionary<string, List<PurchaseOrderLine>>();
            List<PurchaseOrderLine> purchaseOrderLines = new List<PurchaseOrderLine>();

            //int rowNum = 1;

            foreach (DataRow row in table.Rows)
            {
                //Console.WriteLine("ROW =>>>>>> " + rowNum);
                try
                {
                    //if (rowNum == 47062)
                    //{
                    //    string str = "";
                    //}
                    //rowNum++;

                    var PONumber = row["PurchaseNo"].ToString();
                    var estimate = row["Estimate in VN fty."].ToString();
                    var remark = row["Remark"].ToString();
                    var check = row["Column1"].ToString();
                    bool read = true;

                    if ((!string.IsNullOrEmpty(remark) && remark.ToUpper().Contains("CANCELLED")) ||
                        (!string.IsNullOrEmpty(estimate) && estimate.ToUpper().Contains("CANCELLED")) ||
                         (!string.IsNullOrEmpty(check) && check.ToUpper().Contains("CANCELLED")))
                    {
                        read = false;
                    }

                    if (!string.IsNullOrEmpty(PONumber) && read)
                    {
                        PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();

                        var position = PONumber.IndexOf(":");
                        if (position > 0)
                        {
                            PONumber = PONumber.Substring(0, position);
                        }

                        var unit = row["Unit"]?.ToString().Trim();

                        if (!string.IsNullOrEmpty(unit) && unit.ToUpper().Trim().Equals("BAG"))
                        {
                            unit = "";
                        }

                        var qty = row["Allocation Qty"]?.ToString();

                        purchaseOrderLine.CustomerStyle = row["Style no."]?.ToString().Trim();
                        purchaseOrderLine.ContractNo = row["Contract No."]?.ToString().Trim();
                        purchaseOrderLine.CustomerPurchaseOrderNumber = PONumber.Trim();
                        purchaseOrderLine.ItemID = row["Material Code"]?.ToString().Trim();
                        purchaseOrderLine.ItemName = row["Material Name"]?.ToString().Trim();
                        purchaseOrderLine.ItemColorName = row["Color"]?.ToString().Trim();
                        purchaseOrderLine.CustomerSupplier = row["Supplier"]?.ToString().Trim();
                        purchaseOrderLine.UnitID = unit;
                        purchaseOrderLine.SecondUnitID = purchaseOrderLine.UnitID;
                        purchaseOrderLine.Specify = row["Material Specification"]?.ToString().Trim();
                        purchaseOrderLine.Quantity = decimal.Parse(row["Allocation Qty"]?.ToString());
                        purchaseOrderLine.Remarks = remark;
                        purchaseOrderLine.MaterialTypeClass = row["Matrial Class"]?.ToString().Trim();

                        var mtrType = row["Type"]?.ToString();
                        if (!string.IsNullOrEmpty(mtrType) && mtrType.ToUpper().Equals("FABRICS"))
                        {
                            purchaseOrderLine.MaterialType = "FB";
                        }
                        else
                        {
                            purchaseOrderLine.MaterialType = "AC";
                        }

                        if (dicPurchaseOrderLines.TryGetValue(purchaseOrderLine.CustomerPurchaseOrderNumber,
                            out List<PurchaseOrderLine> rsListPO))
                        {
                            purchaseOrderLines = rsListPO;
                        }

                        if (!dicOldVendors.ContainsKey(purchaseOrderLine.CustomerSupplier))
                        {
                            Vendor newVendor = new Vendor();
                            newVendor.ID = purchaseOrderLine.CustomerSupplier;
                            newVendor.Name = purchaseOrderLine.CustomerSupplier;
                            newVendor.SetCreateAudit(userName);

                            newVendors.Add(newVendor);

                            dicOldVendors[purchaseOrderLine.CustomerSupplier] = newVendor;
                        }

                        purchaseOrderLines.Add(purchaseOrderLine);
                        contractNos.Add(purchaseOrderLine.ContractNo);
                        dicPurchaseOrderLines[purchaseOrderLine.CustomerPurchaseOrderNumber] = purchaseOrderLines;
                        purchaseOrderLines = new List<PurchaseOrderLine>();


                    }

                }
                catch (Exception ex)
                {
                    errorMessage = ex.InnerException?.Message;
                    LogHelper.Instance.Error("{@Datetime} ---Import purchase order for PU error with message {@message}",
                        DateTime.Now.ToString(), ex.Message);
                    return null;
                }
            }

            if (dicPurchaseOrderLines.Any())
            {
                foreach (var item in dicPurchaseOrderLines)
                {
                    purchaseOrderNumbers.Add(item.Key);
                }
            }

            return dicPurchaseOrderLines;
        }

        /// <summary>
        /// get list purchase order line, get sheet International OCL
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="userName"></param>
        /// <param name="errorMessage"></param>
        /// <returns> dictionary <PONum, List<POLine>> </returns>
        public static Dictionary<string, List<PurchaseOrderLine>> ImportPurchaseOrderLineInternational_PU(this ExcelWorksheet worksheet,
            string userName,
            ref List<string> purchaseOrderNumbers,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            var table = worksheet.ToDataTable(startHeader: 1, startColumn: 1, endColumn: 26, startRow: 1);

            Dictionary<string, List<PurchaseOrderLine>> dicPurchaseOrderLines = new Dictionary<string, List<PurchaseOrderLine>>();

            //var firstRow = table.Rows[0]["International OCL"].GetType();

            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var PONumber = row["Purcahse order number"].ToString();
                    string itemCode = worksheet.Cells[1, row.Table.Columns["International OCL"].Ordinal + 2].GetValue<string>().Trim();

                    if (!string.IsNullOrEmpty(PONumber))
                    {
                        //var qty = row.Table.Columns["International OCL"].ToString().Trim();
                        if (decimal.TryParse(row["International OCL"].ToString().Trim(), out decimal rs))
                        {
                            if (rs > 0)
                            {
                                PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
                                List<PurchaseOrderLine> purchaseOrderLines = new List<PurchaseOrderLine>();

                                purchaseOrderLine.Quantity = rs;
                                purchaseOrderLine.CustomerStyle = row["STYNUM"]?.ToString().Trim();
                                purchaseOrderLine.ContractNo = row["TSG contract number"]?.ToString().Trim();
                                purchaseOrderLine.CustomerPurchaseOrderNumber = PONumber.Trim();
                                purchaseOrderLine.ItemID = "PUMA BARCODE STICKER-2"; //International OCL
                                purchaseOrderLine.ItemName = "PUMA BARCODE STICKER-2";//International OCL
                                purchaseOrderLine.Specify = row["SIZECODE"]?.ToString().Trim();
                                purchaseOrderLine.UnitID = row["Unit"]?.ToString().Trim().ToUpper();
                                purchaseOrderLine.SecondUnitID = row["Unit"]?.ToString().Trim().ToUpper();
                                purchaseOrderLine.MaterialTypeClass = "SK";
                                purchaseOrderLine.MaterialType = "AC";

                                if (dicPurchaseOrderLines.TryGetValue(purchaseOrderLine.CustomerPurchaseOrderNumber,
                                out List<PurchaseOrderLine> rsListPO))
                                {
                                    purchaseOrderLines = rsListPO;
                                }
                                purchaseOrderLines.Add(purchaseOrderLine);

                                dicPurchaseOrderLines[purchaseOrderLine.CustomerPurchaseOrderNumber] = purchaseOrderLines;
                            }
                        }

                        if (decimal.TryParse(row[row.Table.Columns["International OCL"].Ordinal + 1].ToString().Trim(), out decimal rsQty))
                        {
                            if (rsQty > 0)
                            {
                                List<PurchaseOrderLine> purchaseOrderLines = new List<PurchaseOrderLine>();
                                PurchaseOrderLine purchaseOrderLine1 = new PurchaseOrderLine();

                                purchaseOrderLine1.CustomerStyle = row["STYNUM"]?.ToString().Trim();
                                purchaseOrderLine1.ContractNo = row["TSG contract number"]?.ToString().Trim();
                                purchaseOrderLine1.CustomerPurchaseOrderNumber = PONumber.Trim();
                                purchaseOrderLine1.ItemID = "PUMA BARCODE STICKER-6";//itemCode.Trim();
                                purchaseOrderLine1.ItemName = "PUMA BARCODE STICKER-6"; //itemCode.Trim();
                                purchaseOrderLine1.Specify = row["SIZECODE"]?.ToString().Trim();
                                purchaseOrderLine1.Quantity = rsQty;
                                purchaseOrderLine1.UnitID = row["Unit"]?.ToString().Trim().ToUpper();
                                purchaseOrderLine1.SecondUnitID = row["Unit"]?.ToString().Trim().ToUpper();
                                purchaseOrderLine1.MaterialTypeClass = "SK";
                                purchaseOrderLine1.MaterialType = "AC";

                                if (dicPurchaseOrderLines.TryGetValue(purchaseOrderLine1.CustomerPurchaseOrderNumber,
                                out List<PurchaseOrderLine> rsListPO))
                                {
                                    purchaseOrderLines = rsListPO;
                                }

                                purchaseOrderLines.Add(purchaseOrderLine1);

                                dicPurchaseOrderLines[purchaseOrderLine1.CustomerPurchaseOrderNumber] = purchaseOrderLines;
                            }

                        }
                        //purchaseOrderLines = new List<PurchaseOrderLine>();
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.InnerException?.Message;
                    LogHelper.Instance.Error("{@Datetime} ---Import purchase order for PU error with message {@message}",
                        DateTime.Now.ToString(), ex.Message);
                    return null;
                }
            }

            if (dicPurchaseOrderLines.Any())
            {
                foreach (var item in dicPurchaseOrderLines)
                {
                    purchaseOrderNumbers.Add(item.Key);
                }
            }

            return dicPurchaseOrderLines;
        }

        /// <summary>
        /// delivery note of contract
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="userName"></param>
        /// <param name="errorMessage"></param>
        /// <returns> dictionary <contractNo, POLine> </returns>
        public static Dictionary<string, List<PurchaseOrderLine>> ImportPurchaseOrderCareLabel_PU(this ExcelWorksheet worksheet, string userName,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            var table = worksheet.ToDataTable(startHeader: 2, startColumn: 1, endColumn: 48, startRow: 2);
            var merge = worksheet.MergedCells;

            Dictionary<string, List<PurchaseOrderLine>> dicPurchaseOrderLines = new Dictionary<string, List<PurchaseOrderLine>>();
            Dictionary<string, PurchaseOrderLine> dicCheckDuplicatePurchaseOrderLines = new Dictionary<string, PurchaseOrderLine>();

            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var PONumber = row["PO No."].ToString().Trim();
                    var contractNo = row["Contract No."].ToString().Trim();
                    var season = row["Season"].ToString();
                    var unitID = row["unit"].ToString().Trim();
                    string materialType = "AC";

                    if (!string.IsNullOrEmpty(PONumber))
                    {
                        List<PurchaseOrderLine> purchaseOrderLines = new List<PurchaseOrderLine>();

                        if (dicPurchaseOrderLines.TryGetValue(PONumber, out List<PurchaseOrderLine> rsPOLines))
                        {
                            purchaseOrderLines = rsPOLines;
                        }

                        var deliveryNoteLabel = row["Delivery Note"].ToString();
                        var orderNoLabel = row["Order No"].ToString().Trim();
                        int rowPos = row.Table.Rows.IndexOf(row) + 2;
                        int colPos = row.Table.Columns["Delivery Note"].Ordinal + 1;
                        var checkStrike = worksheet.Cells[rowPos, colPos].Style.Font.Strike;

                        if (!checkStrike && !string.IsNullOrEmpty(orderNoLabel) && !orderNoLabel.ToLower().Trim().Equals("cancelled") &&
                            !orderNoLabel.ToLower().Replace(" ", "").Trim().Equals("noneed"))
                        {
                            PurchaseOrderLine purchaseOrderLineLabel = new PurchaseOrderLine();

                            var range = merge[0];
                            using (var rangeCell = worksheet.Cells[range])
                            {
                                purchaseOrderLineLabel.ItemID = rangeCell.Text;
                                purchaseOrderLineLabel.ItemName = "PUMA CARE LABEL";
                            }

                            purchaseOrderLineLabel.ContractNo = contractNo;
                            purchaseOrderLineLabel.OrderNo = orderNoLabel;
                            purchaseOrderLineLabel.DeliveryNote = deliveryNoteLabel;
                            purchaseOrderLineLabel.Season = season;
                            purchaseOrderLineLabel.CustomerPurchaseOrderNumber = PONumber;
                            purchaseOrderLineLabel.MaterialTypeClass = "CL";
                            purchaseOrderLineLabel.MaterialType = materialType;

                            if (!string.IsNullOrEmpty(unitID))
                            {
                                purchaseOrderLineLabel.UnitID = unitID;
                                purchaseOrderLineLabel.SecondUnitID = unitID;
                            }

                            if (DateTime.TryParse(row["Order Date"].ToString(), out DateTime rsOrderDate))
                            {
                                purchaseOrderLineLabel.OrderDate = rsOrderDate;
                            }

                            if (decimal.TryParse(row["Order Qty"].ToString(), out decimal qty))
                            {
                                purchaseOrderLineLabel.Quantity = qty;
                                purchaseOrderLineLabel.WareHouseQuantity = qty;
                            }

                            purchaseOrderLines.Add(purchaseOrderLineLabel);
                        }

                        if (decimal.TryParse(row["Order_Qty"].ToString(), out decimal qtySticker))
                        {
                            var deliveryNoteSticker = row["Delivery_Note"].ToString();
                            var orderNoSticker = row["Order_No"].ToString().Trim();
                            colPos = row.Table.Columns["Delivery_Note"].Ordinal + 1;

                            var checkStrikeSticker = worksheet.Cells[rowPos, colPos].Style.Font.Strike;

                            if (!checkStrikeSticker && !string.IsNullOrEmpty(orderNoSticker) && !orderNoSticker.ToLower().Trim().Equals("cancelled") &&
                                !orderNoSticker.ToLower().Replace(" ", "").Trim().Equals("noneed"))
                            {
                                PurchaseOrderLine purchaseOrderLineSticker = new PurchaseOrderLine();

                                var rangeSticker = merge[1];
                                using (var rangeCellSticker = worksheet.Cells[rangeSticker])
                                {
                                    purchaseOrderLineSticker.ItemID = rangeCellSticker.Text;
                                    purchaseOrderLineSticker.ItemName = "PUMA CARE LABEL";
                                }

                                purchaseOrderLineSticker.ContractNo = contractNo;
                                purchaseOrderLineSticker.OrderNo = orderNoSticker;
                                purchaseOrderLineSticker.DeliveryNote = deliveryNoteSticker;
                                purchaseOrderLineSticker.Season = season;
                                purchaseOrderLineSticker.CustomerPurchaseOrderNumber = PONumber;
                                purchaseOrderLineSticker.MaterialTypeClass = "CL";
                                purchaseOrderLineSticker.MaterialType = materialType;

                                if (!string.IsNullOrEmpty(unitID))
                                {
                                    purchaseOrderLineSticker.UnitID = unitID;
                                    purchaseOrderLineSticker.SecondUnitID = unitID;
                                }

                                if (DateTime.TryParse(row["Order_Date"].ToString(), out DateTime rsOrder_Date))
                                {
                                    purchaseOrderLineSticker.OrderDate = rsOrder_Date;
                                }

                                purchaseOrderLineSticker.Quantity = qtySticker;
                                purchaseOrderLineSticker.WareHouseQuantity = qtySticker;

                                purchaseOrderLines.Add(purchaseOrderLineSticker);
                            }
                        }

                        if (purchaseOrderLines.Count > 0)
                        {
                            dicPurchaseOrderLines[PONumber] = purchaseOrderLines;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.InnerException?.Message;
                    LogHelper.Instance.Error("{@Datetime} ---Import website purchase order for PU error with message {@message}",
                        DateTime.Now.ToString(), ex.Message);
                    return null;
                }
            }

            return dicPurchaseOrderLines;
        }
        #region support function
        /// <summary>
        /// Generate LSStyle for contract detail for customer PU
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="salesContractsDetails"></param>
        /// <param name="CustomerID"></param>
        /// <param name="newParts"></param>
        public static void GenerateLSStyle(List<Part> parts, List<SalesContractDetail> salesContractsDetails,
            string CustomerID, out List<Part> newParts)
        {
            newParts = new List<Part>();

            var currentStyleNumber = string.Empty;
            var currentGarmentColorCode = string.Empty;
            var currentSeason = string.Empty;
            Part currentPart = null;

            foreach (var salesContractDetail in salesContractsDetails)
            {
                var sortedParts = parts?.OrderByDescending(o => o.Number)
                                           .ThenByDescending(o => o.Character.Length)
                                           .ThenByDescending(o => o.Character)
                                           .ThenByDescending(o => o.LastSequenceNumber).ToList();

                if (string.IsNullOrEmpty(salesContractDetail.LSStyle))
                {
                    currentStyleNumber = salesContractDetail.CustomerStyle;
                    currentGarmentColorCode = salesContractDetail.GarmentColorCode;
                    currentSeason = salesContractDetail.Season;

                    var existPart = sortedParts?.FirstOrDefault(x => x.Number == currentStyleNumber &&
                                             x.GarmentColorCode.ToUpper() == currentGarmentColorCode.ToUpper() &&
                                             x.Season == currentSeason);
                    if (existPart != null)
                    {
                        currentPart = existPart;
                    }
                    else
                    {
                        var diffentSeasonPart = parts?.OrderByDescending(o => o.Number)
                                           .ThenByDescending(o => o.Season)
                                           .ThenByDescending(o => o.Character.Length)
                                           .ThenByDescending(o => o.Character).ToList()
                               .FirstOrDefault(x => x.Number == currentStyleNumber &&
                                                    x.GarmentColorCode.ToUpper() == currentGarmentColorCode.ToUpper());

                        if (diffentSeasonPart != null)
                        {
                            var newSeasonPart = new Part()
                            {
                                ID = string.Empty,
                                Number = diffentSeasonPart.Number,
                                GarmentColorCode = diffentSeasonPart.GarmentColorCode,
                                GarmentColorName = diffentSeasonPart.GarmentColorName,
                                Season = currentSeason,
                                CustomerID = diffentSeasonPart.CustomerID,
                                Character = diffentSeasonPart.Character,
                                LastSequenceNumber = 1,
                            };

                            currentPart = newSeasonPart;
                            parts.Add(newSeasonPart);
                        }
                        else
                        {
                            var diffentGarmentColorPart = sortedParts?
                                    .FirstOrDefault(x => x.Number == currentStyleNumber);

                            if (diffentGarmentColorPart != null)
                            {
                                var newGarmentColorPart = new Part()
                                {
                                    ID = string.Empty,
                                    Number = currentStyleNumber,
                                    GarmentColorCode = salesContractDetail.GarmentColorCode,
                                    GarmentColorName = salesContractDetail.GarmentColorName,
                                    Season = currentSeason,
                                    CustomerID = diffentGarmentColorPart.CustomerID,
                                    Character = CharacterHelpers
                                                .GetNextCharacter(diffentGarmentColorPart.Character.ToUpper()),
                                    LastSequenceNumber = 1,
                                };

                                currentPart = newGarmentColorPart;
                                parts.Add(newGarmentColorPart);
                            }
                            else
                            {
                                var newPart = new Part()
                                {
                                    ID = string.Empty,
                                    Number = salesContractDetail.CustomerStyle,
                                    GarmentColorCode = salesContractDetail.GarmentColorCode,
                                    GarmentColorName = salesContractDetail.GarmentColorName,
                                    CustomerID = CustomerID,
                                    Season = currentSeason,
                                    Character = "A",
                                    LastSequenceNumber = 1,
                                };

                                currentPart = newPart;
                                parts.Add(newPart);
                            }
                        }
                    }

                    salesContractDetail.LSStyle = salesContractDetail.CustomerStyle + "-"
                                                        + currentSeason + "-"
                                                        + currentPart.Character + currentPart.LastSequenceNumber;
                    currentPart.LastSequenceNumber += 1;
                }
            }

            newParts = parts.Where(x => x.ID == string.Empty).ToList();
            parts = parts.Where(x => x.ID != string.Empty).ToList();

            newParts.ForEach(x =>
            {
                x.ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12);
            });
        }

        /// <summary>
        /// Generate LSStyle for item style
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="itemStyles"></param>
        /// <param name="CustomerID"></param>
        /// <param name="newParts"></param>
        public static void GenerateLSStyle(List<Part> parts, List<ItemStyle> itemStyles,
            string CustomerID, out List<Part> newParts)
        {
            newParts = new List<Part>();

            parts = parts?.OrderByDescending(o => o.Number)
                .ThenByDescending(o => o.Character.Length)
                .ThenByDescending(o => o.Character)
                .ThenByDescending(o => o.LastSequenceNumber).ToList();

            var currentStyleNumber = string.Empty;
            var currentGarmentColorCode = string.Empty;
            var currentSeason = string.Empty;
            Part currentPart = null;

            foreach (var itemStyle in itemStyles)
            {
                var sortedParts = parts?.OrderByDescending(o => o.Number)
                                           .ThenByDescending(o => o.Season)
                                           .ThenByDescending(o => o.Character.Length)
                                           .ThenByDescending(o => o.Character)
                                           .ToList();

                if (string.IsNullOrEmpty(itemStyle.LSStyle))
                {
                    currentStyleNumber = itemStyle.CustomerStyle;
                    currentGarmentColorCode = itemStyle.ColorCode;
                    currentSeason = itemStyle.Season;

                    var existPart = sortedParts?.FirstOrDefault(x => x.Number == currentStyleNumber &&
                                             x.GarmentColorCode == currentGarmentColorCode &&
                                             x.Season == currentSeason);

                    if (existPart != null)
                    {
                        currentPart = existPart;
                    }
                    else
                    {
                        var diffentSeasonPart = sortedParts?
                               .FirstOrDefault(x => x.Number == currentStyleNumber &&
                                                    x.GarmentColorCode == currentGarmentColorCode);

                        if (diffentSeasonPart != null)
                        {
                            var newSeasonPart = new Part()
                            {
                                ID = string.Empty,
                                Number = diffentSeasonPart.Number,
                                GarmentColorCode = diffentSeasonPart.GarmentColorCode,
                                GarmentColorName = diffentSeasonPart.GarmentColorName,
                                Season = currentSeason,
                                CustomerID = diffentSeasonPart.CustomerID,
                                Character = diffentSeasonPart.Character,
                                LastSequenceNumber = 1,
                            };

                            currentPart = newSeasonPart;
                            parts.Add(newSeasonPart);
                        }
                        else
                        {
                            var diffentGarmentColorPart = parts?
                                           .OrderByDescending(o => o.Season)
                                           .ThenByDescending(o => o.Character.Length)
                                           .ThenByDescending(o => o.Character).ToList()
                                    .FirstOrDefault(x => x.Number == currentStyleNumber);

                            if (diffentGarmentColorPart != null)
                            {
                                var newGarmentColorPart = new Part()
                                {
                                    ID = string.Empty,
                                    Number = currentStyleNumber,
                                    GarmentColorCode = itemStyle.ColorCode,
                                    GarmentColorName = itemStyle.ColorName,
                                    Season = currentSeason,
                                    CustomerID = diffentGarmentColorPart.CustomerID,
                                    Character = CharacterHelpers
                                                .GetNextCharacter(diffentGarmentColorPart.Character.ToUpper()),
                                    LastSequenceNumber = 1,
                                };

                                currentPart = newGarmentColorPart;
                                parts.Add(newGarmentColorPart);
                            }
                            else
                            {
                                var newPart = new Part()
                                {
                                    ID = string.Empty,
                                    Number = itemStyle.CustomerStyle,
                                    GarmentColorCode = itemStyle.ColorCode,
                                    GarmentColorName = itemStyle.ColorName,
                                    CustomerID = CustomerID,
                                    Season = currentSeason,
                                    Character = "A",
                                    LastSequenceNumber = 1,
                                };

                                currentPart = newPart;
                                parts.Add(newPart);
                            }
                        }
                    }

                    /// Generate LSStyle
                    if (CustomerID == "KA")
                    {
                        if (itemStyle.DeliveryPlace.Trim().ToUpper() == "FRANCE")
                        {
                            itemStyle.LSStyle += "KA";
                        }
                        else
                        {
                            itemStyle.LSStyle += "KAE";
                        }
                    }

                    itemStyle.LSStyle += itemStyle.CustomerStyle + "-";

                    if (!string.IsNullOrEmpty(currentSeason))
                    {
                        itemStyle.LSStyle += currentSeason + "-";
                    }

                    if (CustomerID == "DE")
                    {
                        if (itemStyle.PriceTermCode != "CM")
                        {
                            itemStyle.LSStyle += itemStyle.PriceTermCode + "-";
                        }
                    }

                    itemStyle.LSStyle += currentPart.Character + currentPart.LastSequenceNumber;

                    currentPart.LastSequenceNumber += 1;
                }
            }

            newParts = parts.Where(x => x.ID == string.Empty).ToList();
            parts = parts.Where(x => x.ID != string.Empty).ToList();

            newParts.ForEach(x =>
            {
                x.ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12);
            });
        }

        /// <summary>
        /// Generate LSStyle non season.. for HA
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="itemStyles"></param>
        /// <param name="CustomerID"></param>
        /// <param name="newParts"></param>
        public static void GenerateLSStyleNonSeason(
            string customerID,
            string username,
             ref List<Part> parts, List<ItemStyle> itemStyles,
            out List<Part> newParts, out string errorMessage)
        {
            newParts = new List<Part>();
            errorMessage = string.Empty;

            var currentContractNo = string.Empty;
            var currentGarmentColorCode = string.Empty;
            Part currentPart = null;

            try
            {
                foreach (var itemStyle in itemStyles)
                {
                    var sortedParts = parts?.OrderByDescending(o => o.ContractNo)
                                               //.ThenByDescending(o => o.GarmentColorCode)
                                               .ThenByDescending(o => o.Character?.Length)
                                               .ThenByDescending(o => o.Character)
                                               .ThenByDescending(o => o.LastSequenceNumber).ToList();

                    if (string.IsNullOrEmpty(itemStyle.LSStyle))
                    {
                        if (itemStyle.ContractNo != currentContractNo ||
                            itemStyle.ColorCode != currentGarmentColorCode)
                        {
                            currentContractNo = itemStyle.ContractNo;
                            currentGarmentColorCode = itemStyle.ColorCode;

                            var existPart = sortedParts?.FirstOrDefault(x => x.ContractNo == currentContractNo &&
                                                        x.GarmentColorCode.ToUpper() == currentGarmentColorCode.ToUpper());


                            if (existPart != null)
                            {
                                currentPart = existPart;
                                currentPart.SetUpdateAudit(username);
                            }
                            else
                            {
                                var diffentGarmentColorPart = sortedParts?
                                                .FirstOrDefault(x => x.ContractNo == currentContractNo);

                                if (diffentGarmentColorPart != null)
                                {

                                    var newGarmentColorPart = new Part()
                                    {
                                        ID = string.Empty,
                                        ContractNo = diffentGarmentColorPart.ContractNo,
                                        GarmentColorCode = currentGarmentColorCode,
                                        GarmentColorName = itemStyle.ColorName,
                                        CustomerID = diffentGarmentColorPart.CustomerID,
                                        Character = CharacterHelpers
                                                .GetNextCharacter(diffentGarmentColorPart.Character.ToUpper()),
                                        LastSequenceNumber = 1,
                                    };
                                    newGarmentColorPart.SetCreateAudit(username);
                                    currentPart = newGarmentColorPart;

                                    parts.Add(newGarmentColorPart);
                                }
                                else
                                {
                                    var newPart = new Part()
                                    {
                                        ID = string.Empty,
                                        ContractNo = itemStyle.ContractNo,
                                        GarmentColorCode = currentGarmentColorCode,
                                        GarmentColorName = itemStyle.ColorName,
                                        CustomerID = customerID,
                                        Character = "A",
                                        LastSequenceNumber = 1,
                                    };
                                    newPart.SetCreateAudit(username);
                                    currentPart = newPart;
                                    parts.Add(newPart);
                                }
                            }
                        }


                        itemStyle.LSStyle = itemStyle.CustomerStyle + "-" +
                                            currentPart.Character +
                                            currentPart.LastSequenceNumber;

                        currentPart.LastSequenceNumber += 1;
                    }
                }

                newParts = parts.Where(x => x.ID == string.Empty).ToList();
                parts = parts.Where(x => x.ID != string.Empty).ToList();

                newParts.ForEach(x =>
                {
                    x.ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12);
                });
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Check template valid
        /// </summary>
        /// <param name="idTemplate"></param>
        /// <param name="errorMessage"></param>
        /// <param name="worksheet"></param>
        /// <param name="rowCheck"></param>
        /// <returns></returns>
        private static bool IsValidTemplate(string idTemplate, out string errorMessage,
            ExcelWorksheet worksheet, int rowCheck = 1)
        {
            errorMessage = string.Empty;

            string[] template = ImportTemplates[idTemplate].Split(';');
            int column = 1;
            for (int i = 0; i < template.Count(); i++)
            {
                var val = worksheet.Cells[rowCheck, column++];
                if (val != null && !val.Text.ToUpper().Replace(" ", "")
                                                        .Replace("\r\n", "")
                                                        .Replace("\n", "")
                                                        .Replace("\t", "").Trim()
                        .Contains(template[i].ToUpper().Replace(" ", "")
                                                        .Replace("\r\n", "")
                                                        .Replace("\n", "")
                                                        .Replace("\t", "").Trim()))
                {
                    errorMessage = "Column " + template[i] + " incorrect template";
                    return false;
                }

            }
            return true;
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
