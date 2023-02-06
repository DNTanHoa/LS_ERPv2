using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public static class DeletePurchaseOrder
    {
        public static bool DeleteOne(PurchaseOrder purchaseOrder, List<ProductionBOM> relatedProductionBOMs,
            out List<ReservationEntry> willDeleteReservationEntries,
            out List<ProductionBOM> willUpdateProductionBOMs,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            willDeleteReservationEntries = purchaseOrder.PurchaseOrderLines
                .SelectMany(x => x.ReservationEntries).ToList();
            willUpdateProductionBOMs = new List<ProductionBOM>();

            foreach (var reservationEntry in willDeleteReservationEntries)
            {
                var productionBom = relatedProductionBOMs
                    .FirstOrDefault(x => x.ID == reservationEntry.ProductionBOMID);

                if(productionBom != null)
                {
                    productionBom.RemainQuantity += reservationEntry.ReservedQuantity ?? 0;
                    productionBom.ReservedQuantity -= reservationEntry.ReservedQuantity ?? 0;
                    willUpdateProductionBOMs.Add(productionBom);
                }
            }

            return true;
        }

        public static bool DeleteMany(List<PurchaseOrder> purchaseOrders, out string errorMessage)
        {
            errorMessage = string.Empty;
            return false;
        }
    }
}
