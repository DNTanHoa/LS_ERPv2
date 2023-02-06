using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class UpdatePurchaseOrderLineQuantity
    {
        public static void UpdateReservedQuantity(List<PurchaseOrderLine> purchaseOrderLines)
        {
            if(purchaseOrderLines != null &&
               purchaseOrderLines.Any())
            {
                foreach(var purchaseOrderLine in purchaseOrderLines)
                {
                    if (purchaseOrderLine.ReservedQuantity == null)
                        purchaseOrderLine.ReservedQuantity = 0;

                    purchaseOrderLine.ReservedQuantity = purchaseOrderLine.ReservationEntries?
                        .Sum(x => x.ReservedQuantity);
                }
            }
        }
    }
}
