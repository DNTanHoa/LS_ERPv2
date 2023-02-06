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
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext context;

        public PurchaseOrderRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PurchaseOrder Add(PurchaseOrder purchaseOrder)
        {
            return context.Add(purchaseOrder).Entity;
        }

        public void Delete(PurchaseOrder purchaseOrder)
        {
            context.PurchaseOrder.Remove(purchaseOrder);
        }

        public IQueryable<PurchaseOrder> GetOnlyPurchaseOrders(List<string> number)
        {
            return context.PurchaseOrder.Where(x => number.Contains(x.Number));
        }

        public PurchaseOrder GetPurchaseOrder(string ID)
        {
            return context.PurchaseOrder
                .Include(x => x.Company)
                .Include(x => x.Vendor)
                .Include(x => x.PurchaseOrderGroupLines)
                .Include(x => x.PurchaseOrderLines)
                .ThenInclude(x => x.ReservationEntries)
                .AsNoTracking()
                .FirstOrDefault(x => x.ID == ID);
        }

        public IEnumerable<PurchaseOrder> GetPurchaseOrders()
        {
            return context.PurchaseOrder;
        }

        public IQueryable<PurchaseOrder> GetPurchaseOrders(string customerID)
        {
            return context.PurchaseOrder.Where(x => x.CustomerID == customerID);
        }
        public IEnumerable<PurchaseOrder> GetPurchaseOrders(string customerID, string vendorID, DateTime fromDate, DateTime toDate)
        {
            return context.PurchaseOrder.Where(x => (x.CustomerID == customerID || string.IsNullOrEmpty(customerID))
                                                 && (x.VendorID == vendorID || string.IsNullOrEmpty(vendorID))
                                                 && x.OrderDate >= fromDate
                                                 && x.OrderDate <= toDate);
        }

        public IQueryable<PurchaseOrder> GetPurchaseOrders(List<string> number)
        {
            return context.PurchaseOrder
                .Include(x => x.PurchaseOrderGroupLines)
                .Include(x => x.PurchaseOrderLines)
                .Where(x => number.Contains(x.Number));
        }

        public bool IsExist(string ID, out PurchaseOrder purchaseOrder)
        {
            purchaseOrder = context.PurchaseOrder
                .AsNoTracking().FirstOrDefault(x => x.ID == ID);
            return purchaseOrder != null ? true : false;
        }

        public bool IsExist(string ID)
        {
            var purchaseOrder = context.PurchaseOrder.FirstOrDefault(x => x.ID == ID);
            return purchaseOrder != null ? true : false;
        }

        public void Update(PurchaseOrder purchaseOrder)
        {
            context.Entry(purchaseOrder).State = EntityState.Modified;
        }
    }
}
