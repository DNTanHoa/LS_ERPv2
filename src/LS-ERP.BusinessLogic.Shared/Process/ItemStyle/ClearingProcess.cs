using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class ClearingProcess
    {
        public static void ClearingStyle(List<ItemStyle> itemStyles, List<StorageDetail> storageDetails, 
            out List<ReservationEntry> reservationEntries, string Username = "")
        {
            reservationEntries = new List<ReservationEntry>();

            foreach(var itemStyle in itemStyles)
            {
                foreach(var orderDetail in itemStyle.OrderDetails)
                {
                    var totalQuantiy = orderDetail.Quantity - orderDetail.ReservedQuantity - orderDetail.ConsumedQuantity;
                    var finishGoods = storageDetails.Where(x => x.CustomerStyle == itemStyle.CustomerStyle &&
                                                            x.GarmentColorCode == itemStyle.ColorCode &&
                                                            x.GarmentColorName == itemStyle.ColorName &&
                                                            x.GarmentSize == orderDetail.Size &&
                                                            x.Season == itemStyle.Season &&
                                                            x.CanUseQuantity > 0 &&
                                                            string.IsNullOrEmpty(x.ItemID) &&
                                                            string.IsNullOrEmpty(x.ItemName));
                    foreach(var finishGood in finishGoods)
                    {
                        var reservationEntry = new ReservationEntry()
                        {
                            OrderDetailID = orderDetail.ID,
                            StorageDetailID = finishGood.ID,
                        };

                        reservationEntry.SetCreateAudit(Username);

                        if (finishGood.CanUseQuantity >= totalQuantiy)
                        {
                            finishGood.CanUseQuantity -= totalQuantiy;
                            reservationEntry.ReservedQuantity = totalQuantiy;
                            reservationEntry.AvailableQuantity = totalQuantiy;
                            
                            break;
                        }
                        else
                        {
                            totalQuantiy -= finishGood.CanUseQuantity;
                            reservationEntry.ReservedQuantity = finishGood.CanUseQuantity;
                            reservationEntry.AvailableQuantity = finishGood.CanUseQuantity;
                        }

                        reservationEntries.Add(reservationEntry);
                        orderDetail.ReservationEntries.Add(reservationEntry);
                        finishGood.ReservationEntries.Add(reservationEntry);

                        finishGood.CalculateQuantity();
                        orderDetail.CalculateQuantity();
                    }
                }
            }
        }

        public static void ClearingBOM(List<ProductionBOM> productionBOMs, List<StorageDetail> storageDetails,
            out List<ReservationEntry> reservationEntries, out List<ProductionBOM> updateProductionBoms,
            out List<StorageDetail> updateStorageDetails, string Username = "")
        {
            reservationEntries = new List<ReservationEntry>();
            updateProductionBoms = new List<ProductionBOM>();
            updateStorageDetails = new List<StorageDetail>();

            foreach (var prodcutionBOM in productionBOMs)
            {
                var storageDetail = storageDetails
                    .FirstOrDefault(x => x.ItemID == prodcutionBOM.ItemID &&
                                         x.ItemName == prodcutionBOM.ItemName &&
                                         x.ItemColorCode == prodcutionBOM.ItemColorCode &&
                                         x.ItemColorName == prodcutionBOM.ItemColorName &&
                                         x.Specify == prodcutionBOM.Specify);
            }
        }
    }
}
