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
    public class PackingListRepository : IPackingListRepository
    {
        private readonly AppDbContext context;

        public PackingListRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PackingList Add(PackingList packingList)
        {
            return context.PackingList.Add(packingList).Entity;
        }

        public void Delete(PackingList packingList)
        {
            context.PackingList.Remove(packingList);
        }

        public IQueryable<PackingList> GetPackingList()
        {
            return context.PackingList;
        }

        public IQueryable<PackingList> GetPackingList(string Code, string CustomerID)
        {
            return context.PackingList.Where(x => x.PackingListCode == Code &&
                                             x.CustomerID == CustomerID);
        }

        public IQueryable<PackingList> GetPackingList(long? InvoiceID)
        {
            return context.PackingList
                .AsNoTracking()
                .Include(x => x.PackingLines)
                .Include(x => x.ItemStyles)
                .ThenInclude(x => x.OrderDetails)
                .Where(x => InvoiceID == x.Invoice.ID);
        }

        public void Update(PackingList packingList)
        {
            context.Entry(packingList).State = EntityState.Modified;
        }
    }
}
