using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        PurchaseOrder Add(PurchaseOrder purchaseOrder);
        void Update(PurchaseOrder purchaseOrder);
        void Delete(PurchaseOrder purchaseOrder);
        IEnumerable<PurchaseOrder> GetPurchaseOrders();
        IQueryable<PurchaseOrder> GetPurchaseOrders(string customerID);
        IQueryable<PurchaseOrder> GetPurchaseOrders(List<string> number);
        IQueryable<PurchaseOrder> GetOnlyPurchaseOrders(List<string> number);
        IEnumerable<PurchaseOrder> GetPurchaseOrders(string customerID, string vendorID, DateTime fromDate, DateTime toDate);
        PurchaseOrder GetPurchaseOrder(string ID);
        bool IsExist(string ID, out PurchaseOrder purchaseOrder);
        bool IsExist(string ID);
    }
}
