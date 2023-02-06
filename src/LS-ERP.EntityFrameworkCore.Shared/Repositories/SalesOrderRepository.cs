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
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly AppDbContext context;

        public SalesOrderRepository(AppDbContext context)
        {
            this.context = context;
        }

        public SalesOrder Add(SalesOrder salesOrder)
        {
            return context.SalesOrders.Add(salesOrder).Entity;
        }

        public void Delete(SalesOrder salesOrder)
        {
            context.SalesOrders.Remove(salesOrder);
        }

        public bool ExistFileSalesOrders(string fileName)
        {
            var salesOrder = context.SalesOrders.FirstOrDefault(x => x.FileName == fileName);
            return salesOrder != null ? true : false;
        }
        public bool IsExist(string ID)
        {
            var salesOrder = context.SalesOrders.FirstOrDefault(x => x.ID == ID);
            return salesOrder != null ? true : false;
        }

        public SalesOrder GetSalesOrder(string ID)
        {
            return context.SalesOrders
                .Include(x => x.Customer)
                .Include(x => x.Division)
                .Include(x => x.Currency)
                .Include(x => x.Brand)
                .Include(x => x.PaymentTerm)
                .Include(x => x.PriceTerm)
                .Include(x => x.ItemStyles)
                .ThenInclude(x => x.OrderDetails)
                .FirstOrDefault(x => x.ID == ID);
        }

        public IEnumerable<SalesOrder> GetSalesOrders()
        {
            return context.SalesOrders
                .Include(x => x.Customer)
                .Include(x => x.Division)
                .Include(x => x.Currency)
                .Include(x => x.Brand)
                .Include(x => x.PaymentTerm)
                .Include(x => x.PriceTerm);
        }

        public IQueryable<SalesOrder> GetSalesOrders(string CustomerID)
        {
            return context.SalesOrders
                .Include(x => x.ItemStyles)
                .ThenInclude(x => x.OrderDetails)
                .Include(x => x.ItemStyles)
                .ThenInclude(x => x.Barcodes)
                .Where(x => x.CustomerID == CustomerID);
        }

        public void Update(SalesOrder salesOrder)
        {
            context.Entry(salesOrder).State = EntityState.Modified;
        }

        public IQueryable<SalesOrder> GetSalesOrders(List<string> salesOrders)
        {
            return context.SalesOrders
               .Where(x => salesOrders.Contains(x.ID));
        }
    }
}
