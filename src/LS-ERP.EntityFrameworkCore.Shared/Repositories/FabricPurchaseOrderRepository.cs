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
    public class FabricPurchaseOrderRepository : IFabricPurchaseOrderRepository
    {
        private readonly AppDbContext context;

        public FabricPurchaseOrderRepository(AppDbContext context)
        {
            this.context = context;
        }
        public FabricPurchaseOrder Add(FabricPurchaseOrder fabricPurchaseOrder)
        {
            return context.FabricPurchaseOrder.Add(fabricPurchaseOrder).Entity;
        }

        public void Delete(FabricPurchaseOrder fabricPurchaseOrder)
        {
            context.FabricPurchaseOrder.Remove(fabricPurchaseOrder);
        }

        public FabricPurchaseOrder GetFabricPurchaseOrder(long? ID)
        {
            return context.FabricPurchaseOrder
                         .FirstOrDefault(x => x.ID == ID);
        }

        public FabricPurchaseOrder GetFabricPurchaseOrder(string number)
        {
            return context.FabricPurchaseOrder
                         .FirstOrDefault(x => x.Number == number);
        }

        public IQueryable<FabricPurchaseOrder> GetFabricPurchaseOrders()
        {
            return context.FabricPurchaseOrder;
        }

        public IQueryable<FabricPurchaseOrder> GetFabricPurchaseOrders(string CustomerID)
        {
            return context.FabricPurchaseOrder.Where(x => x.CustomerID == CustomerID);
        }

        public IQueryable<FabricPurchaseOrder> GetFabricPurchaseOrders(List<string> Numbers)
        {
            return context.FabricPurchaseOrder
                .Where(x => Numbers.Contains(x.Number));
        }

        public void Update(FabricPurchaseOrder fabricPurchaseOrder)
        {
            context.Entry(fabricPurchaseOrder).State = EntityState.Modified;
        }
    }
}
