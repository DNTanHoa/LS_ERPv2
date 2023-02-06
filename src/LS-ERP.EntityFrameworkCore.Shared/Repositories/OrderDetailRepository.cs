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
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext context;

        public OrderDetailRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<OrderDetail> GetOrderDetails()
        {
            return context.OrderDetail;
        }

        public IQueryable<OrderDetail> GetOrderDetails(List<long> OrderDetailIDs)
        {
            return context.OrderDetail
                .Include(x => x.ReservationEntries)
                .ThenInclude(r => r.JobHead)
                .Where(x => OrderDetailIDs.Contains(x.ID));
        }

        public IQueryable<OrderDetail> GetOrderDetails(List<string> ItemStyleNumbers)
        {
            return context.OrderDetail
                .Include(x => x.ReservationEntries)
                .ThenInclude(r => r.JobHead)
                .Where(x => ItemStyleNumbers.Contains(x.ItemStyleNumber));
        }
    }
}
