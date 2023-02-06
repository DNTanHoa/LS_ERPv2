using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        IQueryable<OrderDetail> GetOrderDetails();
        IQueryable<OrderDetail> GetOrderDetails(List<long> OrderDetailIDs);
        IQueryable<OrderDetail> GetOrderDetails(List<string> ItemStyleNumbers);
    }
}
