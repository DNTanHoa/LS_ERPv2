using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IFabricPurchaseOrderRepository
    {
        FabricPurchaseOrder Add(FabricPurchaseOrder fabricPurchaseOrder);
        void Update(FabricPurchaseOrder fabricPurchaseOrder);
        void Delete(FabricPurchaseOrder fabricPurchaseOrder);
        IQueryable<FabricPurchaseOrder> GetFabricPurchaseOrders();
        IQueryable<FabricPurchaseOrder> GetFabricPurchaseOrders(string CustomerID);
        IQueryable<FabricPurchaseOrder> GetFabricPurchaseOrders(List<string> Numbers);
        FabricPurchaseOrder GetFabricPurchaseOrder(long? ID);
        FabricPurchaseOrder GetFabricPurchaseOrder(string number);
    }
}
