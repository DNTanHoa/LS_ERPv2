using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class OrderDetailProcesss
    {
        public static (List<PurchaseOrderLine>, List<StorageDetail>) 
            BulkCancel(string UserName, List<OrderDetail> orderDetails,
            List<ReservationEntry> reservationOfProductionBomEntries,
            List<ProductionBOM> productionBOMs, List<PurchaseOrderLine> purchaseOrderLines,
            List<StorageDetail> storageDetails)
        {
            var updatePurchaseOrderLines = new List<PurchaseOrderLine>();
            var updateStorageDetails = new List<StorageDetail>();

            /// Update quantity to zero
            foreach(var orderDetail in orderDetails)
            {
                orderDetail.Quantity = 0;
                orderDetail.ReservedQuantity = 0;
                orderDetail.ConsumedQuantity = 0;
            }

            /// Cancel reservation entry
            foreach(var reservationEntry in reservationOfProductionBomEntries)
            {
                if(reservationEntry.PurchaseOrderLineID != null)
                {
                    var purchaseOrderLine = purchaseOrderLines
                        .FirstOrDefault(x => x.ID == reservationEntry.PurchaseOrderLineID);

                    if (purchaseOrderLine.PurchaseOrderGroupLine != null &&
                        (purchaseOrderLine.PurchaseOrderGroupLine.ReceiptQuantity == 0 ||
                         purchaseOrderLine.PurchaseOrderGroupLine.ReceiptQuantity == null))
                    {
                        purchaseOrderLine.ReservedQuantity -= reservationEntry.ReservedQuantity;
                        purchaseOrderLine.CanReusedQuantity += reservationEntry.ReservedQuantity;

                        var existUpdatePurchaseOrderLine = updatePurchaseOrderLines
                            .FirstOrDefault(x => x.ID == purchaseOrderLine.ID);
                        if (existUpdatePurchaseOrderLine != null)
                        {
                            updatePurchaseOrderLines.Remove(existUpdatePurchaseOrderLine);
                        }

                        updatePurchaseOrderLines.Add(purchaseOrderLine);
                    }
                }
                else if(reservationEntry.StorageDetailID != null)
                {
                    var storageDetail = storageDetails
                        .FirstOrDefault(x => x.ID == reservationEntry.StorageDetailID);

                    storageDetail.ReseveredQuantity -= reservationEntry.ReservedQuantity;

                    var existUpdateStorageDetail = storageDetails
                        .FirstOrDefault(x => x.ID == storageDetail.ID);

                    if(existUpdateStorageDetail != null)
                    {
                        updateStorageDetails.Remove(existUpdateStorageDetail);
                    }

                    updateStorageDetails.Add(storageDetail);
                }
            }

            return (updatePurchaseOrderLines, updateStorageDetails);
        }

        public static (List<ReservationEntry>, List<JobHead>,
            List<PurchaseOrderLine>, List<StorageDetail>)
            BulkUpdate(string UserName, List<OrderDetail> updateOrderDetails, List<OrderDetail> orderDetails,
            List<ReservationEntry> reservationOfOrderEntries,
            List<ReservationEntry> reservationOfProductionBomEntries)
        {
            var updateReservations = new List<ReservationEntry>();
            var updateJobHeads = new List<JobHead>();
            var updatePurchaseOrderLines = new List<PurchaseOrderLine>();
            var updateStorageDetails = new List<StorageDetail>();

            foreach(var updateOrderDetail in updateOrderDetails)
            {
                var existOrderDetail = orderDetails
                    .FirstOrDefault(x => x.ID == updateOrderDetail.ID);

                if(existOrderDetail !=  null)
                {
                    /// Update quantity less than old quantity
                    if(updateOrderDetail.Quantity < existOrderDetail.Quantity)
                    {
                        /// Update reservation entry for order detail
                        
                    }
                }
            }

            return (updateReservations, updateJobHeads, 
                updatePurchaseOrderLines, updateStorageDetails);
        }
    }
}
