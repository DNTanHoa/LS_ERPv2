using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class CancelItemStyleProcess
    {
        public static void BulkCancel(string UserName, List<ItemStyle> itemStyles,
            List<ProductionBOM> productionBOMs,
            List<ReservationEntry> reservationEntries, 
            out List<ReservationEntry> willDeleteReservationEntry, 
            out List<PurchaseOrderLine> willUpdatePurchaseOrderLines)
        {

            willDeleteReservationEntry = new List<ReservationEntry>();
            willUpdatePurchaseOrderLines = new List<PurchaseOrderLine>();

            foreach (var itemStyle in itemStyles)
            {
                /// Update purchase order status to cancel
                itemStyle.PurchaseOrderStatusCode = "CANCEL";
                itemStyle.SetUpdateAudit(UserName);
            }

            /// Check for produciton bom was purchased
            foreach (var productionBOM in productionBOMs)
            {
                if(productionBOM.ReservedQuantity > 0)
                {
                    var bomReservationEntries = reservationEntries
                        .Where(x => x.ProductionBOMID == productionBOM.ID);

                    willDeleteReservationEntry.AddRange(bomReservationEntries);

                    foreach(var bomReservationEntry in bomReservationEntries)
                    {
                        var purchaseOrderLine = bomReservationEntry.PurchaseOrderLine; 

                        if(purchaseOrderLine != null)
                        {
                            purchaseOrderLine.CanReusedQuantity += 
                                bomReservationEntry.ReservedQuantity;
                            purchaseOrderLine.ReservedQuantity -=
                                bomReservationEntry.ReservedQuantity;

                            purchaseOrderLine.SetUpdateAudit(UserName);
                            willUpdatePurchaseOrderLines.Add(purchaseOrderLine);
                        }
                    }
                }
            }
        }
    }
}
