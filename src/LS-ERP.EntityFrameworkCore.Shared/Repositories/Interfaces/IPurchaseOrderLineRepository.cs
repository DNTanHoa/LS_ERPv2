using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPurchaseOrderLineRepository
    {
        public IQueryable<PurchaseOrderLine> GetPurchaseOrderLines();
        public IQueryable<PurchaseOrderLine> GetPurchaseOrderLines(List<long> PurchaseOrderLineID);
        public IQueryable<PurchaseOrderLine> GetPurchaseOrderLinesFollowPO(List<string> PurchaseOrderID);
        public IQueryable<PurchaseOrderLine> GetPurchaseOrderLinesMatching(List<string> purchaseOrderNumbers);
    }
}
