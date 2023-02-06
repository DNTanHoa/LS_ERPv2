using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class PurchaseOrderLineRepository : IPurchaseOrderLineRepository
    {
        private readonly AppDbContext context;
        public PurchaseOrderLineRepository(AppDbContext context)
        {
            this.context = context;
        }
        public IQueryable<PurchaseOrderLine> GetPurchaseOrderLines()
        {
            return context.PurchaseOrderLine;
        }
        public IQueryable<PurchaseOrderLine> GetPurchaseOrderLines(
            List<long> PurchaseOrderLineIDs)
        {
            return context.PurchaseOrderLine
                .Include(x => x.PurchaseOrderGroupLine)
                .Where(x => PurchaseOrderLineIDs.Contains(x.ID));
        }

        public IQueryable<PurchaseOrderLine> GetPurchaseOrderLinesFollowPO(List<string> PurchaseOrderIDs)
        {
            return context.PurchaseOrderLine
               .Where(x => PurchaseOrderIDs.Contains(x.PurchaseOrderID));
        }

        public IQueryable<PurchaseOrderLine> GetPurchaseOrderLinesMatching(List<string> purchaseOrderNumbers)
        {
            return context.PurchaseOrderLine
                .Include(x => x.PurchaseOrder)
                .Where(x => purchaseOrderNumbers.Contains(x.PurchaseOrder.Number) &&
                            (x.MatchingShipment == null || (x.MatchingShipment != null && x.MatchingShipment == false)));
                //.Select(x => new PurchaseOrderLine
                //{
                //    ID = x.ID,
                //    CustomerPurchaseOrderNumber = x.CustomerPurchaseOrderNumber,
                //    ContractNo = x.ContractNo,
                //    MaterialTypeClass = x.MaterialTypeClass,
                //    ItemID = x.ItemID,
                //    ItemName = x.ItemName,
                //    ItemColorName = x.ItemColorName,
                //    DeliveryNote = x.DeliveryNote,
                //    VendorConfirmDate = x.VendorConfirmDate,
                //    PurchaseOrderID = x.PurchaseOrderID,
                //    PurchaseOrderGroupLineID = x.PurchaseOrderGroupLineID
                //});
        }
    }
}
