using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPurchaseOrderGroupLineRepository
    {
        PurchaseOrderGroupLine Add(PurchaseOrderGroupLine PurchaseOrderGroupLine);
        void Update(PurchaseOrderGroupLine PurchaseOrderGroupLine);
        void Delete(PurchaseOrderGroupLine PurchaseOrderGroupLine);
        IQueryable<PurchaseOrderGroupLine> GetPurchaseOrderGroupLines();
        IQueryable<PurchaseOrderGroupLine> GetPurchaseOrderGroupLines(List<long> IDs);
        IQueryable<PurchaseOrderGroupLine> GetPurchaseOrderGroupLines(List<string> PurrchaseOrderID);
        PurchaseOrderGroupLine GetPurchaseOrderGroupLine(long ID);
        bool IsExist(long ID, out PurchaseOrderGroupLine PurchaseOrderGroupLine);
        bool IsExist(long ID);
    }
}
