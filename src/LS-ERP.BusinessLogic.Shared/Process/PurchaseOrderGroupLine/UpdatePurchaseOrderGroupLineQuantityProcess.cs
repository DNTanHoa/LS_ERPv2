using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class UpdatePurchaseOrderGroupLineQuantityProcess
    {
        public static bool CalculateReceiptQuantity(List<PurchaseOrderGroupLine> purchaseOrderGroupLines,
           List<ReceiptGroupLine> receiptGroupLines, out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach(var purchaseOrderGroupLine in purchaseOrderGroupLines)
            {
                if (purchaseOrderGroupLine.ReceiptQuantity == null)
                    purchaseOrderGroupLine.ReceiptQuantity = 0;

                purchaseOrderGroupLine.ReceiptQuantity += receiptGroupLines
                    .Where(r => r.PurchaseOrderGroupLineID == purchaseOrderGroupLine.ID)
                    .Sum(s => s.ReceiptQuantity);
            }

            return true;
        }
    }
}
