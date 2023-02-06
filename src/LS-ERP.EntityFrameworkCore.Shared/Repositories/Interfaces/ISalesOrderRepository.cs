using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface ISalesOrderRepository
    {
        SalesOrder Add(SalesOrder salesOrder);
        void Update(SalesOrder salesOrder);
        void Delete(SalesOrder salesOrder);
        IEnumerable<SalesOrder> GetSalesOrders();
        IQueryable<SalesOrder> GetSalesOrders(string CustomerID);
        IQueryable<SalesOrder> GetSalesOrders(List<string> salesOrders);
        bool ExistFileSalesOrders(string fileName);
        bool IsExist(string ID);
        SalesOrder GetSalesOrder(string ID);
    }
}
