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
    public class PurchaseOrderGroupLineRepository : IPurchaseOrderGroupLineRepository
    {
        private readonly AppDbContext context;

        public PurchaseOrderGroupLineRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PurchaseOrderGroupLine Add(PurchaseOrderGroupLine PurchaseOrderGroupLine)
        {
            return context.PurchaseOrderGroupLine.Add(PurchaseOrderGroupLine).Entity;
        }

        public void Delete(PurchaseOrderGroupLine PurchaseOrderGroupLine)
        {
            context.PurchaseOrderGroupLine.Remove(PurchaseOrderGroupLine);
        }

        public PurchaseOrderGroupLine GetPurchaseOrderGroupLine(long ID)
        {
            return context.PurchaseOrderGroupLine
                .Include(x => x.PurchaseOrderLines)
                .FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<PurchaseOrderGroupLine> GetPurchaseOrderGroupLines()
        {
            return context.PurchaseOrderGroupLine;
        }

        public IQueryable<PurchaseOrderGroupLine> GetPurchaseOrderGroupLines(List<long> IDs)
        {
            return context.PurchaseOrderGroupLine.Where(x => IDs.Contains(x.ID));
        }

        public IQueryable<PurchaseOrderGroupLine> GetPurchaseOrderGroupLines(List<string> PurrchaseOrderIDs)
        {
            return context.PurchaseOrderGroupLine.Where(x => PurrchaseOrderIDs.Contains(x.PurchaseOrderID));
        }

        public bool IsExist(long ID, out PurchaseOrderGroupLine PurchaseOrderGroupLine)
        {
            PurchaseOrderGroupLine = null;
            PurchaseOrderGroupLine = context.PurchaseOrderGroupLine.FirstOrDefault(x => x.ID == ID);
            return PurchaseOrderGroupLine != null ? true : false;
        }

        public bool IsExist(long ID)
        {
            var PurchaseOrderGroupLine = context.PurchaseOrderGroupLine.FirstOrDefault(x => x.ID == ID);
            return PurchaseOrderGroupLine != null ? true : false;
        }

        public void Update(PurchaseOrderGroupLine PurchaseOrderGroupLine)
        {
            context.Entry(PurchaseOrderGroupLine).State = EntityState.Modified;
        }
    }
}
