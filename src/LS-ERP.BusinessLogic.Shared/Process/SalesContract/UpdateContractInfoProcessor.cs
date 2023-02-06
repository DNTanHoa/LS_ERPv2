using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Extensions;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class UpdateContractInfoProcessor
    {
        public static Dictionary<string, ItemStyle> Import(string filePath, string fileName, string customerID,
            out List<string> contractNos,
            out Dictionary<string, List<Dictionary<string, OrderDetail>>> importDicOrderDetails,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            var result = new Dictionary<string, ItemStyle>();
            contractNos = new List<string>();
            importDicOrderDetails = new Dictionary<string, List<Dictionary<string, OrderDetail>>>();

            switch (customerID)
            {
                case "PU":
                    {
                        result = ReadPU(filePath, fileName, out importDicOrderDetails, out contractNos, out errorMessage);
                    }
                    break;
            }

            return result;
        }

        public static void CompareData(string userName, List<ItemStyle> oldItemStyles, Dictionary<string, ItemStyle> importDicItemStyles,
            Dictionary<string, List<Dictionary<string, OrderDetail>>> importDicOrderDetails,
            IEnumerable<Size> sizes, ref List<SalesContractDetail> salesContractDetails,
            out List<ItemStyle> updateItemStyles,
            out List<OrderDetail> newOrderDetails,
            out List<OrderDetail> updateOrderDetails,
            out string errorMessage)
        {
            updateItemStyles = new List<ItemStyle>();
            newOrderDetails = new List<OrderDetail>();
            updateOrderDetails = new List<OrderDetail>();
            errorMessage = string.Empty;
            var dicNewOrderDetails = new Dictionary<string, OrderDetail>();
            var dicSizes = sizes.ToDictionary(x => x.Code);

            var config = new MapperConfiguration(
                cfg => cfg.CreateMap<ItemStyle, ItemStyle>()
                .ForMember(d => d.SalesOrder, o => o.Ignore())
                .ForMember(d => d.PurchaseOrderType, o => o.Ignore())
                .ForMember(d => d.PurchaseOrderStatus, o => o.Ignore())
                .ForMember(d => d.OrderDetails, o => o.Ignore())
                .ForMember(d => d.IsBomPulled, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                );

            var mapper = new Mapper(config);

            var configOrderDetail = new MapperConfiguration(
            cfg => cfg.CreateMap<OrderDetail, OrderDetail>()
            .ForMember(d => d.ItemStyle, o => o.Ignore()));

            var mapperOrderDetail = new Mapper(configOrderDetail);

            #region UPDATE ITEM STYLE
            foreach (var oldItemStyle in oldItemStyles)
            {
                if (importDicItemStyles.TryGetValue(oldItemStyle.ContractNo, out ItemStyle importItemStyle))
                {
                    oldItemStyle.PurchaseOrderNumber = importItemStyle.PurchaseOrderNumber;
                    oldItemStyle.CustomerCode = importItemStyle.CustomerCode;
                    oldItemStyle.CustomerCodeNo = importItemStyle.CustomerCodeNo;
                    oldItemStyle.UCustomerCode = importItemStyle.UCustomerCode;
                    oldItemStyle.UCustomerCodeNo = importItemStyle.UCustomerCodeNo;
                    oldItemStyle.ColorCode = importItemStyle.ColorCode;
                    oldItemStyle.ColorName = importItemStyle.ColorName.Replace(importItemStyle.ColorCode, "").Trim();
                    oldItemStyle.DeliveryPlace = importItemStyle.DeliveryPlace;
                    oldItemStyle.CustomerStyle = importItemStyle.CustomerStyle;
                    oldItemStyle.Description = importItemStyle.Description;
                    oldItemStyle.ShipMode = importItemStyle.ShipMode;
                    oldItemStyle.OldTotalQuantity = oldItemStyle.TotalQuantity;
                    oldItemStyle.TotalQuantity = importItemStyle.TotalQuantity;

                    oldItemStyle.SetUpdateAudit(userName);

                    // for check new size
                    var dicOldOrderDetail = oldItemStyle.OrderDetails?.ToDictionary(x => oldItemStyle.ContractNo + x.Size);

                    foreach (var oldOrderDetail in oldItemStyle.OrderDetails)
                    {
                        if (importDicOrderDetails.TryGetValue(oldItemStyle.ContractNo, out List<Dictionary<string, OrderDetail>> rsOrderDetails))
                        {
                            var checkUpdate = false;
                            var key = oldItemStyle.ContractNo + oldOrderDetail.Size;

                            foreach (var itemODImport in rsOrderDetails)
                            {
                                if (itemODImport.TryGetValue(key, out OrderDetail orderDetail))
                                {
                                    OrderDetail updateOrderDetail = new OrderDetail();

                                    mapperOrderDetail.Map(oldOrderDetail, updateOrderDetail);

                                    updateOrderDetail.Quantity = orderDetail.Quantity; // update quantity
                                    updateOrderDetail.ShipQuantity = orderDetail.Quantity; // update quantity
                                    updateOrderDetail.ItemStyle = null;
                                    updateOrderDetails.Add(updateOrderDetail);
                                    checkUpdate = true;
                                    break;
                                }
                            }

                            if (!checkUpdate)
                            {
                                OrderDetail updateOrderDetail = new OrderDetail();

                                mapperOrderDetail.Map(oldOrderDetail, updateOrderDetail);

                                updateOrderDetail.Quantity = 0; // delete quantity
                                updateOrderDetail.ShipQuantity = 0; // delete quantity
                                updateOrderDetail.ItemStyle = null;
                                updateOrderDetails.Add(updateOrderDetail);
                            }
                        }
                    }

                    // check new size
                    if (importDicOrderDetails.TryGetValue(oldItemStyle.ContractNo, out List<Dictionary<string, OrderDetail>> rsNewOrderDetails))
                    {
                        foreach (var importOrders in rsNewOrderDetails)
                        {
                            foreach (var itemNew in importOrders)
                            {
                                if (!dicOldOrderDetail.ContainsKey(itemNew.Key))
                                {
                                    OrderDetail newOrderDetail = new OrderDetail();
                                    mapperOrderDetail.Map(itemNew.Value, newOrderDetail);

                                    if (dicSizes.TryGetValue(itemNew.Value.Size, out Size rsSize))
                                    {
                                        newOrderDetail.SizeSortIndex = rsSize.SequeneceNumber;
                                    }

                                    newOrderDetail.ItemStyleNumber = oldItemStyle.Number;

                                    dicNewOrderDetails[itemNew.Key] = newOrderDetail;
                                }
                            }
                        }
                    }


                    oldItemStyle.OrderDetails = null;

                    updateItemStyles.Add(oldItemStyle);
                }
            }

            foreach (var newOrderDetail in dicNewOrderDetails)
            {
                newOrderDetails.Add(newOrderDetail.Value);
            }
            #endregion UPDATE ITEM STYLE

            #region UPDATE SALES CONTRACT
            foreach (var contractDetail in salesContractDetails)
            {
                if (importDicItemStyles.TryGetValue(contractDetail.ContractNo, out ItemStyle importItemStyle))
                {
                    contractDetail.PurchaseOrderNumber = importItemStyle.PurchaseOrderNumber;
                    contractDetail.CustomerPO = importItemStyle.CustomerCodeNo;
                    contractDetail.UCustomterCode = importItemStyle.UCustomerCode;
                    contractDetail.UltimateCode = importItemStyle.UCustomerCodeNo;
                    contractDetail.GarmentColorCode = importItemStyle.ColorName;
                    contractDetail.CustomerStyle = importItemStyle.CustomerStyle;
                    contractDetail.Destination = importItemStyle.Destination;
                    contractDetail.CountryName = importItemStyle.DeliveryPlace;
                    contractDetail.Quantity = importItemStyle.TotalQuantity;

                    contractDetail.SetUpdateAudit(userName);
                }
            }
            #endregion UPDATE SALES CONTRACT
        }

        public static Dictionary<string, ItemStyle> ReadPU(string filePath, string fileName,
            out Dictionary<string, List<Dictionary<string, OrderDetail>>> dicOrderDetails,
            out List<string> contractNos,
            out string errorMessage)
        {
            var dicItemStyle = new Dictionary<string, ItemStyle>();
            dicOrderDetails = new Dictionary<string, List<Dictionary<string, OrderDetail>>>();
            contractNos = new List<string>();

            errorMessage = string.Empty;

            if (File.Exists(filePath) &&
                Path.GetExtension(filePath).Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets[0];
                        var dataTable = workSheet.ToDataTable(1, 1, 33, 1);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            string contract = row["Generated Order No"]?.ToString();

                            if (!String.IsNullOrEmpty(contract) && (!contract.ToLower().Replace(" ", "")
                                                                 .Equals("Generated Order No".ToLower().Replace(" ", ""))))
                            {

                                if (dicItemStyle.TryGetValue(contract, out ItemStyle rsItemStyle))
                                {
                                    if (!string.IsNullOrEmpty(row["QTY"]?.ToString()))
                                    {
                                        var orderedQuantityResult = decimal.TryParse(row["QTY"]?.ToString(), out decimal orderedQty);
                                        if (orderedQuantityResult && orderedQty > 0)
                                        {
                                            OrderDetail orderDetail = new OrderDetail();
                                            orderDetail.Quantity = orderedQty;
                                            orderDetail.ShipQuantity = orderedQty;
                                            orderDetail.Size = row["SIZ"]?.ToString();

                                            Dictionary<string, OrderDetail> childOD = new Dictionary<string, OrderDetail>();

                                            childOD[rsItemStyle.ContractNo + orderDetail.Size] = orderDetail;

                                            if (dicOrderDetails.TryGetValue(rsItemStyle.ContractNo, out List<Dictionary<string, OrderDetail>> rsOrder))
                                            {
                                                rsOrder.Add(childOD);
                                                dicOrderDetails[contract] = rsOrder;
                                            }

                                            rsItemStyle.TotalQuantity += orderDetail.Quantity;

                                            dicItemStyle[contract] = rsItemStyle;
                                        }
                                    }
                                }
                                else
                                {
                                    var itemStyle = new ItemStyle()
                                    {
                                        ContractNo = contract,
                                        PurchaseOrderNumber = row["PO NO"]?.ToString().Trim(),
                                        CustomerCode = row["CUST CODE"]?.ToString().Trim(),
                                        CustomerCodeNo = row["CUST CO NO"]?.ToString().Trim(),
                                        UCustomerCode = row["UCUST CODE"]?.ToString().Trim(),
                                        UCustomerCodeNo = row["UCUST CO NO"]?.ToString().Trim(),
                                        ColorCode = row["COLOR"]?.ToString()?.Trim(),
                                        ColorName = row["Col Description"]?.ToString().Trim(),
                                        DeliveryPlace = row["Country"]?.ToString(),
                                        CustomerStyle = row["STYLE NO"]?.ToString(),
                                        Description = row["STYLE DESC"]?.ToString(),
                                        ShipMode = row["ShipMode"]?.ToString(),
                                        Destination = row["Destination"]?.ToString()
                                    };

                                    if (DateTime.TryParse(row["CHD"]?.ToString(), out DateTime updateCHD))
                                    {
                                        itemStyle.ContractDate = updateCHD;
                                    }

                                    if (!string.IsNullOrEmpty(row["QTY"]?.ToString()))
                                    {
                                        var orderedQuantityResult = decimal.TryParse(row["QTY"]?.ToString(), out decimal orderedQty);
                                        if (orderedQuantityResult && orderedQty > 0)
                                        {
                                            OrderDetail orderDetail = new OrderDetail();
                                            orderDetail.Quantity = orderedQty;
                                            orderDetail.ShipQuantity = orderedQty;
                                            orderDetail.Size = row["SIZ"]?.ToString();

                                            Dictionary<string, OrderDetail> childOrderDetail = new Dictionary<string, OrderDetail>();

                                            childOrderDetail[itemStyle.ContractNo + orderDetail.Size] = orderDetail;
                                            List<Dictionary<string, OrderDetail>> childs = new List<Dictionary<string, OrderDetail>>();
                                            childs.Add(childOrderDetail);
                                            dicOrderDetails[itemStyle.ContractNo] = childs;

                                            itemStyle.TotalQuantity = orderDetail.Quantity;
                                        }
                                    }

                                    dicItemStyle[itemStyle.ContractNo] = itemStyle;
                                    contractNos.Add(itemStyle.ContractNo);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                errorMessage = "Invalid file";
            }

            return dicItemStyle;
        }
    }
}
