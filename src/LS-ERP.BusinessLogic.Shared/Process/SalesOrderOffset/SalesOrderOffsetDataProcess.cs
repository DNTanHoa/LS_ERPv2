using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class SalesOrderOffsetDataProcess
    {
        public static bool Calculate(List<SalesOrderOffset> salesOrderOffsets, List<ItemStyle> sourceItemStyles,
            List<ItemStyle> destinationItemStyles, List<ProductionBOM> sourceProductionBoms,
            out List<ProductionBOM> updatedProductionBoms, out List<SalesOrderOffset> updatedSalesOrderOffsets)
        {
            updatedProductionBoms = new List<ProductionBOM>();
            updatedSalesOrderOffsets = new List<SalesOrderOffset>();

            foreach (SalesOrderOffset salesOrderOffset in salesOrderOffsets)
            {
                if(salesOrderOffset.IsProcess != true)
                {
                    switch (salesOrderOffset.Type)
                    {
                        case "FG":
                            break;
                        case "ACC":
                            /// Đắp toàn bộ phụ liệu
                            if (string.IsNullOrEmpty(salesOrderOffset.ItemID) &&
                                string.IsNullOrEmpty(salesOrderOffset.ItemName))
                            {
                                var destinationStyle = destinationItemStyles
                                    .FirstOrDefault(x => x.LSStyle == salesOrderOffset.TargetLSStyle);
                                
                                if(destinationStyle != null)
                                {
                                    var size = destinationStyle.OrderDetails
                                        .FirstOrDefault(x => x.Size.ToUpper().Trim().Replace(" ", "") ==
                                                            salesOrderOffset.GarmentSize.ToUpper().Trim().Replace(" ", ""));
                                    if (size != null)
                                    {
                                        decimal ratio = (salesOrderOffset.OffsetQuantity / size.Quantity) ?? 1;
                                        var styleProductionBoms = sourceProductionBoms
                                            .Where(x => x.ItemStyleNumber == destinationStyle.Number);
                                        
                                        foreach(var productionBom in styleProductionBoms)
                                        {
                                            if (productionBom.ReservedQuantity == null)
                                                productionBom.ReservedQuantity = 0;

                                            productionBom.ReservedQuantity += productionBom.RequiredQuantity * ratio;
                                            productionBom.RemainQuantity = productionBom.RequiredQuantity - productionBom.ReservedQuantity;
                                        }
                                    }
                                    else
                                    {
                                        salesOrderOffset.IsSuccess = false;
                                        salesOrderOffset.ErrorMessage = 
                                            $"Order {destinationStyle.LSStyle} not have size {salesOrderOffset.GarmentSize}";
                                    }
                                }
                            }
                            /// Đắp phụ liệu riêng lẻ
                            else
                            {
                                var destinationStyle = destinationItemStyles
                                    .FirstOrDefault(x => x.LSStyle == salesOrderOffset.TargetLSStyle);

                                if (destinationStyle != null)
                                {
                                    var size = destinationStyle.OrderDetails
                                        .FirstOrDefault(x => x.Size.ToUpper().Trim().Replace(" ", "") ==
                                                            salesOrderOffset.GarmentSize.ToUpper().Trim().Replace(" ", ""));
                                    if (size != null)
                                    {
                                        decimal ratio = (salesOrderOffset.OffsetQuantity / size.Quantity) ?? 1;
                                        var styleProductionBom = sourceProductionBoms
                                            .FirstOrDefault(x => x.ItemStyleNumber == destinationStyle.Number);

                                        if(styleProductionBom != null)
                                        {
                                            if (styleProductionBom.ReservedQuantity == null)
                                                styleProductionBom.ReservedQuantity = 0;

                                            styleProductionBom.ReservedQuantity += styleProductionBom.RequiredQuantity * ratio;
                                            styleProductionBom.RemainQuantity = 
                                                styleProductionBom.RequiredQuantity - styleProductionBom.ReservedQuantity;
                                        }
                                    }
                                    else
                                    {
                                        salesOrderOffset.IsSuccess = false;
                                        salesOrderOffset.ErrorMessage =
                                            $"Order {destinationStyle.LSStyle} not have size {salesOrderOffset.GarmentSize}";
                                    }
                                }
                            }
                            break;
                        default:
                            salesOrderOffset.ErrorMessage = "Invalid offset type";
                            salesOrderOffset.IsSuccess = false;
                            break;
                    }

                    salesOrderOffset.IsProcess = true;
                    salesOrderOffset.ProcessAt = DateTime.Now;

                    /// Add to update list
                    updatedSalesOrderOffsets.Add(salesOrderOffset);
                    updatedProductionBoms = sourceProductionBoms;
                }
            }

            return true;
        }
    }
}
