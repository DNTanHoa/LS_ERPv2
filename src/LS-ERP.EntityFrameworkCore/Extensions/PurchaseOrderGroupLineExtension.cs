using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Extensions
{
    public static class PurchaseOrderGroupLineExtension
    {
        public static void CalculateQuantity(this PurchaseOrderGroupLine purchaseOrderGroupLine)
        {
        }

        public static decimal RoundQuantity(this PurchaseOrderGroupLine purchaseOrderGroupLine)
        {
            decimal quantity = 0;
            if (purchaseOrderGroupLine.Unit.RoundDown != null && (bool)purchaseOrderGroupLine.Unit.RoundDown)
            {
                quantity = Math.Floor((decimal)purchaseOrderGroupLine.Quantity);
            }
            else if (purchaseOrderGroupLine.Unit.RoundUp != null && (bool)purchaseOrderGroupLine.Unit.RoundUp)
            {
                quantity = Math.Ceiling((decimal)purchaseOrderGroupLine.Quantity);
            }
            else
            {
                quantity = purchaseOrderGroupLine.Quantity ?? 0;
            }
            return quantity;
        }
    }
}
