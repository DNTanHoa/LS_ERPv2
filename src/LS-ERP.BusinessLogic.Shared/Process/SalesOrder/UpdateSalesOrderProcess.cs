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
    public class UpdateSalesOrderProcess
    {
        public static void UpdateSalesOrder(SalesOrder oldSalesOrder, string username,
            string filePath, string fileName, string customerID,
            IEnumerable<Size> sizes,
            List<PurchaseOrderType> purchaseOrderTypes,
            IQueryable<Part> parts,
            IQueryable<ItemStyle> itemStyles,
            out List<Part> newParts,
            out List<PurchaseOrderType> newPurchaseOrderTypes,
            out List<OrderDetail> orderDetails,
            out List<OrderDetail> newOrderDetails,
            out List<ItemStyle> newItemStyle,
            out List<ItemStyle> updateItemStyle,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            orderDetails = new List<OrderDetail>();
            newOrderDetails = new List<OrderDetail>();
            newPurchaseOrderTypes = new List<PurchaseOrderType>();
            newParts = new List<Part>();
            newItemStyle = new List<ItemStyle>();
            updateItemStyle = new List<ItemStyle>();

            try
            {

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

                            switch (customerID)
                            {
                                case "GA":
                                    {
                                        var newSalesOrder = workSheet.ImportSalesOrderGA(username, sizes,
                                                        purchaseOrderTypes, itemStyles.ToList(), parts.ToList(),
                                                        out newParts, out newPurchaseOrderTypes,
                                                        out errorMessage, true);

                                        CompareDataSalesOrder(oldSalesOrder, newSalesOrder, sizes, username,
                                            out newItemStyle, out updateItemStyle,
                                            out orderDetails, out newOrderDetails,
                                            out errorMessage);
                                    }
                                    break;
                                case "IFG":
                                    {
                                        var newSalesOrder = workSheet.ImportSalesOrderIFG(username, sizes,
                                                        purchaseOrderTypes, itemStyles.ToList(), out newPurchaseOrderTypes, out errorMessage, true);

                                        CompareDataSalesOrder(oldSalesOrder, newSalesOrder, sizes, username,
                                            out newItemStyle, out updateItemStyle,
                                            out orderDetails, out newOrderDetails,
                                            out errorMessage);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            errorMessage = "File no data";
                        }
                    }
                }
                else
                {
                    errorMessage = "File not exist or invalid format";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

        }

        public static void CompareDataSalesOrder(SalesOrder oldSalesOrder, SalesOrder newSalesOrder,
            IEnumerable<Size> sizes, string username,
            out List<ItemStyle> newItemStyle,
            out List<ItemStyle> updateItemStyle,
            out List<OrderDetail> orderDetails,
            out List<OrderDetail> newOrderDetails,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            updateItemStyle = new List<ItemStyle>();
            newItemStyle = new List<ItemStyle>();
            orderDetails = new List<OrderDetail>();
            newOrderDetails = new List<OrderDetail>();

            try
            {
                var dicSizes = sizes.ToDictionary(x => x.Code);

                var config = new MapperConfiguration(
                cfg => cfg.CreateMap<ItemStyle, ItemStyle>()
                .ForMember(d => d.Number, o => o.Ignore())
                .ForMember(d => d.LSStyle, o => o.Ignore())
                .ForMember(d => d.SalesOrderID, o => o.Ignore())
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
                .ForMember(d => d.ItemStyleNumber, o => o.Ignore())
                .ForMember(d => d.ID, o => o.Ignore())
                .ForMember(d => d.ItemStyle, o => o.Ignore()));

                var mapperOrderDetail = new Mapper(configOrderDetail);

                var dicOldItemStyle = oldSalesOrder?.ItemStyles?.ToDictionary(x => x.LSStyle);

                StringBuilder strBuilder = new StringBuilder();

                foreach (var itemStyle in newSalesOrder.ItemStyles)
                {
                    //if (itemStyle.LSStyle.Equals("S2334800-A9"))
                    //{
                    //    string str = "";
                    //}

                    if (dicOldItemStyle.TryGetValue(itemStyle.LSStyle, out ItemStyle rsItemStyle))
                    {
                        mapper.Map(itemStyle, rsItemStyle);

                        decimal? totalQuantity = 0;

                        var dicNewOrder = itemStyle.OrderDetails.ToDictionary(x => x.Size);

                        foreach (var oldOrderDtl in rsItemStyle.OrderDetails)
                        {
                            if (dicNewOrder.TryGetValue(oldOrderDtl.Size, out OrderDetail rsOrderDetail))
                            {
                                var quantity = rsOrderDetail.Quantity;
                                var price = rsOrderDetail.Price;                /// update 2022-12-14
                                totalQuantity += quantity;

                                mapperOrderDetail.Map(oldOrderDtl, rsOrderDetail);
                                oldOrderDtl.Quantity = quantity;
                                oldOrderDtl.Price = price;                      /// update 2022-12-14
                                orderDetails.Add(oldOrderDtl);

                                if (dicSizes.TryGetValue(oldOrderDtl.Size, out Size rsSize))
                                {
                                    oldOrderDtl.SizeSortIndex = rsSize.SequeneceNumber;
                                }

                                //break;
                            }
                            else
                            {
                                oldOrderDtl.Quantity = 0;
                                orderDetails.Add(oldOrderDtl);
                            }
                        }

                        var dicOldSize = rsItemStyle.OrderDetails.ToDictionary(x => x.Size);
                        foreach (var item in dicNewOrder)
                        {
                            if (!dicOldSize.ContainsKey(item.Key))
                            {
                                totalQuantity += item.Value.Quantity;

                                if (dicSizes.TryGetValue(item.Value.Size, out Size rsSize))
                                {
                                    item.Value.SizeSortIndex = rsSize.SequeneceNumber;
                                }

                                item.Value.ItemStyleNumber = rsItemStyle.Number;

                                newOrderDetails.Add(item.Value);
                            }
                        }

                        rsItemStyle.ItemStyleStatusCode = "2";
                        if (totalQuantity != itemStyle.TotalQuantity && itemStyle.TotalQuantity != itemStyle.OldTotalQuantity)
                        {
                            itemStyle.OldTotalQuantity = itemStyle.TotalQuantity;
                            itemStyle.TotalQuantity = totalQuantity;
                        }

                        itemStyle.SetUpdateAudit(username);
                        updateItemStyle.Add(rsItemStyle);
                    }
                    else
                    {
                        itemStyle.SalesOrderID = oldSalesOrder.ID;
                        itemStyle.ItemStyleStatusCode = "1";

                        decimal? totalQuantity = 0;

                        foreach (var item in itemStyle.OrderDetails)
                        {
                            totalQuantity += item.Quantity;
                        }

                        itemStyle.TotalQuantity = totalQuantity;
                        itemStyle.SetCreateAudit(username);
                        newItemStyle.Add(itemStyle);
                    }
                    strBuilder.Append("," + itemStyle.LSStyle + ",");
                }

                foreach (var oldItemStyle in oldSalesOrder.ItemStyles)
                {
                    if (!strBuilder.ToString().Contains(oldItemStyle.LSStyle))
                    {
                        oldItemStyle.ItemStyleStatusCode = "3";

                        foreach (var orderDtl in oldItemStyle.OrderDetails)
                        {
                            orderDtl.Quantity = 0;

                            if (oldItemStyle.IsBomPulled != null && !(bool)oldItemStyle.IsBomPulled)
                            {
                                orderDetails.Add(orderDtl);
                            }

                        }
                        oldItemStyle.SetUpdateAudit(username);
                        updateItemStyle.Add(oldItemStyle);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

    }
}
